using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WarOfLords.Common.Models
{
    public abstract class BattleUnit : IFightCapability, IMovementCapability
    {
        public BattleUnit(int id, string name)
        {
            this.Id = id;
            this.Name = name;

            this.AttackRange = 1;
            this.Damage = 10;
            this.DamageRange = 1;
            this.Defense = 0;
            this.DodgeRatio = 0;
            this.Health = 100;
            this.HitInterval = 1000;
            this.HitRatio = 50;
            this.IsMultiDamageTargets = false;
            this.IsRangeDamage = false;
            this.MaxDamageTargets = 1;
            this.MaxHealth = 100;
            this.TotalDamageToEnemies = 0;
            this.TotalKilled = 0;

            this.SightRange = 50;
            this.MoveInterval = 500;
            this.MoveStepLength = 1;

            this.Selected = false;
        }

        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual bool Selected { get; set; }
        public virtual int MoveInterval { get; set; }
        public virtual int MoveStepLength { get; set; }

        private MapVertex position;
        public delegate void OnPositionInitedHandler(MapVertex initPos);
        public event OnPositionInitedHandler OnPositionInited;
        public virtual MapVertex Position {
            get
            {
                return this.position;
            }
            set
            {
                position = value;
                if(position !=null)
                {
                    this.OnPositionInited?.Invoke(position);
                }
            }
        }


        public delegate void OnPositionChangedHanlder(MapVertex orgPos, MapVertex movedPos, TimeSpan duration);

        public event OnPositionChangedHanlder OnPositionChanged;

        public virtual int SightRange { get; set; }

        private int health;
        public virtual int Health
        {
            get
            {
                return health;
            }
            set
            {
                lock(this.heathLock)
                {
                    int change = value - health;

                    if (this.Team != null)
                    {
                        OnHealthChanged?.Invoke(health, value);
                        if (value <= 0)
                        {
                            this.Team.MessageQueue.EnqueueMessage(this, "I am dead!!!");
                        }
                        else if(change != 0)
                        {
                            this.Team.MessageQueue.EnqueueMessage(this, "Health changed from {0} to {1}", health, value);
                        }
                    }
                    health = value;
                }
            }
        }

        public delegate void OnHealthChangedHandler(int from, int to);

        public event OnHealthChangedHandler OnHealthChanged;

        public virtual int MaxHealth { get; set; }
        public virtual int Defense { get; set; }
        public virtual int DodgeRatio { get; set; }

        public virtual int AttackRange { get; set; }

        public virtual int Damage { get; set; }

        public virtual int DamageRange { get; set; }

        public virtual int HitInterval { get; set; }

        public virtual int HitRatio { get; set; }

        public virtual bool IsMultiDamageTargets { get; set; }

        public virtual bool IsRangeDamage { get; set; }

        public virtual int MaxDamageTargets { get; set; }

        public virtual int TotalDamageToEnemies { get; set; }

        public int TotalKilled { get; set; }

        public BattleTeam Team { get; set; }
        public int TotalCuredHP { get; private set; }

        private object moveLock = new object();
        private object attackLock = new object();
        private object heathLock = new object();
        private CancellationTokenSource moveCancelTokenSource = null;
        private CancellationTokenSource attackCancelTokenSource = null;

        private IEnumerable<BattleUnit> DiscoverTargetUnits(int range)
        {
            if (this is IMedicalCapability)
            {
                return this.Team.DiscoverUnhealthUnits(this.position, range);
            }
            else
            {
                return this.Team.DiscoverEnemies(this.Position,range);
            }
        }

        private async Task<int> MoveForAttack(MapVertex direction)
        {
            if (this.Health <= 0) return 0;
            CancellationToken  cancelToken;
            this.PrepareMove(out cancelToken);
            int movedTotal = 0;
            //var routePoints = MapHelper.DefaultMap.GetRoutePoints(this.position, direction);
            MapTileIndex fromTile = this.Team.BattleFieldMap.GetTileIndex(this.position);
            MapTileIndex toTile = this.Team.BattleFieldMap.GetTileIndex(direction);

            if (fromTile == null || toTile == null) return 0;
            TileNavigationResult navResult = this.Team.BattleFieldMap.SearchWay(fromTile, toTile);
            if (navResult == null) return 0;

            foreach (var tile in navResult.RoutingTiles)
            {
                MapVertex routePos = this.Team.BattleFieldMap.FindReachablePointInTile(this, tile);
                if (!cancelToken.IsCancellationRequested)
                {
                    this.Team.MessageQueue.EnqueueMessage(this, "Moving to {0} for attack", routePos);
                    int distance = routePos.DistanceTo(this.Position);
                    if (distance == 0) continue;

                    int stepNeeded = distance / this.MoveStepLength;
                    if (stepNeeded == 0) stepNeeded = 1;
                    IEnumerable<BattleUnit> targetUnits = DiscoverTargetUnits(this.AttackRange);
                    MapVertex fromPos = this.position.Clone();
                    TimeSpan duration = TimeSpan.FromMilliseconds(0);
                    TimeSpan moveInterval = TheLordTime.FromMilliseconds(this.MoveInterval);
                    TimeSpan actionInterval = TimeSpan.FromMilliseconds(500);
                    while (stepNeeded > 0 && !cancelToken.IsCancellationRequested && targetUnits.Count() == 0)
                    {
                        duration += moveInterval;

                        this.Position.Moved(routePos, this.MoveStepLength, this.Team.BattleFieldMap);
                        this.OnPositionChanged?.Invoke(fromPos.Clone(), this.Position.Clone(), moveInterval);
                        await Task.Delay(moveInterval);

                        //if (duration > actionInterval)
                        //{
                        //    this.OnPositionChanged?.Invoke(fromPos.Clone(), this.Position.Clone(), actionInterval);
                        fromPos = this.position;
                        //    duration -= actionInterval;
                        //}
                        movedTotal += this.MoveStepLength;
                        stepNeeded--;
                        if (movedTotal % 10 == 0) this.Team.MessageQueue.EnqueueMessage(this, "Moved to {0}", this.Position);

                        targetUnits = DiscoverTargetUnits(this.AttackRange);
                    }
                    //this.OnPositionChanged?.Invoke(fromPos.Clone(), this.Position.Clone(), duration);
                    if (targetUnits.Count() > 0)
                    {
                        return movedTotal;
                    }
                    if (!cancelToken.IsCancellationRequested && stepNeeded <= 0 && !this.position.Equals(routePos))
                    {
                        this.OnPositionChanged?.Invoke(this.position.Clone(), routePos.Clone(), moveInterval);
                        this.Position.X = routePos.X;
                        this.Position.Y = routePos.Y;
                        this.Position.Z = routePos.Z;
                        await Task.Delay(moveInterval);
                        this.Team.MessageQueue.EnqueueMessage(this, "Moved to {0}", this.Position);
                    }
                }
            }
            return movedTotal;
        }

        public async virtual Task<int> MoveTo(MapVertex pos)
        {
            this.Team.MessageQueue.EnqueueMessage(this, "Moving to {0}", pos);
            if (this.Health <= 0) return 0;
            CancellationToken cancelToken;
            this.PrepareMove(out cancelToken);

            MapTileIndex fromTile = this.Team.BattleFieldMap.GetTileIndex(this.position);
            MapTileIndex toTile = this.Team.BattleFieldMap.GetTileIndex(pos);

            if (fromTile == null || toTile == null) return 0;
            TileNavigationResult navResult = this.Team.BattleFieldMap.SearchWay(fromTile, toTile);
            if (navResult == null) return 0;
            int movedTotal = 0;
            foreach (var tile in navResult.RoutingTiles)
            {
                MapVertex routePos = this.Team.BattleFieldMap.FindReachablePointInTile(this, tile);
                if (!cancelToken.IsCancellationRequested)
                {
                    int distance = routePos.DistanceTo(Position);
                    int stepNeeded = distance / this.MoveStepLength;
                    
                    while (stepNeeded > 0 && !cancelToken.IsCancellationRequested)
                    {
                        
                        if (!cancelToken.IsCancellationRequested)
                        {
                            var orgPos = this.Position.Clone();
                            this.Position.Moved(routePos, this.MoveStepLength, this.Team.BattleFieldMap);
                            this.OnPositionChanged?.Invoke(orgPos, this.Position.Clone(), TheLordTime.FromMilliseconds(this.MoveInterval));
                            await Task.Delay(TheLordTime.FromMilliseconds(this.MoveInterval));
                            this.Team.MessageQueue.EnqueueMessage(this, "Moved to {0}", this.Position);
                            movedTotal += this.MoveStepLength;
                            stepNeeded--;
                            if (stepNeeded % 10 == 0) this.Team.MessageQueue.EnqueueMessage(this, "Moved to {0}", this.Position);
                        }
                    }
                    if (!cancelToken.IsCancellationRequested && !this.position.Equals(routePos))
                    {
                        this.OnPositionChanged?.Invoke(this.Position.Clone(), routePos, TheLordTime.FromMilliseconds(this.MoveInterval));
                        this.Position.X = routePos.X;
                        this.Position.Y = routePos.Y;
                        this.Position.Z = routePos.Z;
                        await Task.Delay(TheLordTime.FromMilliseconds(this.MoveInterval));
                        this.Team.MessageQueue.EnqueueMessage(this, "Moved to {0}", this.Position);
                    }
                    
                }
            }
            return movedTotal;
        }

        public virtual bool IsInSight(MapVertex targetPos)
        {
            return Util.IsInSight(this.Position, this.SightRange, targetPos);
        }

        public virtual bool IsInAttackRange(MapVertex targetPos)
        {
            return Util.IsInSight(this.Position, this.AttackRange, targetPos);
        }

        public async virtual Task Attack(BattleUnit target)
        {
            if (this.Health <= 0) return;
            this.Team.MessageQueue.EnqueueMessage(this, "Attacking target unit: {0}", target);
            CancellationToken cancelToken;
            this.moveCancelTokenSource.Cancel();
            this.PrepareAttack(out cancelToken);
            if (!cancelToken.IsCancellationRequested)
            {
                await this.attack(target, cancelToken);
            }
        }

        public async virtual Task Attack(MapVertex targetPos)
        {
            if (this.Health <= 0) return;
            CancellationToken cancelToken;
            if (this.moveCancelTokenSource != null && !this.moveCancelTokenSource.IsCancellationRequested)
            {
                this.moveCancelTokenSource.Cancel();
            }
            this.PrepareAttack(out cancelToken);
            this.Team.MessageQueue.EnqueueMessage(this, "Attacking target position: {0}", targetPos);
            while (!cancelToken.IsCancellationRequested  && this.Health > 0 && !this.Team.EnemyAllDead())
            {
                await this.MoveForAttack(targetPos);

                IEnumerable<BattleUnit> targetUnits = DiscoverTargetUnits(this.SightRange)
                    .OrderBy(en => this.position.DistanceTo(en.position));

                while (this.Health > 0 && targetUnits.Count() > 0 && !cancelToken.IsCancellationRequested)
                {
                    BattleUnit target = null;
                    if (this is IMedicalCapability)
                    {
                        target = targetUnits.FirstOrDefault();
                        if (target != null)
                        {
                            await this.cure(target, cancelToken);
                        }
                    }
                    else
                    {
                        
                        foreach (var unit in targetUnits)
                        {

                            if (this.Team.LockEnemy(unit))
                            {
                                target = unit;
                                break;
                            }
                        }
                        if (target != null)
                        {
                            await this.attack(target, cancelToken);
                            this.Team.UnlockEnemy(target);
                        }
                        else
                        {
                            //select first
                            target = targetUnits.FirstOrDefault();
                            if (target != null)
                            {
                                await this.attack(target, cancelToken);
                            }
                            else
                            {
                                break;
                            }
                        } 
                    }
                    targetUnits = DiscoverTargetUnits(this.SightRange);
                }
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                //if(!cancelToken.IsCancellationRequested && this.Health > 0)
                //{
                //   if(this.Position != targetPos) await this.MoveTo(targetPos);
                //}
            }
        }

        private void PrepareMove(out CancellationToken cancelToken)
        {
            lock(this.moveLock)
            {
                if(this.moveCancelTokenSource != null && !this.moveCancelTokenSource.IsCancellationRequested)
                {
                    this.moveCancelTokenSource.Cancel();
                }
                this.moveCancelTokenSource = new CancellationTokenSource();
                cancelToken = this.moveCancelTokenSource.Token;
            }
        }

        private void PrepareAttack(out CancellationToken cancelToken)
        {
            lock (this.attackLock)
            {
                if (this.attackCancelTokenSource != null && !this.attackCancelTokenSource.IsCancellationRequested)
                {
                    this.attackCancelTokenSource.Cancel();
                }
                this.attackCancelTokenSource = new CancellationTokenSource();
                cancelToken = this.attackCancelTokenSource.Token;
            }
        }

        private async Task attack(BattleUnit target, CancellationToken cancelToken)
        {
            this.Team.MessageQueue.EnqueueMessage(this, "Attacking target : {0}", target);
            this.Team.LockEnemy(target);
            while (this.health > 0 && target.Health > 0 && !cancelToken.IsCancellationRequested)
            {
                if (this.Position.DistanceTo(target.Position) > this.AttackRange)
                {
                    await this.MoveForAttack(target.Position);
                }
                await Task.Delay(TheLordTime.FromMilliseconds(this.HitInterval));
                if (!cancelToken.IsCancellationRequested)
                {
                    if (!this.IsRangeDamage)
                    {
                        damage(target);
                    }
                    else
                    {
                        damage(target);
                        var enemies = this.Team.DiscoverEnemies(target.Position, this.DamageRange);
                        int damageTargets = this.MaxDamageTargets;
                        foreach (var enemy in enemies)
                        {
                            if (!enemy.Equals(target) && damageTargets > 0)
                            {
                                damage(enemy, rangeDamage:true);
                            }
                            damageTargets--;
                            if (damageTargets <= 0) break;
                        }
                    }
                }
            }
            this.Team.UnlockEnemy(target);
        }

        private void damage(BattleUnit target, bool rangeDamage = false)
        {
            if (this.Health > 0 && target.Health > 0)
            {
                if(this.Position.DistanceTo(target.Position) > this.AttackRange)
                {
                    this.Team.MessageQueue.EnqueueMessage(this, "Attack missed to {0}, out of attack range.", target);
                    return;
                }
                //Random ran = new Random();
                //if (ran.Next(0, 100) > Math.Abs(this.HitRatio - target.DodgeRatio))
                //{
                //    //attack missed
                //    this.Team.MessageQueue.EnqueueMessage(this, "Attack missed to {0}", target);
                //    return;
                //}
                int damage = Math.Max(this.Damage - target.Defense, this.Damage >> 2);
                target.Health -= damage;
                this.TotalDamageToEnemies += damage;
                if (target.health <= 0)
                {
                    this.TotalKilled++;
                    this.Team.MessageQueue.EnqueueMessage(this, "Killed {0} by damage {1}, range damage: {2}, My Pos: {3}, Enemy Pos: {4}", target, damage, rangeDamage, this.Position, target.Position);
                }
                else
                {
                    this.Team.MessageQueue.EnqueueMessage(this, "Damaged {0} to {1}, range damage: {2}, My Pos: {3}, Enemy Pos: {4}", damage, target, rangeDamage, this.Position, target.Position);
                }
            }
        }

        private async Task cure(BattleUnit target, CancellationToken cancelToken)
        {  
            IMedicalCapability medicalUnit = this as IMedicalCapability;
            if (medicalUnit == null) return;
            this.Team.MessageQueue.EnqueueMessage(this, "Curing target : {0}", target);

            while (this.health > 0 && target.Health > 0 && target.Health < target.MaxHealth  && !cancelToken.IsCancellationRequested)
            {
                if (this.Position.DistanceTo(target.Position) > this.AttackRange)
                {
                    await this.MoveForAttack(target.Position);
                }
                await Task.Delay(TheLordTime.FromMilliseconds(medicalUnit.RecoverInterval));
                if (!cancelToken.IsCancellationRequested)
                {
                    if (!medicalUnit.IsRangeRecover)
                    {
                        cure(target);
                    }
                    else
                    {
                        cure(target);
                        var targets = DiscoverTargetUnits(medicalUnit.RecoverRange);
                        int cureTargets = medicalUnit.MaxRecoverTargets;
                        foreach (var tgt in targets)
                        {
                            if (!tgt.Equals(target) && cureTargets > 0)
                            {
                                cure(tgt, rangeCure: true);
                            }
                            cureTargets--;
                            if (cureTargets <= 0) break;
                        }
                    }
                }
            }
        }

        private void cure(BattleUnit target, bool rangeCure = false)
        {
            IMedicalCapability medicalUnit = this as IMedicalCapability;
            if (medicalUnit == null) return;

            if (this.Health > 0 && target.Health > 0 && target.Health < target.MaxHealth)
            {
                if (this.Position.DistanceTo(target.Position) > this.AttackRange)
                {
                    this.Team.MessageQueue.EnqueueMessage(this, "Cure missed to {0}, out of attack range.", target);
                    return;
                }
                int cureHP = Math.Min(medicalUnit.Recover, target.MaxHealth - target.Health);
                target.Health += cureHP;
                this.TotalCuredHP += cureHP;
            }
        }

        private const string BattleUnitFormatString = "[{0}]{1}~{2}";
        public override string ToString()
        {
            return string.Format(BattleUnitFormatString, this.GetType().Name, this.Name, this.Id);
        }

        public override bool Equals(object obj)
        {
            var comp = obj as BattleUnit;
            if (comp == null) return false;
            if (this.Id == comp.Id) return true;
            return false;
        }

        public override int GetHashCode()
        {
            return this.Id;
        }
    }
}
