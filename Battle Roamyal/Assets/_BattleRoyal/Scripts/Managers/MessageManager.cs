// // --
// // Author: Josh van den Heever
// // Date: 1/08/2018 @ 8:51 p.m.
// // --
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MessageManager : SingletonMonobehaviour<MessageManager>
{
    public Transform messageUIPrefab;

    private ChatUIScript chatUIScript;

    public class MessageData
    {
        public string message;
        public Color color;

        public float remainingTime;

        public MessageData(string message, Color color, float duration)
        {
            this.message = message;
            this.color = color;

            remainingTime = duration;
        }
    }

    private List<MessageData> messages = new List<MessageData>();

    protected override void Awake()
    {
        base.Awake();
        Transform chatInstance = Instantiate(messageUIPrefab, Vector3.zero, Quaternion.identity, transform);
        chatUIScript = chatInstance.GetComponent<ChatUIScript>();
    }

    public void AddMessage(string message, Color color, float duration = 5f)
    {
        AddMessage(new MessageData(message, color, duration));
    }

    public void AddMessage(MessageData data)
    {
        messages.Add(data);
        chatUIScript.SetMessages(messages);
    }

    private void FixedUpdate()
    {
        //Don't do this all the time but update accordingly
        ClearStaleMessages();
    }

    private void ClearStaleMessages()
    {
        MessageData[] staleMessages = messages.Where((arg) => {
            arg.remainingTime -= Time.fixedDeltaTime;
            return arg.remainingTime <= 0f;
        }).ToArray();

        foreach (var msg in staleMessages) messages.Remove(msg);

        chatUIScript.SetMessages(messages);
    }

    public static void ServerMessage(string message, float duration = 5f)
    {
        AddMsg(message, Color.blue, duration);
    }

    public static void AddMsg(string message, Color color, float duration = 5f)
    {
        Instance.AddMessage(message, color, duration);
    }
}
