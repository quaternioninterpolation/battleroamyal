// // --
// // Author: Josh van den Heever
// // Date: 1/08/2018 @ 9:52 p.m.
// // --
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using static MessageManager;
using System.Text;

public class ChatUIScript : MonoBehaviour
{
    public int maxMessagesInList = 10;
    public Text chatMessage;

    public void SetMessages(List<MessageData> messages)
    {
        RecalculateText(messages);
    }

    public void RecalculateText(List<MessageData> messages)
    {
        StringBuilder textBuilder = new StringBuilder();
        for (int i = 0; i < messages.Count; ++i)
        {
            MessageData msg = messages[0];

            if (i < messages.Count) textBuilder.AppendLine();
            textBuilder.Append("<color=");
            ColorUtility.ToHtmlStringRGB(msg.color);
            textBuilder.Append(">");
            textBuilder.Append(msg.message);
            textBuilder.Append("</color>");
        }
    }
}
