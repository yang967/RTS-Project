using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarInfo : MonoBehaviour
{
    [SerializeField] string[] keys;
    [SerializeField] int[] values;

    Dictionary<string, int> dict;

    // Start is called before the first frame update
    public void Set()
    {
        if (keys.Length != values.Length)
            Debug.LogError("Error! Key and value amounts are unmatched!");
        dict = new Dictionary<string, int>();
        for (int i = 0; i < keys.Length; i++)
            dict.Add(keys[i], values[i]);
    }

    public int getIndex(string key)
    {
        return dict[key];
    }
}
