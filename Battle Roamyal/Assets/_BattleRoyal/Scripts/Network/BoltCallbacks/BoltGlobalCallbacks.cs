// // --
// // Author: Josh van den Heever
// // Date: 1/08/2018 @ 7:27 p.m.
// // --
using UnityEngine;
using System.Collections;
using Bolt;
using UdpKit;

[BoltGlobalBehaviour]
public class BoltGlobalCallbacks : Bolt.GlobalEventListener
{
    public override void BoltStartDone()
    {
        base.BoltStartDone();
        BoltNetwork.RegisterTokenClass<ConnectionAcceptToken>();
        BoltNetwork.RegisterTokenClass<ConnectionRequestToken>();
    }

    public override void Connected(BoltConnection connection)
    {
        base.Connected(connection);

        if (BoltNetwork.isServer)
        {
            //Connected
            MessageManager.ServerMessage("Player connected. ID: "+connection.ConnectionId);
        }
    }
}