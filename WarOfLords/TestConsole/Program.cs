using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WarOfLords.Client;
using WarOfLords.Common;
using WarOfLords.Common.Models;
using static WarOfLords.Client.MapHelper;

namespace TestConsole
{
    class Program
    {
        static bool stop = false;
        static BattleManager BattleManager;
        static void Main(string[] args)
        {
            TileMap map = new TileMap(20, 26, 40, 40);
            map.AddReachableTilesInColumn(5, 20, 2);
            map.AddReachableTilesInColumn(5, 20, 7);
            map.AddReachableTilesInColumn(5, 20, 12);
            map.AddReachableTilesInColumn(5, 20, 17);
            map.AddReachableTilesInColumn(11, 14, 1);
            map.AddReachableTilesInColumn(11, 14, 18);

            map.AddReachableTilesInRow(2, 17, 5);
            map.AddReachableTilesInRow(2, 17, 10);
            map.AddReachableTilesInRow(2, 17, 15);
            map.AddReachableTilesInRow(2, 17, 20);
            map.AddReachableTilesInRow(8, 11, 4);
            map.AddReachableTilesInRow(8, 11, 21);

            map.AddHubTile(new MapTileIndex(2, 5));
            map.AddHubTile(new MapTileIndex(7, 5));
            map.AddHubTile(new MapTileIndex(12, 5));
            map.AddHubTile(new MapTileIndex(17, 5));

            map.AddHubTile(new MapTileIndex(2, 10));
            map.AddHubTile(new MapTileIndex(7, 10));
            map.AddHubTile(new MapTileIndex(12, 10));
            map.AddHubTile(new MapTileIndex(17, 10));

            map.AddHubTile(new MapTileIndex(2, 15));
            map.AddHubTile(new MapTileIndex(7, 15));
            map.AddHubTile(new MapTileIndex(12, 15));
            map.AddHubTile(new MapTileIndex(17, 15));

            map.AddHubTile(new MapTileIndex(2, 20));
            map.AddHubTile(new MapTileIndex(7, 20));
            map.AddHubTile(new MapTileIndex(12, 20));
            map.AddHubTile(new MapTileIndex(17, 20));

            map.AddHubTile(new MapTileIndex(8, 5));
            map.AddHubTile(new MapTileIndex(9, 5));
            map.AddHubTile(new MapTileIndex(10, 5));
            map.AddHubTile(new MapTileIndex(11, 5));

            map.AddHubTile(new MapTileIndex(8, 20));
            map.AddHubTile(new MapTileIndex(9, 20));
            map.AddHubTile(new MapTileIndex(10, 20));
            map.AddHubTile(new MapTileIndex(11, 20));

            map.AddHubTile(new MapTileIndex(2, 11));
            map.AddHubTile(new MapTileIndex(2, 12));
            map.AddHubTile(new MapTileIndex(2, 13));
            map.AddHubTile(new MapTileIndex(2, 14));

            map.AddHubTile(new MapTileIndex(17, 11));
            map.AddHubTile(new MapTileIndex(17, 12));
            map.AddHubTile(new MapTileIndex(17, 13));
            map.AddHubTile(new MapTileIndex(17, 14));

            var navResult = map.SearchWay(new MapTileIndex(8, 4), new MapTileIndex(11, 21));
            navResult.Optimize();
        }

       //static void Main(string[] args)
       // {
       //     string country1 = "C1";
       //     string federation1 = "F1";

        //     string country2 = "C2";
        //     string federation2 = "F2";

        //     ArmyMaker armyMaker = new ArmyMaker();
        //     MapMaker mapMaker = new MapMaker();
        //     BattleManager = new BattleManager();
        //     BattleManager.CountryFederationMapDic.AddOrUpdate(country1,federation1, (key,oldvalue)=> federation1);
        //     BattleManager.CountryFederationMapDic.AddOrUpdate(country2, federation2, (key, oldvalue) => federation2);

