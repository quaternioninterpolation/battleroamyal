// // --
// // Author: Josh van den Heever
// // Date: 1/08/2018 @ 4:21 p.m.
// // --
using UnityEngine;
using System.Collections;

public class BRConfig : ConfigBase
{
    public static class SceneNames
    {
        public static string mainMenu = "MainMenuScene";
        public static string battleMode = "BattleModeScene";
    }

    public static class PrefabNames
    {
    }

    public static string[] SceneNamesArray = typeof(SceneNames).GetListOfStaticPublicMemberValues<string>();
    public static string[] PrefabNamesArray = typeof(PrefabNames).GetListOfStaticPublicMemberValues<string>();

    public override string[] GetRequiredPrefabs()
    {
        return PrefabNamesArray;
    }

    public override string[] GetScenes()
    {
        return SceneNamesArray;
    }

    public override PlayerSaveData CreateSaveData()
    {
        return new BRPlayerSaveData();
    }
}
