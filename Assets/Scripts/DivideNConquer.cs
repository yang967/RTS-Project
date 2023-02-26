using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DivideNConquer
{
    public static void Insert(List<Spot> list, Spot toInsert, int start, int end)
    {
        if (list.Count == 0)
            list.Add(toInsert);
        else if(start < end)
        {
            int middle = start + (end - start) / 2;

            if(middle < list.Count - 1 && middle > 0)
            {
                if (CompareVector(toInsert,list[middle]) > 0 && CompareVector(toInsert, list[middle + 1]) < 0)
                    list.Insert(middle + 1, toInsert);
                else if (CompareVector(toInsert, list[middle]) < 0 && CompareVector(toInsert, list[middle - 1]) > 0)
                    list.Insert(middle, toInsert);
                else if (CompareVector(toInsert, list[middle]) > 0)
                    Insert(list, toInsert, middle + 1, end);
                else if (CompareVector(toInsert, list[middle]) < 0)
                    Insert(list, toInsert, start, middle);
                else
                    list.Insert(middle, toInsert);
            }
            else if(middle == list.Count - 1)
            {
                if (CompareVector(toInsert, list[middle]) == 0)
                    list.Add(toInsert);
                else if (CompareVector(toInsert, list[middle]) < 0 && CompareVector(toInsert, list[middle - 1]) > 0)
                    list.Insert(middle - 1, toInsert);
                else
                    Insert(list, toInsert, start, middle);
            }
            else
            {
                if (CompareVector(toInsert, list[middle]) == 0 || CompareVector(toInsert, list[middle]) < 0)
                    list.Insert(0, toInsert);
                else if (CompareVector(toInsert, list[middle]) > 0 && CompareVector(toInsert, list[middle + 1]) < 0)
                    list.Insert(middle + 1, toInsert);
                else
                    Insert(list, toInsert, middle + 1, end);
            }
        }
        else
        {
            if (CompareVector(list[end], toInsert) > 0)
                list.Insert(end, toInsert);
            else
                list.Insert(end + 1, toInsert);
        }
    }

    public static int CompareVector(Spot a, Spot b)
    {
        Vector3 aCurrent = new Vector3((int)a.current.x, (int)a.current.y, (int)a.current.z);
        Vector3 bCurrent = new Vector3((int)b.current.x, (int)b.current.y, (int)b.current.z);
        if (aCurrent.x == bCurrent.x && aCurrent.y == bCurrent.y && aCurrent.z == bCurrent.z)
            return 0;
        if (aCurrent.x < bCurrent.x ||
            (aCurrent.x == bCurrent.x && aCurrent.y < bCurrent.y) ||
            (aCurrent.x == bCurrent.x && aCurrent.y == bCurrent.y && aCurrent.z < bCurrent.z))
            return -1;
        else
            return 1;
    }

    public static int CompareVector(Vector3 a, Vector3 b)
    {
        Vector3 aCurrent = new Vector3((int)a.x, (int)a.y, (int)a.z);
        Vector3 bCurrent = new Vector3((int)b.x, (int)b.y, (int)b.z);
        if (aCurrent.x == bCurrent.x && aCurrent.y == bCurrent.y && aCurrent.z == bCurrent.z)
            return 0;
        if (aCurrent.x < bCurrent.x ||
            (aCurrent.x == bCurrent.x && aCurrent.y < bCurrent.y) ||
            (aCurrent.x == bCurrent.x && aCurrent.y == bCurrent.y && aCurrent.z < bCurrent.z))
            return -1;
        else
            return 1;
    }

    public static int Find(List<Spot> list, Spot toFind, int start, int end)
    {
        if (start < end)
        {
            int middle = start + (end - start) / 2;
            if (CompareVector(toFind, list[middle]) == 0)
                return middle;
            else if (CompareVector(toFind, list[middle]) > 0)
                return Find(list, toFind, middle + 1, end);
            else
                return Find(list, toFind, start, middle);
        }
        else if (CompareVector(toFind, list[end]) == 0)
            return end;
        else
            return -1;
    }

    public static int Find(List<Spot> list, Vector3 toFind, int start, int end)
    {
        if (list.Count == 0)
            return -1;
        else if (start < end)
        {
            int middle = start + (end - start) / 2;
            if (CompareVector(toFind, list[middle].current) == 0)
                return middle;
            else if (CompareVector(toFind, list[middle].current) > 0)
                return Find(list, toFind, middle + 1, end);
            else
                return Find(list, toFind, start, middle);
        }
        else if (end < list.Count && CompareVector(toFind, list[end].current) == 0)
            return end;
        else
            return -1;
    }
}
