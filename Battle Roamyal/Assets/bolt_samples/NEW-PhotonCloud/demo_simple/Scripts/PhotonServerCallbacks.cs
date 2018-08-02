using Bolt;
using System.Collections;
using System.Collections.Generic;
using UdpKit;
using UnityEngine;

namespace Photon
{
    [BoltGlobalBehaviour("Level1")]
    public class PhotonServerCallbacks : Bolt.GlobalEventListener
    {
        public override void Connected(BoltConnection connection)
        {
            BoltLog.Warn("Connected");

            ServerAcceptToken acceptToken = connection.AcceptToken as ServerAcceptToken;

            if (acceptToken != null)
            {
                BoltConsole.Write("AcceptToken: " + acceptToken.GetType().ToString());
                BoltConsole.Write("AcceptToken: " + acceptToken.data);
            }
            else
            {
                BoltLog.Warn("AcceptToken is NULL");
            }

            ServerConnectToken connectToken = connection.ConnectToken as ServerConnectToken;

            if (connectToken != null)
            {
                BoltConsole.Write("ConnectToken: " + connectToken.GetType().ToString());
                BoltConsole.Write("ConnectToken: " + connectToken.data);
            }
            else
            {
                BoltLog.Warn("ConnectToken is NULL");
            }
        }

        public override void ConnectAttempt(UdpEndPoint endpoint, IProtocolToken token)
        {
            BoltLog.Warn("Connect Attempt");
            base.ConnectAttempt(endpoint, token);
        }

        public override void ConnectFailed(UdpEndPoint endpoint, IProtocolToken token)
        {
            BoltLog.Warn("Connect Failed");
            base.ConnectFailed(endpoint, token);
        }

        public override void ConnectRefused(UdpEndPoint endpoint, IProtocolToken token)
        {
            BoltLog.Warn("Connect Refused");
            base.ConnectRefused(endpoint, token);
        }

        public override void ConnectRequest(UdpEndPoint endpoint, IProtocolToken token)
        {
            BoltLog.Warn("Connect Request");

            //token should be ServerConnectToken
            if (token != null)
            {
                BoltLog.Warn(token.GetType().ToString());

                ServerConnectToken t = token as ServerConnectToken;
                BoltLog.Warn("Server Token: null? " + (t == null));
                BoltLog.Warn("Data: " + t.data);

            } else
            {
                BoltLog.Warn("Received token is null");
            }

            ServerAcceptToken acceptToken = new ServerAcceptToken
            {
                data = "Accepted"
            };

            BoltNetwork.Accept(endpoint, acceptToken);
        }

        public override void Disconnected(BoltConnection connection)
        {
            BoltLog.Warn("Disconnected");
            base.Disconnected(connection);
        }
    }
}
