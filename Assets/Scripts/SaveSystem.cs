using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;

public static class SaveSystem
{
    public static void SaveUnit(List<Unit> Units)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Units.rts";
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, Units);
        stream.Close();
    }

    public static List<Unit> LoadUnit()
    {
        string path = Application.persistentDataPath + "/Units.rts";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            List<Unit> result = formatter.Deserialize(stream) as List<Unit>;
            stream.Close();
            return result;
        }
        else
        {
            Debug.Log("Error! In Game Unit File NOT FOUND!");
            return null;
        }
    }

    public static void SavePlayerList(List<Player> list)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Players.rts";
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, list);
        stream.Close();
    }

    public static List<Player> LoadPlayerList()
    {
        string path = Application.persistentDataPath + "/Players.rts";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            List<Player> result = formatter.Deserialize(stream) as List<Player>;
            stream.Close();
            return result;
        }
        else
        {
            Debug.Log("Error! Player List File NOT FOUND!");
            return null;
        }
    }

    public static void EncryptEquipment(List<List<Equipment>>[] list)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = "./Assets/Files/Equipments.rts";
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, list);
        stream.Close();
    }

    public static List<List<Equipment>>[] DecryptEquipment()
    {
        string path = "./Assets/Files/Equipments.rts";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            List<List<Equipment>>[] list = formatter.Deserialize(stream) as List<List<Equipment>>[];
            stream.Close();
            return list;
        }
        else
        {
            Debug.Log("Equipment file NOT FOUND!");
            return null;
        }
    }

    public static void EncryptUnit(Unit[] units)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = "./Assets/Files/Units.rts";
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, units);
        stream.Close();
    }

    public static Unit[] DecryptUnit()
    {
        string path = "./Assets/Files/Units.rts";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            Unit[] units = formatter.Deserialize(stream) as Unit[];
            stream.Close();
            return units;
        }
        else
        {
            Debug.Log("Units file NOT FOUND!");
            return null;
        }
    }

    public static void EncryptDictionary(Dictionary<string, int> dict, string name)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = "./Assets/Files/" + name;
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, dict);
        stream.Close();
    }

    public static Dictionary<string, int> DecryptDictionary(string name)
    {
        string path = "./Assets/Files/" + name;
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            Dictionary<string, int> dict = formatter.Deserialize(stream) as Dictionary<string, int>;
            stream.Close();
            return dict;
        }
        else
        {
            Debug.Log(name + " NOT FOUND!");
            return null;
        }
    }

    public static void EncryptEffect(List<Effect> effects)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = "./Assets/Files/Effects.rts";
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, effects);
        stream.Close();
    }

    public static List<Effect> DecryptEffect()
    {
        string path = "./Assets/Files/Effects.rts";
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            List<Effect> effects = formatter.Deserialize(stream) as List<Effect>;
            stream.Close();
            return effects;
        }
        else
        {
            Debug.Log("Effect File NOT FOUND");
            return null;
        }
    }

    public static void EncryptConstructions(List<Building>[] construction)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = "./Assets/Files/Constructions.rts";
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, construction);
        stream.Close();
    }

    public static List<Building>[] DecryptConstructions()
    {
        string path = "./Assets/Files/Constructions.rts";
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            List<Building>[] constructions = formatter.Deserialize(stream) as List<Building>[];
            stream.Close();
            return constructions;
        }
        else
        {
            Debug.Log("CONSTRUCTION FILE not FOUND!");
            return null;
        }
    }
}
