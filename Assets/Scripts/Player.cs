using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player
{
    int PlayerID;
    int group;

    public Player(int playerID, int group)
    {
        PlayerID = playerID;
        this.group = group;
    }
    public Player(Player player)
    {
        if(player == null)
        {
            PlayerID = -1;
            group = -1;
        }
        else
        {
            PlayerID = player.getPlayerID();
            group = player.getGroup();
        }
    }
    public int getPlayerID()
    {
        return PlayerID;
    }

    public int getGroup()
    {
        return group;
    }
}
