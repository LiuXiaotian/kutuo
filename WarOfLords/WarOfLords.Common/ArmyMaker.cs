using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarOfLords.Common.Models;

namespace WarOfLords.Common
{
    public class ArmyMaker
    {
        private static int lastId = 0;
        private static object idLock = new object();

        public static int NewId()
        {
            lock(idLock)
            {
                return lastId++;
            }
        }

        public static IEnumerable<SwordMan> MakeSwordMen(string namePrefix, int count)
        {
            List<SwordMan> unitList = new List<SwordMan>();
            for (int i = 0; i < count; i++)
            {
                int id = NewId();
                string name = namePrefix + id;
                unitList.Add(new SwordMan(id, name));
            }
            return unitList;
        }

        public static IEnumerable<BowMan> MakeBowMen(string namePrefix, int count)
        {
            List<BowMan> unitList = new List<BowMan>();
            for (int i = 0; i < count; i++)
            {
                int id = NewId();
                string name = namePrefix + id;
                unitList.Add(new BowMan(id, name));
            }
            return unitList;
        }

        public static IEnumerable<MedicalMan> MakeMedicalMen(string namePrefix, int count)
        {
            List<MedicalMan> unitList = new List<MedicalMan>();
            for (int i = 0; i < count; i++)
            {
                int id = NewId();
                string name = namePrefix + id;
                unitList.Add(new MedicalMan(id, name));
            }
            return unitList;
        }

        public static IEnumerable<WeaponOperator> MakeWeaponOperators(string namePrefix, int count)
        {
            List<WeaponOperator> unitList = new List<WeaponOperator>();
            for (int i = 0; i < count; i++)
            {
                int id = NewId();
                string name = namePrefix + id;
                unitList.Add(new WeaponOperator(id, name));
            }
            return unitList;
        }

        public static IEnumerable<Scout> MakeScouts(string namePrefix, int count)
        {
            List<Scout> unitList = new List<Scout>();
            for (int i = 0; i < count; i++)
            {
                int id = NewId();
                string name = namePrefix + id;
                unitList.Add(new Scout(id, name));
            }
            return unitList;
        }

        public static IEnumerable<Trebuchet> MakeTrebuchets(string namePrefix, int count, bool makeOperators = true)
        {
            List<Trebuchet> unitList = new List<Trebuchet>();
            for (int i = 0; i < count; i++)
            {
                int id = NewId();
                string name = namePrefix + id;
                var weapon = new Trebuchet(id, name);
                weapon.AddOperators(MakeWeaponOperators(namePrefix + "Operator", weapon.RequiredOperatorCount));

                unitList.Add(weapon);
            }
            return unitList;
        }

        //public static BattleTeam MakeBattleTeam(
        //    BattleManager battleManager,
        //    string country,
        //    string federation,
        //    string teamName,
        //    int swordManCount,
        //    int bowManCount,
        //    int medicalManCount,
        //    int trebuchetCount,
        //    int scoutCount)
        //{
        //    BattleTeam team = new BattleTeam(NewId(), teamName, country, federation, battleManager);
        //    team.AddBattleUnitRange(MakeSwordMen("SwordMan", swordManCount));
        //    team.AddBattleUnitRange(MakeBowMen("BowMan", bowManCount));
        //    team.AddBattleUnitRange(MakeMedicalMen("MedicalMan", medicalManCount));
        //    team.AddBattleUnitRange(MakeTrebuchets("Trebuchet", trebuchetCount));
        //    team.AddBattleUnitRange(MakeScouts("Scout", scoutCount));

        //    return team;
        //}

        public static void MakeArmy(
            BattleTeam team,
            int swordManCount,
            int bowManCount,
            int medicalManCount,
            int trebuchetCount,
            int scoutCount)
        {         
            team.AddBattleUnitRange(MakeSwordMen("SwordMan", swordManCount));
            team.AddBattleUnitRange(MakeBowMen("BowMan", bowManCount));
            team.AddBattleUnitRange(MakeMedicalMen("MedicalMan", medicalManCount));
            team.AddBattleUnitRange(MakeTrebuchets("Trebuchet", trebuchetCount));
            team.AddBattleUnitRange(MakeScouts("Scout", scoutCount));
        }
    }
}
