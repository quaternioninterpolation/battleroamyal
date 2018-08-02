// // --
// // Author: Josh van den Heever
// // Date: 1/08/2018 @ 4:31 p.m.
// // --
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LaunchScript : MonoBehaviour
{
    public static string LAUNCH_SCENE_NAME = "_LaunchScene";

    [EasyEditor.ReadOnly]
    public string launchSceneName = LAUNCH_SCENE_NAME;
    public Transform configPrefab;

    private void Awake()
    {
        if (configPrefab == null)
        {
            Debug.LogError("Cannot start game! give launch object a configPrefab");
            return;
        }

        //Load our scene!
        Transform configInstance = Instantiate(configPrefab, Vector3.zero, Quaternion.identity);
        configInstance.name = "Game Config Core";
    }
}
