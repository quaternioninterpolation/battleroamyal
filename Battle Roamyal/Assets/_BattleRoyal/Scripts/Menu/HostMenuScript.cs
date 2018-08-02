// // --
// // Author: Josh van den Heever
// // Date: 1/08/2018 @ 8:42 p.m.
// // --
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HostMenuScript : MonoBehaviour
{
    public interface Callback
    {
        void OnHostClicked();
        void OnHostCancelled();
    }

    public Button hostBtn;
    public Button backBtn;

    private Callback callback;

    private void Awake()
    {
        hostBtn.onClick.AddListener(() => callback.OnHostClicked());
        backBtn.onClick.AddListener(() => callback.OnHostCancelled());
    }

    public void SetCallback(Callback callback)
    {
        this.callback = callback;
    }
}
