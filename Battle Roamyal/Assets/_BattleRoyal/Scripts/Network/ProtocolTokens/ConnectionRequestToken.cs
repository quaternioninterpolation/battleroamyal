// // --
// // Author: Josh van den Heever
// // Date: 1/08/2018 @ 7:30 p.m.
// // --
using UnityEngine;
using System.Collections;
using UdpKit;

public class ConnectionRequestToken : Bolt.IProtocolToken
{
    public string playerName;
    public Color colorPreference;

    public void Read(UdpPacket packet)
    {
        playerName = packet.ReadString();
        colorPreference = packet.ReadColorRGB();
    }

    public void Write(UdpPacket packet)
    {
        packet.WriteString(playerName);
        packet.WriteColorRGB(colorPreference);
    }
}
