using UnityEngine;
using System.Collections;
using UdpKit;
using Bolt;

//[BoltGlobalBehaviour]
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
        GameModeBase.GetActiveGameMode().SV_OnPlayerConnected(connection);
    }

    public override void Disconnected(BoltConnection connection)
    {
        base.Disconnected(connection);

        Debug.Log("Player connected. Connection: " + connection);
        GameModeBase.GetActiveGameMode().SV_OnPlayerDisconnected(connection);
    }
}
