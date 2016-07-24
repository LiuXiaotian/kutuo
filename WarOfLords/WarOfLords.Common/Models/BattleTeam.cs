using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WarOfLords.Common.Models
{
    public class BattleTeam
    {
        private ConcurrentBag<BattleUnit> BattleUnits;
        private string enemyCountry;
        private string enemyFederation;
        private Dictionary<int, CancellationTokenSource> moveCancelTokenSourceList;
        private ConcurrentDictionary<int, BattleUnit> DetectedEnemyUnits;
        private IEnumerable<MapTileIndex> positionTies;
        private ConcurrentDictionary<string, IEnumerable<BattleUnit>> SubTeamDics;
        private BattleManager BattleManager;

        public BattleTeam(int id, string name, string country, string federation, TileMap map, IEnumerable<MapTileIndex> teamTiles, BattleManager battleManager)
        {
            this.Id = id;
            this.Name = name;
            this.Country = country;
            this.Federation = federation;
            this.BattleUnits = new ConcurrentBag<BattleUnit>();
            this.moveCancelTokenSourceList = new Dictionary<int, CancellationTokenSource>();
            this.enemyCountry = null;
            this.enemyFederation = null;
            this.ActiveAttack = false;
            this.DetectedEnemyUnits = new ConcurrentDictionary<int, BattleUnit>();
            EnemyLockNumberDic = new ConcurrentDictionary<int, int>();
            this.Setting = new BattleSetting();
            this.SubTeamDics = new ConcurrentDictionary<string, IEnumerable<BattleUnit>>();
            this.BattleManager = battleManager;
            this.BattleFieldMap = map;
            this.positionTies = teamTiles;
        }

        public IEnumerable< MapTileIndex> InTiles
        {
            get
            {
                return this.AllAliveUnits.Select(_ => BattleFieldMap.GetTileIndex(_.Position.X, _.Position.Y)).Distinct();
            }
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Country { get; set; }

        public string Federation { get; set; }

        public bool ActiveAttack { get; set; }

        public TileMap BattleFieldMap { get; set; }

        public BattleSetting Setting { get; set; }

        public ConcurrentDictionary<int, int> EnemyLockNumberDic
        {
            get; set;
        }

        public IEnumerable<BattleUnit> AllBattleUnits
        {
            get
            {
                return this.BattleUnits.AsEnumerable();
            }
        }

        public IEnumerable<SwordMan> SwordManList
        {
            get
            {
                return this.BattleUnits.Where(_=> _ is SwordMan).Select(unit=>unit as SwordMan);
            }
        }

        public IEnumerable<BowMan> BowManList
        {
            get
            {
                return this.BattleUnits.Where(_ => _ is BowMan).Select(unit => unit as BowMan);
            }
        }
        public IEnumerable<MedicalMan> MedicalManList
        {
            get
            {
                return this.BattleUnits.Where(_ => _ is MedicalMan).Select(unit => unit as MedicalMan);
            }
        }
        public IEnumerable<WeaponOperator> WeaponOperatorList
        {
            get
            {
                return this.BattleUnits.Where(_ => _ is WeaponOperator).Select(unit => unit as WeaponOperator);
            }
        }
        public IEnumerable<Trebuchet> TrebuchetList
        {
            get
            {
                return this.BattleUnits.Where(_ => _ is Trebuchet).Select(unit => unit as Trebuchet);
            }
        }
        public IEnumerable<Scout> ScoutList
        {
            get
            {
                return this.BattleUnits.Where(_ => _ is Scout).Select(unit => unit as Scout);
            }
        }

        public CountryMessageQueue MessageQueue
        {
            get
            {
                while (!BattleManager.CountryMessageQueueMapDic.ContainsKey(Country))
                {
                    BattleManager.CountryMessageQueueMapDic.TryAdd(Country, new CountryMessageQueue(Country, Federation));
                    Task.WaitAll(Task.Delay(100));
                }
                return BattleManager.CountryMessageQueueMapDic[Country];
            }
        }

        public void CreateSubTeam(
            string subTeamId,
            int swordManCount,
            int bowManCount,
            int medicalManCount,
            int trebuchetCount,
            int scoutCount)
        {
            List<BattleUnit> unitList = new List<BattleUnit>();
            unitList.AddRange(this.SwordManList.Where(u => u.Health > 0 && u.SubTeamId == null).Take(swordManCount));
            unitList.AddRange(this.BowManList.Where(u => u.Health > 0 && u.SubTeamId == null).Take(bowManCount));
            unitList.AddRange(this.MedicalManList.Where(u => u.Health > 0 && u.SubTeamId == null).Take(medicalManCount));
            unitList.AddRange(this.TrebuchetList.Where(u => u.Health > 0 && u.SubTeamId == null).Take(trebuchetCount));
            unitList.AddRange(this.ScoutList.Where(u => u.Health > 0 && u.SubTeamId == null).Take(scoutCount));

            unitList.ForEach(_ => _.SubTeamId = subTeamId);

            //this.SubTeamDics.AddOrUpdate(subTeamId, unitList, (key, oldValue) => { return unitList; });
        }

        public void CreateSubTeam<T>(string subTeamId) where T : BattleUnit
        {
            var unitList = this.BattleUnits.Where(u => u.Health > 0 && (u as T) != null);
            unitList.ToList().ForEach(_ => _.SubTeamId = subTeamId);
            //this.SubTeamDics.AddOrUpdate(subTeamId, unitList, (key, oldValue) => { return unitList; });
        }

        public void SelectSubTeam(string subTeamId)
        {
            lock (this.selectedUnitsLock)
            {
                this.unselectSelectedUnits();
                IEnumerable<BattleUnit> selectList;
                if (this.SubTeamDics.TryGetValue(subTeamId, out selectList))
                {
                    selectList.Where(_=>_.SubTeamId == subTeamId).ToList().ForEach(u => u.Selected = true);
                }
            }
        }

        public IEnumerable<string> CurrentSubTeams
        {
            get
            {
                return this.AllAliveUnits.Select(_ => _.SubTeamId).Distinct();
            }
        }

        public delegate void AddBattleUnitEventHanlder(BattleUnit unit);

        public event AddBattleUnitEventHanlder OnAddBattleUnitSucceeded;

        public void AddBattleUnit(BattleUnit unit)
        {
            Type t = unit.GetType();
            if (this.BattleUnits.Any(_ => object.ReferenceEquals(unit, _)))
            {
                return;
            }
            this.BattleUnits.Add(unit);
            unit.Team = this;
            unit.Position = this.BattleFieldMap.FindReachablePointInTiles(unit, this.positionTies);
            this.OnAddBattleUnitSucceeded?.Invoke(unit);
            this.MessageQueue.EnqueueMessage(this, "BattleUnit Jioned the team: {0}, {1}~{2}", t.Name, unit.Name, unit.Id);
        }

        public void AddBattleUnitRange(IEnumerable<BattleUnit> units)
        {
            foreach(var unit in units)
            {
                this.AddBattleUnit(unit);
            }
        }

        public void SetEnemy(string country, string federation)
        {
            this.enemyFederation = federation;
            this.enemyCountry = country;
            this.MessageQueue.EnqueueMessage(this, "Enemy setting changed to country {0} at federation {1}", country, federation);
        }

        public bool IsInSight(MapPoint pos)
        {
            foreach(var unit in this.AllAliveUnits.OrderByDescending(m=>m.SightRange))
            {
                if (Util.IsInSight(unit.Position, unit.SightRange, pos)) return true;
            }
            return false;
        }

        public async Task Move(MapVertex pos)
        {
            this.MessageQueue.EnqueueMessage(this, "Moving to {0}", pos);
            List<Task> taskList = new List<Task>();
            List<BattleUnit> selectList = new List<BattleUnit>(); ;
            lock (selectedUnitsLock)
            {
                selectList.AddRange(this.SelectedUnits);
            }
            foreach (var mov in selectList)
            {
                taskList.Add(mov.MoveTo(pos));
                await Task.Delay(TimeSpan.FromMilliseconds(1));
            }
            await Task.WhenAll(taskList.ToArray());
        }

        public async Task Attack(BattleUnit unit)
        {
            this.MessageQueue.EnqueueMessage(this, "Attacking {0}~{1} from {2}@{3}", unit.Name, unit.Id, unit.Team.Country, unit.Team.Federation);
            List<Task> taskList = new List<Task>();
            List<BattleUnit> selectList = new List<BattleUnit>(); ;
            lock (selectedUnitsLock)
            {
                selectList.AddRange(this.SelectedUnits);
            }
            foreach (var mov in selectList)
            {
                taskList.Add(mov.Attack(unit));
                await Task.Delay(TimeSpan.FromMilliseconds(1));
            }
            await Task.WhenAll(taskList.ToArray());
        }

        private object selectedUnitsLock = new object();
        public async Task Attack(MapVertex pos)
        {
            this.MessageQueue.EnqueueMessage(this, "Attacking at {0}", pos);
            List<Task> taskList = new List<Task>();
            List<BattleUnit> selectList = new List<BattleUnit>(); ;
            lock (selectedUnitsLock)
            {
                selectList.AddRange(this.SelectedUnits);
            }
            foreach (var mov in selectList)
            {
                taskList.Add(mov.Attack(pos));
                await Task.Delay(TimeSpan.FromMilliseconds(1));
            }
            await Task.WhenAll(taskList.ToArray());
        }

        public void SelectAll()
        {
            this.AllAliveUnits.ToList().ForEach(_ => _.Selected = true);
            this.MessageQueue.EnqueueMessage(this, "All units are selected");

        }

        public void UnselectAll()
        {
            this.BattleUnits.ToList().ForEach(_ => _.Selected = false);
            this.MessageQueue.EnqueueMessage(this, "All units are unselected");
        }

        public IEnumerable<BattleUnit> InRangeUnits(MapVertex center, int radius)
        {
            return this.AllAliveUnits.Where(unit => unit.Position.DistanceTo(center) <= radius);
        }

        public IEnumerable<BattleUnit> InRangeUnits(IEnumerable<MapCircle> circles)
        {
            return this.AllAliveUnits.Where(unit => DetectHelper.IsInRange(unit.Position, circles));
        }

        public int SelectMultiple(MapRegion selectRegion)
        {
            lock(this.selectedUnitsLock)
            {
                unselectSelectedUnits();
                var units = this.AllAliveUnits.Where(unit => Util.IsInRegion(unit.Position, selectRegion));
                units.ToList().ForEach(_ => _.Selected = true);
                this.MessageQueue.EnqueueMessage(this, "{0} units are unselected", units.Count());
                return units.Count();
            }
        }

        public BattleUnit SelectSingle(MapVertex p)
        {
            lock (this.selectedUnitsLock)
            {
                unselectSelectedUnits();
                var units = this.AllAliveUnits.Where(unit => unit.Position.DistanceTo(p) <= 1);
                if (units.Count() > 0)
                {
                    var selected = units.First();
                    this.MessageQueue.EnqueueMessage(this, "Unit {0}~{1} is unselected", selected.Name, selected.Id);
                    return selected;
                }
                return null;
            }
        }

        public IEnumerable<BattleUnit> DiscoverEnemies(MapVertex center, int radius)
        {
            List<BattleUnit> unitList = new List<BattleUnit>();
            return this.DetectedEnemyUnits.Values.Where(_ => _.Health > 0 && _.Position.DistanceTo(center) <= radius); /*&& (!this.EnemyLockNumberDic.ContainsKey(_.Id) || this.EnemyLockNumberDic[_.Id] < this.Setting.MaxLockPerEnemy))*/
        }



        private IEnumerable<BattleUnit> DetectEnemies(/*MapVertex center, int radius*/)
        {
            List<BattleUnit> unitList = new List<BattleUnit>();
            if (this.enemyCountry != null)
            {
                if (BattleManager.CountryBattleTeamsMapDic.ContainsKey(this.enemyCountry) && BattleManager.CountryBattleTeamsMapDic[this.enemyCountry] != null)
                {
                    IEnumerable<MapCircle> circles = this.AllAliveUnits.Select(_=>new MapCircle(_.Position, _.SightRange));
                    foreach (var enemyTeam in BattleManager.CountryBattleTeamsMapDic[this.enemyCountry])
                    {
                        
                        unitList.AddRange(enemyTeam.InRangeUnits(circles));
                    }
                }
            }
            return unitList.AsEnumerable();
        }

        private const int DetectIntervalInMs = 1000;
        public async Task Detect(CancellationToken cancelToken)
        {
            while(!cancelToken.IsCancellationRequested && !this.AllDead() && !this.EnemyAllDead())
            {
                // detect enimies
                var enemiesList =  this.DetectEnemies();
                enemiesList.ToList().ForEach(
                    _ => 
                    this.DetectedEnemyUnits.AddOrUpdate(
                        _.Id, 
                        _, 
                        (key, oldValue) => { return oldValue; }));

                this.MessageQueue.EnqueueMessage(this, "Enimies detected: {0}", enemiesList.Count());

                await Task.Delay(TimeSpan.FromMilliseconds(DetectIntervalInMs));
            }

            while (!cancelToken.IsCancellationRequested && !this.DetectedEnemyUnits.IsEmpty && !this.AllDead())
            {
                BattleUnit takeUnit;
                var deadList = this.DetectedEnemyUnits.Values.Where(_=>_.Health <=0 );
                deadList.ToList().ForEach(_ => this.DetectedEnemyUnits.TryRemove(_.Id, out takeUnit));
            }
        }



        public bool AllDead()
        {
            return this.AllAliveUnits.Count() == 0;
        }

        public bool EnemyAllDead()
        {
            if (this.enemyCountry != null)
            {
                if (BattleManager.CountryBattleTeamsMapDic.ContainsKey(this.enemyCountry) && BattleManager.CountryBattleTeamsMapDic[this.enemyCountry] != null)
                {
                    return BattleManager.CountryBattleTeamsMapDic[this.enemyCountry].TrueForAll(_ => _.AllDead());
                }
            }
            return true;
        }

        private IEnumerable<BattleUnit> SelectedUnits
        {
            get
            {
                return this.AllAliveUnits.Where(_ => _.Selected);
            }
        }

        public IEnumerable<BattleUnit> AllAliveUnits
        {
            get
            {
                return this.BattleUnits.Where(_=>_.Health > 0);
            }
        }

        public bool LockEnemy(BattleUnit unit)
        {
            int orgValue = 0;
            bool hasValue = this.EnemyLockNumberDic.TryGetValue(unit.Id, out orgValue);
            if(hasValue)
            {
                if (orgValue >= this.Setting.MaxLockPerEnemy) return false;
            }
            this.EnemyLockNumberDic.AddOrUpdate(unit.Id, 1, (key, oldValue) => { return oldValue + 1; });
            return true;
        }

        public void UnlockEnemy(BattleUnit unit)
        {
            this.EnemyLockNumberDic.AddOrUpdate(unit.Id, 0, (key, oldValue) => { return oldValue - 1; });
        }

        public IEnumerable<BattleUnit> DiscoverUnhealthUnits(MapVertex center, int radius)
        {
            return this.AllAliveUnits.Where(_ => _.Health > 0 && _.Health < _.MaxHealth && _.Position.DistanceTo(center) <= radius);
        }

        private void unselectSelectedUnits()
        {
            this.BattleUnits.ToList().ForEach(_ => _.Selected = false);
        }

        //private void AutoDeployBattleUnits()
        //{
        //    var units = this.AllAliveUnits;/*.ToList().ForEach(_ => _.Position = this.teamPosition.Clone());*/
        //    MapVertex teamPos = this.teamPosition.Clone();
        //    int leftY = teamPos.Y;
        //    int leftX = teamPos.X;
        //    int rightX = teamPos.X;
        //    int rightY = teamPos.Y;

        //    int leftXMax = teamPos.X - this.Setting.DefaultFormationWidth / 2;
        //    int rightXMax = teamPos.X + (this.Setting.DefaultFormationWidth - this.Setting.DefaultFormationWidth / 2);
        //    if (leftXMax < 0)
        //    {
        //        rightXMax -= leftXMax;
        //        leftXMax = 0;
        //    }
        //    bool leftTurn = true;
        //    foreach(var unit in units)
        //    {
        //        //bool reachable = false; 
        //        if(leftTurn)
        //        {
        //            MapVertex point = new MapVertex
        //            {
        //                X = leftX,
        //                Y = leftY,
        //                Z = 0
        //            };

        //            while(!this.BattleFieldMap.PointReachable(point))
        //            {
        //                leftX--;
        //                if(leftX < leftXMax)
        //                {
        //                    leftX = teamPos.X;
        //                    leftY++;
        //                }
        //                point = new MapVertex
        //                {
        //                    X = leftX,
        //                    Y = leftY,
        //                    Z = 0
        //                };
        //            }

        //            this.BattleFieldMap.EnterPoint(point);
        //            unit.Position = point;
        //        }
        //        else
        //        {
        //            MapVertex point = new MapVertex
        //            {
        //                X = rightX,
        //                Y = rightY,
        //                Z = 0
        //            };

        //            while (!this.BattleFieldMap.PointReachable(point))
        //            {
        //                rightX ++;
        //                if (rightX > rightXMax)
        //                {
        //                    rightX = teamPos.X;
        //                    rightY++;
        //                }
        //                point = new MapVertex
        //                {
        //                    X = rightX,
        //                    Y = rightY,
        //                    Z = 0
        //                };
        //            }

        //            this.BattleFieldMap.EnterPoint(point);
        //            unit.Position = point;
        //        }
        //        leftTurn = !leftTurn;
        //    }
        //}

        public int TotalHealth
        {
            get
            {
                return this.AllAliveUnits.Sum(bu => bu.Health > 0 ? bu.Health : 0);
            }
        }
    }
}
