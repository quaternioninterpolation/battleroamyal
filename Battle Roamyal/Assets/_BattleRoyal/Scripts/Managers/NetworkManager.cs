// // --
// // Author: Josh van den Heever
// // Date: 1/08/2018 @ 8:19 p.m.
// // --
using UnityEngine;
using System.Collections;

public class NetworkManager : SingletonMonobehaviour<NetworkManager>
{
    private string currentLevel;

    protected override void Awake()
    {
        base.Awake();
        BoltLauncher.SetUdpPlatform(new PhotonPlatform());
    }

    public void StartServer(string levelName)
    {
        //TODO: Ensure we're not already in a game or hosting
        BoltLauncher.StartServer(UdpKit.UdpEndPoint.Any);
        SceneManager.Instance.GoToScene(levelName, true, OnServerLevelLoaded);
        currentLevel = levelName;
    }

    public void StopServer(string menuScene)
    {
        Debug.Log("Stopping server!");
        BoltLauncher.Shutdown();
        SceneManager.Instance.GoToScene(menuScene, true, null);
    }

    private void OnServerLevelLoaded()
    {
        Debug.Log("Level loaded! currentLevel: "+currentLevel);
    }
}
