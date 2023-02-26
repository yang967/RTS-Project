using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Building
{
    string name;
    int BuildingType;
    int width;
    int length;
    string[] production;

    public Building(string name, int Type, int length, int width) 
    {
        this.name = name;
        BuildingType = Type;
        this.length = length;
        this.width = width;
        production = new string[0];
    }

    public Building(string name, int Type, int length, int width, string[] production)
    {
        this.name = name;
        BuildingType = Type;
        this.length = length;
        this.width = width;
        this.production = new string[production.Length];
        production.CopyTo(this.production, 0);
    }

    public string getName()
    {
        return name;
    }

    public int getBuildingType()
    {
        return BuildingType;
    }

    public int getLength()
    {
        return length;
    }

    public int getWidth()
    {
        return width;
    }

    public string[] getProduction()
    {
        return production;
    }
}
