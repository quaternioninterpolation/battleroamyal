// // --
// // Author: Josh van den Heever
// // Date: 1/08/2018 @ 6:45 p.m.
// // --
using UnityEngine;
using System.Collections;

public class BRPlayerSaveData : PlayerSaveData
{
    public IntSaveProperty totalKillCount = new IntSaveProperty("totalKillCount");

    public BRPlayerSaveData()
    {
        InitSaveProperties(new SavePropertyBase[]{
            totalKillCount
        });
    }

    public override bool IsReturningUser()
    {
        return false;
    }
}
