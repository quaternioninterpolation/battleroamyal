// // --
// // Author: Josh van den Heever
// // Date: 1/08/2018 @ 6:37 p.m.
// // --
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class JoinMenuScript : MonoBehaviour
{
	public interface Callback
    {
        void JoinClicked(string ipAddress);
        void JoinCanceled();
    }

    public InputField ipInput;
    public Button joinBtn;
    public Button backBtn;

    private Callback callback;

    private void Awake()
    {
        //joinBtn.onClick.AddListener(
    }

    private void OnJoinClicked()
    {
        string ip = ipInput.text.Trim();
        //TODO: Validate
        callback?.JoinClicked(ip);
    }

    public void SetCallback(Callback callback)
    {
        this.callback = callback;
    }
}
