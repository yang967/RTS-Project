using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class MapHeap
{
    List<int> list;
    //int[] fscore;

    public MapHeap()
    {
        //this.fscore = fscore;
        list = new List<int>();
    }

    void swap(int i, int j)
    {
        int temp = list[i];
        list[i] = list[j];
        list[j] = temp;
    }

    int HasChild(int i)
    {
        int result = 0;
        if (list.Count > i * 2 + 1)
            result++;
        if (list.Count > i * 2 + 2)
            result++;
        return result;
    }

    public int Count()
    {
        return list.Count;
    }

    public void push(int i, NativeArray<int> fscore)
    {
        list.Add(i);
        int current = list.Count - 1;
        int next;
        while(current > 0)
        {
            if (current % 2 == 0)
                next = (current - 1) / 2;
            else
                next = current / 2;
            if (fscore[list[current]] < fscore[list[next]])
            {
                swap(current, next);
                current = next;
            }
            else
                break;
        }
    }

    public int pop(NativeArray<int> fscore)
    {
        int result = list[0];
        swap(0, list.Count - 1);
        list.RemoveAt(list.Count - 1);
        int current = 0;
        int left, right;
        int h = HasChild(current);
        bool side;
        while (h != 0)
        {
            left = current * 2 + 1;
            right = current * 2 + 2;
            side = h == 1 || fscore[list[left]] < fscore[list[right]];
            if(side)
            {
                if (fscore[list[current]] > fscore[list[left]])
                {
                    swap(current, left);
                    current = left;
                }
                else
                    break;
            }
            else
            {
                if (fscore[list[current]] > fscore[list[right]])
                {
                    swap(current, right);
                    current = right;
                }
                else
                    break;
            }
            h = HasChild(current);
        }
        return result;
    }

    public void PrintList(int[] fscore)
    {
        string o = "";
        for (int i = 0; i < list.Count; i++)
            o += fscore[list[i]] + " ";
        Debug.Log(o);
    }
}
