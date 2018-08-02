// // --
// // Author: Josh van den Heever
// // Date: 1/08/2018 @ 4:38 p.m.
// // --
using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;

public class EditorMenuTools
{
    [MenuItem("Battle Roamyal/Launch Game")]
    public static void LaunchGame()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/Watermark/Scenes/"+LaunchScript.LAUNCH_SCENE_NAME+".unity");
            EditorApplication.isPlaying = true;
        }
    }

    [MenuItem("Battle Roamyal/Clear Player Prefs")]
    public static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    [MenuItem("Battle Roamyal/Print Player Save Data")]
    public static void PrintPlayerSaveData()
    {
        string saveJson = UserManager.GetPlayerSaveDataString();
        Debug.Log("PRINTING SAVE DATA:\n" + saveJson);
    }

    [MenuItem("Battle Roamyal/Print Player Save Data <IN MEMORY>")]
    public static void PrintPlayerSaveDataInMemory()
    {
        if (UserManager.Instance == null)
        {
            Debug.LogError("Game not running, cannot print save data in memory");
            return;
        }

        string saveJson = UserManager.Instance.currentPlayer.Serialize();
        Debug.Log("PRINTING SAVE DATA IN MEMORY:\n" + saveJson);
    }

    [MenuItem("Battle Roamyal/GoTo Launch Scene", priority = 20)]
    public static void GoToLaunchScene()
    {
        LoadScene(LaunchScript.LAUNCH_SCENE_NAME);
    }

    [MenuItem("Battle Roamyal/GoTo Menu Scene", priority = 21)]
    public static void GoToMenuScene()
    {
        LoadScene(BRConfig.SceneNames.mainMenu);
    }

    [MenuItem("Battle Roamyal/GoTo Battle Scene", priority = 22)]
    public static void GoToBattleScene()
    {
        LoadScene(BRConfig.SceneNames.battleMode);
    }

    public static void LoadScene(string sceneName)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/Watermark/Scenes/"+sceneName+".unity");
        }
    }
}
