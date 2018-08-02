// // --
// // Author: Josh van den Heever
// // Date: 1/08/2018 @ 6:28 p.m.
// // --
using UnityEngine;
using System.Collections;

public class MenuScene : BRScene, MainMenuScript.Callback, JoinMenuScript.Callback, HostMenuScript.Callback
{
    public MainMenuScript mainMenuScript;
    public JoinMenuScript joinMenu;
    public HostMenuScript hostMenu;

    private void Awake()
    {
        mainMenuScript.SetCallback(this);
        joinMenu.SetCallback(this);
        hostMenu.SetCallback(this);

        SetScreen(mainMenuScript.transform);
    }

    private void SetScreen(Transform screen)
    {
        mainMenuScript.gameObject.SetActive(screen == mainMenuScript);
        joinMenu.gameObject.SetActive(screen == joinMenu);
        hostMenu.gameObject.SetActive(screen == hostMenu);
    }

    public void MainMenuExitClicked()
    {
        //Leave the game
        Application.Quit();
    }

    public void MainMenuHostClicked()
    {
        SetScreen(hostMenu.transform);
    }

    public void MainMenuJoinClicked()
    {
        SetScreen(joinMenu.transform);
    }

    public void MainMenuOptionsClicked()
    {
        //TODO:
        Debug.LogError("No options menu implemented, but theres a button callback? what you doing...");
    }

    public void JoinClicked(string ipAddress)
    {
        //Host a new game
        NetworkManager.Instance.StartServer(BRConfig.SceneNames.battleMode);
    }

    public void JoinCanceled()
    {
        SetScreen(mainMenuScript.transform);
    }

    public void OnHostClicked()
    {
        //Host a new game
        NetworkManager.Instance.StartServer(BRConfig.SceneNames.battleMode);
    }

    public void OnHostCancelled()
    {
        SetScreen(mainMenuScript.transform);
    }
}
