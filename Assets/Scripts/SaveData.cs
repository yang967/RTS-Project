using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Dictionary<string, int> dictionary = new Dictionary<string, int>();
        dictionary.Add("APC", 0);
        dictionary.Add("ArmedHelicopter", 1);
        dictionary.Add("Drone", 2);
        dictionary.Add("BattleBot", 3);
        dictionary.Add("BombardHelicopter", 4);
        dictionary.Add("BattlePlatform", 5);
        dictionary.Add("AntiAirCanon", 6);
        dictionary.Add("CloneProducer", 7);
        dictionary.Add("Assaulter", 8);
        dictionary.Add("LeviTank", 9);
        SaveSystem.EncryptDictionary(dictionary, "UnitDict.rts");
        Unit[] AllUnits = new Unit[10];
        AllUnits[0] = new Unit(400, 0, -1, 10, 10, 0, 6, 0, true, false, true, "APC", -1, 15, 200, 0, 0, 30, 0, 0, 0, 0, 12, new int[] { 0, 0, 0, 0 }, 2.5f, null, 1, false, 300, 30, null);
        AllUnits[1] = new Unit("ArmedHelicopter", 200, 10, 3, 20f, 0, 20, 1, 21, 20, 200, 15, null);
        //AllUnits[1] = new Unit(200, 0, -1, 10, 3, 0, false, "ArmedHelicopter", -1, 20f, 100f, 0, 0, 20, 0, 0, 0, 0, 21, new int[] { 0, 0, 0, 0 }, 20, null, 1, false, 200, 15, null);
        AllUnits[2] = new Unit("Drone", 80, 5, 1, 25, 0, 100, 1, 21, 20, 75, 10, null);
        //AllUnits[2] = new Unit(80, 0, -1, 5, 1, 0, false, "Drone", -1, 25, 100, 0, 30, 100, 0, 0, 0, 0, 21, new int[] { 0, 0, 0, 0 }, 20, null, 1, false, 75, 10, null);
        AllUnits[3] = new Unit(100, 0, -1, 2, 2, 0, false, "BattleBot", -1, 10, 100, 0, 0, 30, 0, 0, 0, 0, 1, new int[] { 0, 0, 0, 0 }, 1.35f, null, 1, false, 75, 10, null);
        AllUnits[4] = new Unit(500, 0, -1, 10, 6, 0, false, "BombardHelicopter", -1, 10, 100, 0, 0, 0, 0, 0, 0, 0, 22, new int[] { 0, 0, 0, 0 }, 20, null, 1, false, 500, 45, null);
        AllUnits[5] = new Unit(1000, 0, -1, 50, 20, 0, 10, 2, true, true, false, "BattlePlatform", -1, 5, 100, 0, 60, 80, 0, 0, 0, 0, 22, new int[] { 0, 0, 0, 0 }, 20, null, 0, false, 2000, 60, null);
        AllUnits[6] = new Unit(400, 0, -1, 10, 4, 5, false, "AntiAirCanon", -1, 0, 100, 3, 50, 50, 0.2f, 1, 1, 1, 31, new int[] { -1, -1, 1, -1 }, 1.3f, null, 0, true, 150, 18, null);
        AllUnits[7] = new Unit(800, 0, -1, 50, 2, 0, false, "CloneProducer", -1, 0, 0, 0, 0, 50, 0, 0, 0, 0, 31, new int[] { -1, -1, -1, -1 }, 0.5f, null, 0, true, 200, 30, null);
        AllUnits[8] = new Unit(150, 0, -1, 8, 2, 0, false, "Assaulter", -1, 20f, 100, 0, 0, 50, 0, 0, 0, 0, 21, new int[] { -1, -1, -1, -1 }, 20, null, 1, false, 150, 20, null);
        AllUnits[9] = new Unit("LeviTank", 250, 7, 3, 20, 0, 30, 1, 11, 2, 120, 10, null);
        SaveSystem.EncryptUnit(AllUnits);
        Dictionary<string, int> eDictionary = new Dictionary<string, int>();
        eDictionary.Add("APCStandardHull", 0);
        eDictionary.Add("APCAVCanon", 1);
        eDictionary.Add("APCHighCapacityEngine", 2);
        eDictionary.Add("LightAutoCanon", 200);
        eDictionary.Add("PulseRifle", 300);
        eDictionary.Add("AutoShotgun", 301);
        eDictionary.Add("PlasmaCannon", 302);
        eDictionary.Add("HelicopterChainGun", 100);
        eDictionary.Add("HelicopterRocketLauncher", 101);
        eDictionary.Add("HellfireLauncher", 400);
        eDictionary.Add("DevastatorRocketLauncher", 401);
        eDictionary.Add("RipperGun", 800);
        eDictionary.Add("LeviAssaultTurret", 900);
        SaveSystem.EncryptDictionary(eDictionary, "EquipmentsDict.rts");
        List<List<Equipment>>[] AllEquipments = new List<List<Equipment>>[4];
        AllEquipments = new List<List<Equipment>>[10];
        for (int i = 0; i < AllEquipments.Length; i++)
            AllEquipments[i] = new List<List<Equipment>>();
        AllEquipments[0].Add(new List<Equipment>());
        AllEquipments[0][0].Add(new Equipment("APCStandardHull"));
        AllEquipments[0][0].Add(new Equipment("APCAVCanon", 40, 5, 2, 1, 20, 0, 0, 0, "APC Anti-Vehicle Canon", "APC", 0, new int[] { 0, 1, -1, 0 }, 1, 1, new AbilityData[0]));
        AllEquipments[0][0].Add(new Equipment("APCHighCapacityEngine", 0, 0, 0, 0, 0, 0, 7, 0, "APC High Capacity Engine", "APC", 0, new int[] { -1, -1, -1, -1 }, 0, 0, new AbilityData[0]));
        AllEquipments[1].Add(new List<Equipment>());
        AllEquipments[1][0].Add(new Equipment("HelicopterChainGun", 10, 3, 0.2f, 0.8f, 50f, 0, 0, 0, "Helicopter Chain Gun", "ArmedHelicopter", 0, new int[] { 3, 0, -1, 0 }, 1, 1, new AbilityData[0]));
        AllEquipments[1][0].Add(new Equipment("HelicopterRocketLauncher", 40, 7, 1.5f, 1, 50, 0, 0, 0, "Helicopter Rocket Launcher", "ArmedHelicopter", 0, new int[] { 0, 3, -1, 0 }, 1, 1, new AbilityData[0]));
        AllEquipments[2].Add(new List<Equipment>());
        //AllEquipments[2][0].Add(new Equipment());
        AllEquipments[2][0].Add(new Equipment("LightAutoCanon", 15, 1, 0.5f, 0.8f, 30, 0, 0, 0, "Light Auto Canon", "Drone", 0, new int[] { 1, 0, -1, 0 }, 1, 1, new AbilityData[0]));
        AllEquipments[3].Add(new List<Equipment>());
        AllEquipments[3][0].Add(new Equipment("PulseRifle", 3, 2, 1, 0.98f, 30, 0, 0, 0, "Pulse Rifle", "BattleBot", 0, new int[] { 1, 0, 0, 0 }, 1, 5, new AbilityData[0]));
        AllEquipments[3][0].Add(new Equipment("AutoShotgun", 1, 2, 0.5f, 0.7f, 20, 0, 0, 0, "Auto Shotgun", "BattleBot", 0, new int[] { 1, 0, 0, 0 }, 3, 1, new AbilityData[0]));
        AllEquipments[3][0].Add(new Equipment("PlasmaCannon", 20, 5, 2, 0.95f, 40, 0, 0, 0, "Plasma Cannon", "BattleBot", 0, new int[] { 2, 2, 0, 0 }, 1, 1, new AbilityData[0]));
        AllEquipments[4].Add(new List<Equipment>());
        AllEquipments[4][0].Add(new Equipment("HellfireLauncher", 0, 0, 0, 0, 30, 0, 0, 0, "Hellfire Rocket Launcher", "BombardHelicopter", 0, new int[] { -1, -1, -1, -1 }, 0, 0, new AbilityData[] {
            new AbilityData("AreaBombard", "HellFire", 15, 20, 0, 50, 7, 2, 20, new int[] { 0, 3, -1, 0 }, new Effect(0, "Flame", 5, 5, new int[] { 0, 2, -1, 0 }, 10, "FlameField")),
            new AbilityData("AreaBombard", "Wither", 30, 20, 0, 0, 0, "BioMissle", 3, 15, new int[] { -1, -1, -1, -1 }, new Effect(0, "Bio Contamination", 10, 10, new int[] { 3, 3, -1, 0 }, 20,  "BioField"))
        }));
        AllEquipments[4][0].Add(new Equipment("DevastatorRocketLauncher", 50, 7, 5, 1, 30, 0, 0, 0, "Devastator Rocket Launcher", "BombardHelicopter", 0, new int[] { 0, 0, -1, 3 }, 1, 2, new AbilityData[0]));
        AllEquipments[8].Add(new List<Equipment>());
        AllEquipments[8][0].Add(new Equipment("RipperGun", 2, 2, 0.05f, 0.5f, 30, 0, 0, 0, "Ripper Gun", "Assaulter", 0, new int[] { 1, 0, 0, 1 }, 1, 1, new AbilityData[0]));
        AllEquipments[9].Add(new List<Equipment>());
        AllEquipments[9][0].Add(new Equipment("LeviAssaultTurret", 10, 2, 0.5f, 80f, 30, 0, 0, 0, "Levitational Tank Assault Turret", "LeviTank", 0, new int[] { 1, 1, -1, 0 }, 1, 1, new AbilityData[0]));
        SaveSystem.EncryptEquipment(AllEquipments);
        List<Building>[] AllConstructions = new List<Building>[2];
        AllConstructions[0] = new List<Building>();
        AllConstructions[0].Add(new Building("AntiAirCanon", 0, 1, 1));
        AllConstructions[0].Add(new Building("CloneProducer", 1, 3, 2, new string[] { "BattleBot" }));
        SaveSystem.EncryptConstructions(AllConstructions);
        Dictionary<string, int> cDictionary = new Dictionary<string, int>();
        cDictionary.Add("AntiAirCanon", 0);
        cDictionary.Add("CloneProducer", 1);
        SaveSystem.EncryptDictionary(cDictionary, "ConstructionDict.rts");
    }
}
