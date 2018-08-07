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

[BoltGlobalBehaviour(BoltNetworkModes.Server)]
public class SVBoltGlobalCallbacks : Bolt.GlobalEventListener
{
    public override void BoltStartDone()
    {
        base.BoltStartDone();
    }

    public override void ConnectAttempt(UdpEndPoint endpoint, IProtocolToken token)
    {
        base.ConnectAttempt(endpoint, token);
        BoltNetwork.Accept(endpoint);
    }

    public override void Connected(BoltConnection connection)
    {
        base.Connected(connection);

        Debug.Log("Player connected. Connection: " + connection);
        GameModeBase.GetActiveGameMode().OnPlayerConnected(connection);
    }

    public override void Disconnected(BoltConnection connection)
    {
        base.Disconnected(connection);

        Debug.Log("Player connected. Connection: " + connection);
        GameModeBase.GetActiveGameMode().OnPlayerDisconnected(connection);
    }
}