        //     BattleManager.CountryBattleTeamsMapDic.AddOrUpdate(country1, new  List<BattleTeam>(), (key, oldValue) => oldValue);
        //     BattleManager.CountryBattleTeamsMapDic.AddOrUpdate(country2, new List<BattleTeam>(), (key, oldValue) => oldValue);

        //     var battleTeam1 = ArmyMaker.MakeBattleTeam(BattleManager, country1, federation1, "C1.T1", 100, 100, 2, 2, 2);
        //     var battleTeam2 = ArmyMaker.MakeBattleTeam(BattleManager, country2, federation2, "C2.T2", 100, 100, 2, 2, 2);
        //     battleTeam1.Setting.MaxLockPerEnemy = 5;
        //     battleTeam2.Setting.MaxLockPerEnemy = 2;

        //     //var battleTeam1 = ArmyMaker.MakeBattleTeam(country1, federation1, "C1.T1", 1, 1, 1, 1, 1);
        //     //var battleTeam2 = ArmyMaker.MakeBattleTeam(country2, federation2, "C2.T2", 1, 1, 1, 1, 1);
        //     BattleManager.CountryBattleTeamsMapDic[country1].Add(battleTeam1);
        //     BattleManager.CountryBattleTeamsMapDic[country2].Add(battleTeam2);

        //     MapVertex originPoint = new MapVertex
        //     {
        //         X = 0,
        //         Y = 0,
        //         Z = 0
        //     };

        //     var map = MapMaker.MakeGridMap(originPoint, 3, 3, 100);

        //     battleTeam1.EnterMap(map);
        //     battleTeam1.Position = originPoint;

        //     battleTeam2.EnterMap(map);
        //     battleTeam2.Position = new MapVertex
        //     {
        //         X = 150,
        //         Y = 300,
        //         Z = 0
        //     };

        //     var center = new MapVertex
        //     {
        //         X = 150,
        //         Y = 150,
        //         Z = 0
        //     };

        //     CancellationTokenSource cancelSourceForMessageLoopUp = new CancellationTokenSource();
        //     long fileSuffix = DateTime.Now.ToFileTime();
        //     var lookupTask1 = LookupMessages(country1, "Country1_" + fileSuffix,cancelSourceForMessageLoopUp.Token);
        //     var lookupTask2 = LookupMessages(country2, "Country2_" + fileSuffix, cancelSourceForMessageLoopUp.Token);

        //     //battleTeam1.SelectAll();
        //     battleTeam2.SelectAll();

        //     battleTeam1.SetEnemy(battleTeam2.Country, battleTeam2.Federation);
        //     battleTeam2.SetEnemy(battleTeam1.Country, battleTeam1.Federation);

        //     battleTeam1.CreateSubTeam<BowMan>(1);
        //     battleTeam1.CreateSubTeam(2, 500, 0, 0, 0, 0);
        //     battleTeam1.CreateSubTeam(3, 0, 0, 2, 2, 2);

        //     List<Task> taskList = new List<Task>();

        //     var attackTask2 = battleTeam2.Attack(center);
        //     var targetPos2 = new MapVertex
        //     {
        //         X = 145,
        //         Y = 145,
        //         Z = 0
        //     };
        //     battleTeam1.SelectSubTeam(1);
        //     var attackTask =  battleTeam1.Attack(center);

        //     battleTeam1.SelectSubTeam(2);
        //     var attackTask1_2 = battleTeam1.Move(targetPos2);
        //     var attackTask1_2_1 = attackTask1_2.ContinueWith(async(t) => {
        //         battleTeam1.SelectSubTeam(2);
        //         await battleTeam1.Attack(center);
        //     });

        //     battleTeam1.SelectSubTeam(3);
        //     var attackTask1_3 = battleTeam1.Attack(center);

        //     var detectTask = battleTeam1.Detect(cancelSourceForMessageLoopUp.Token);
        //     var detectTask2 = battleTeam2.Detect(cancelSourceForMessageLoopUp.Token);

        //     Task.WaitAll(attackTask, 
        //         attackTask2, 
        //         attackTask1_2, 
        //         attackTask1_2_1,
        //         attackTask1_3);

