// // --
// // Author: Josh van den Heever
// // Date: 1/08/2018 @ 4:40 p.m.
// // --
using UnityEngine;
using System.Collections;

public class UserManager : SingletonMonobehaviour<UserManager>
{
    private static string USER_DATA_PREFS_NAME = "PlayerData";

    protected PlayerData playerData;

    public PlayerData currentPlayer
    {
        get
        {
            if (playerData == null)
            {
                LoadOrCreatePlayerData();
            }

            return playerData;
        }
    }

    private void LoadOrCreatePlayerData()
    {
        playerData = new PlayerData();
        playerData.saveData = ConfigBase.fromGameObject(gameObject).CreateSaveData();

        if (PlayerPrefs.HasKey(USER_DATA_PREFS_NAME))
        {
            playerData.ParseJSON(PlayerPrefs.GetString(USER_DATA_PREFS_NAME));
        }
    }

    public void DestroyPlayerData()
    {
        playerData = null;
        PlayerPrefs.DeleteKey(USER_DATA_PREFS_NAME);
        PlayerPrefs.Save();
    }

    public static string GetPlayerSaveDataString()
    {
        return PlayerPrefs.GetString(USER_DATA_PREFS_NAME);
    }
}
