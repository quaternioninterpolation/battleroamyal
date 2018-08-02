// // --
// // Author: Josh van den Heever
// // Date: 1/08/2018 @ 6:28 p.m.
// // --
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    public interface Callback
    {
        void MainMenuJoinClicked();
        void MainMenuHostClicked();
        void MainMenuOptionsClicked();
        void MainMenuExitClicked();
    }

    public Button hostBtn;
    public Button joinBtn;
    public Button optionsBtn;
    public Button exitBtn;

    private Callback callback;

    private void Awake()
    {
        
    }

    public void SetCallback(Callback callback)
    {
        this.callback = callback;
    }
}
