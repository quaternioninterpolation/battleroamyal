// // --
// // Author: Josh van den Heever
// // Date: 1/08/2018 @ 7:33 p.m.
// // --
using UnityEngine;
using System.Collections;
using UdpKit;

public class ConnectionAcceptToken : Bolt.IProtocolToken
{
    public string levelName;
    public int playersInGame;
    public string messageOfTheDay;

    public void Read(UdpPacket packet)
    {
        levelName = packet.ReadString();
        playersInGame = packet.ReadInt();
        messageOfTheDay = packet.ReadString();
    }

    public void Write(UdpPacket packet)
    {
        packet.WriteString(levelName);
        packet.WriteInt(playersInGame);
        packet.WriteString(messageOfTheDay);
    }
}