        //     while (!battleTeam1.AllDead() && !battleTeam2.AllDead())
        //     {
        //         var attackTask3 = battleTeam2.Attack(center);
        //         var attackTask4 = battleTeam1.Attack(center);
        //         Task.WaitAll(attackTask3, attackTask4);
        //     }

        //     cancelSourceForMessageLoopUp.Cancel();
        //     Task.WaitAll(detectTask, detectTask2);
        //     stop = true;
        //     Task.WaitAll(lookupTask1, lookupTask2);
        // }

        async static Task LookupMessages(string country, string outputFile, CancellationToken cancelToken)
        {
            DateTime startTime = DateTime.Now;
            int count = 0;
            string fileName = string.Format("{0}_{1}{2}", outputFile, count, ".txt");
            if (File.Exists(fileName))
            {
                File.Delete(fileName); 
            }
            var writer = File.CreateText(fileName);
            int lines = 0;
            int maxLine = 100000;

            while (!cancelToken.IsCancellationRequested && !stop)
            {
                if (BattleManager.CountryMessageQueueMapDic.ContainsKey(country) &&
                    BattleManager.CountryMessageQueueMapDic[country].HasMore)
                {
                    string message = BattleManager.CountryMessageQueueMapDic[country].Dequeue();
                    writer.WriteLine(message);
                    //Console.WriteLine(message);
                    lines++;
                    if (lines >= maxLine)
                    {
                        Console.WriteLine("Log file >> {0}", fileName);
                        writer.Flush();
                        writer.Close();
                        count++;
                        fileName = string.Format("{0}_{1}{2}", outputFile, count, ".txt");
                        if (File.Exists(fileName))
                        {
                            File.Delete(fileName);
                        }
                        writer = File.CreateText(fileName);
                        lines = 0;
                    }
                }
                await Task.Delay(TimeSpan.FromMilliseconds(3));
            }

            if (BattleManager.CountryMessageQueueMapDic.ContainsKey(country))
            {
                while (BattleManager.CountryMessageQueueMapDic[country].HasMore)
                {
                    string message = BattleManager.CountryMessageQueueMapDic[country].Dequeue();
                    writer.WriteLine(message);
                    //Console.WriteLine(message);
                    lines++;
                    if (lines >= maxLine)
                    {
                        Console.WriteLine("Log file >> {0}", fileName);
                        writer.Flush();
                        writer.Close();
                        count++;
                        fileName = string.Format("{0}_{1}{2}", outputFile, count, ".txt");
                        if (File.Exists(fileName))
                        {
                            File.Delete(fileName);
                        }
                        writer = File.CreateText(fileName);
                        lines = 0;
                    }
                }
            }

            writer.Flush();
            writer.Close();
            fileName = string.Format("{0}_{1}{2}", outputFile, "Result", ".txt");
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            writer = File.CreateText(fileName);

            DateTime endTime = DateTime.Now;
            writer.WriteLine("Start Time: {0}", startTime);
            writer.WriteLine("End Time: {0}", endTime);
            foreach (var teamList in BattleManager.CountryBattleTeamsMapDic.Values)
            {
                writer.WriteLine("=================================================================================");
                foreach (var team in teamList)
                {
                    writer.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                    writer.WriteLine("Team:{0} {1}, Alives:{2}", team.Country, team.Name, team.AllAliveUnits.Count());
                    foreach(var alive in team.AllAliveUnits)
                    {
                        writer.WriteLine("{0}, Health:{1}/{2}, {3}, {4}", alive, alive.Health, alive.MaxHealth, alive.TotalDamageToEnemies, alive.TotalKilled);
                    }
                    writer.WriteLine("---------------------------------------------------");
                    foreach (var unit in team.AllBattleUnits)
                    {
                        writer.WriteLine("{0}\t{1}\t{2}", unit, unit.TotalDamageToEnemies, unit.TotalKilled);
                    }
                }
            }

            writer.Flush();
            writer.Close();
        }
    }
}
