//#define TEST_FAIL

using UnityEngine;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using System;
using ExitGames.Client.Photon.LoadBalancing;
using UdpKit;
using System.Collections;
using Bolt;
using System.Net;
using udpkit.plataform.photon.puncher;

public sealed class PhotonCloudRoomProperties : IProtocolToken
{
    ExitGames.Client.Photon.Hashtable _CustomRoomProperties;
    HashSet<string> _CustomRoomPropertiesInLobby;

    public bool IsVisible = true;
    public bool IsOpen = true;

    public PhotonCloudRoomProperties()
    {
        _CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
        _CustomRoomPropertiesInLobby = new HashSet<string>();
    }

    public ExitGames.Client.Photon.Hashtable CustomRoomProperties
    {
        get { return _CustomRoomProperties; }
    }

    public HashSet<string> CustomRoomPropertiesInLobby
    {
        get { return _CustomRoomPropertiesInLobby; }
    }

    public bool AddRoomProperty(string key, object value, bool showInRoom = true)
    {
        if (!IsValid(value)) { return false; }

        _CustomRoomProperties[key] = value;

        if (showInRoom)
        {
            _CustomRoomPropertiesInLobby.Add(key);
        } else
        {
            _CustomRoomPropertiesInLobby.Remove(key);
        }

        return true;
    }

    public bool RemoveRoomProperty(string key)
    {
        return _CustomRoomProperties.Remove(key) && _CustomRoomPropertiesInLobby.Remove(key);
    }

    public void Read(UdpPacket packet) {}
    public void Write(UdpPacket packet) {}

    public bool IsValid(object value)
    {
        return value is sbyte || value is byte
            || value is short || value is ushort
            || value is int   || value is uint
            || value is long  || value is ulong
            || value is float || value is double
            || value is decimal;
    }
}

public class PhotonPoller : Bolt.GlobalEventListener
{
    public enum ConnectState
    {
        Idle = 0,
        JoinRoomPending = 1,
        DirectPending = 2,
        DirectFailed = 3,
        DirectSuccess = 4,
        RelayPending = 5,
        RelayFailed = 6,
        RelaySuccess = 7
    }

    public class PhotonSession : UdpSession
    {
        internal Guid _id;
        internal Int32 _playerCount;
        internal Int32 _playerLimit;
        internal String _roomName;
        internal Byte[] _hostData;
        internal ExitGames.Client.Photon.Hashtable _customProperties;

        public override Int32 ConnectionsCurrent
        {
            get { return _playerCount; }
        }

        public override Int32 ConnectionsMax
        {
            get { return _playerLimit; }
        }

        public override Boolean HasLan
        {
            get { return false; }
        }

        public override Boolean HasWan
        {
            get { return true; }
        }

        public override String HostName
        {
            get { return _roomName; }
        }

        public override Guid Id
        {
            get { return _id; }
        }

        public override Boolean IsDedicatedServer
        {
            get { return false; }
        }

        public override UdpEndPoint LanEndPoint
        {
            get { return default(UdpEndPoint); }
        }

        public override UdpSessionSource Source
        {
            get { return UdpSessionSource.Photon; }
        }

        public override UdpEndPoint WanEndPoint
        {
            get { return default(UdpEndPoint); }
        }

        public override Byte[] HostData
        {
            get { return _hostData; }
        }

        public override System.Object HostObject
        {
            get;
            set;
        }

        public override UdpSession Clone()
        {
            return (UdpSession)MemberwiseClone();
        }

        public ExitGames.Client.Photon.Hashtable Properties
        {
            get { return _customProperties; }
        }
    }

    class PhotonLoadBalancingClient : LoadBalancingClient, IPunchLoadBalancingClient
    {
        private RaiseEventOptions cachedOptions = new RaiseEventOptions();

        public PhotonLoadBalancingClient()
        {
            this.OnEventAction += HandlerEvent;
        }

        public void CleanUp()
        {
            this.OnEventAction -= HandlerEvent;
        }

        public void CallOpRaiseEvent(int targetPlayer, byte messageCode, string message)
        {
            if (cachedOptions.TargetActors == null)
            {
                cachedOptions.TargetActors = new int[1];
            }

            cachedOptions.TargetActors[0] = targetPlayer;

            this.OpRaiseEvent(messageCode, message, true, cachedOptions);
        }

        private void HandlerEvent(EventData evt)
        {
            int otherIDJoin = 0;
            string info = null;

            if (evt.Parameters.ContainsKey(ParameterCode.ActorNr))
            {
                otherIDJoin = (int)evt.Parameters[ParameterCode.ActorNr];
            }

            if (evt.Parameters.ContainsKey(ParameterCode.Data))
            {
                info = evt.Parameters[ParameterCode.Data].ToString();
            }

            PunchAPI.HandlerPhotonEvent(evt.Code, otherIDJoin, info);
        }

        public override void DebugReturn(DebugLevel level, string message)
        {
            if (level == DebugLevel.ERROR)
            {
                Debug.LogError(message);
            }
            else if (level == DebugLevel.WARNING)
            {
                Debug.LogWarning(message);
            }
            else if (level == DebugLevel.INFO)
            {
                Debug.Log(message);
            }
            else if (level == DebugLevel.ALL)
            {
                Debug.Log(message);
            }
        }

        public int LocalPlayerID()
        {
            return this.LocalPlayer.ID;
        }

        public bool LocalPlayerIsMasterClient()
        {
            return this.LocalPlayer.IsMasterClient;
        }

        public void OnPunchSuccess(int remotePlayerID, UdpEndPoint remoteEndPoint)
        {
            UdpLog.Info("[PUNCH SUCCESS] Local Player {0} with Remote Player {1}", LocalPlayerID(), remotePlayerID);
            if (!this.LocalPlayer.IsMasterClient)
            {
                BoltNetwork.Connect(remoteEndPoint, Instance.joinToken);
            }

            Instance.joinToken = null;
        }

        public void OnPunchFailed(int remotePlayerID)
        {
            UdpLog.Info("[PUNCH FAILED] Local Player {0} with Remote Player {1}", LocalPlayerID(), remotePlayerID);
            if (!this.LocalPlayer.IsMasterClient)
            {
                Instance.ChangeState(ConnectState.DirectFailed);    
            }
        }

    }

    class PhotonPacket
    {
        public Byte[] Data;
        public Int32 Remote;

        public PhotonPacket()
        {

        }

        public PhotonPacket(Int32 size)
        {
            Data = new byte[size];
        }
    }

    class SynchronizedQueue<T>
    {
        Queue<T> queue = new Queue<T>();

        public void Clear()
        {
            lock (queue)
            {
                queue.Clear();
            }
        }

        public Int32 Count
        {
            get
            {
                lock (queue)
                {
                    return queue.Count;
                }
            }
        }

        public void Enqueue(T item)
        {
            lock (queue)
            {
                queue.Enqueue(item);
            }
        }

        public bool TryDequeue(out T item)
        {
            lock (queue)
            {
                if (queue.Count > 0)
                {
                    item = queue.Dequeue();
                    return true;
                }

                item = default(T);
                return false;
            }
        }
    }

    static PhotonPoller _instance;

    public static PhotonPoller Instance
    {
        get
        {
            return _instance;
        }
    }

    public static void CreatePoller(PhotonPlatformConfig config)
    {
        if (!_instance)
        {
            var pollers = FindObjectsOfType<PhotonPoller>();
            if (pollers.Length == 0)
            {
                _instance = new GameObject(typeof(PhotonPoller).Name).AddComponent<PhotonPoller>();
            }

            if (pollers.Length == 1)
            {
                _instance = pollers[0];
            }

            if (pollers.Length >= 2)
            {
                _instance = pollers[0];

                for (int i = 1; i < pollers.Length; ++i)
                {
                    Destroy(pollers[i].gameObject);
                }
            }

            _instance._config = config;

            DontDestroyOnLoad(_instance);
        }
    }

    const Byte DATA_EVENT_CODE = 1;
    const Single ROOM_UPDATE_RATE = 5f;
    const Single ROOM_CREATE_TIMEOUT = 10f;
    const Single ROOM_JOIN_TIMEOUT = 10f;

    Timer _roomUpdateTimer;
    ClientState _state;
    public ConnectState _connectState;
#pragma warning disable 414
    Coroutine _currentConnectRoutine;
#pragma warning restore 414
    PhotonPlatformConfig _config;
    PhotonLoadBalancingClient _lbClient;

    IProtocolToken joinToken;

    SynchronizedQueue<PhotonPacket> _packetPool = new SynchronizedQueue<PhotonPacket>();
    SynchronizedQueue<PhotonPacket> _packetSend = new SynchronizedQueue<PhotonPacket>();
    SynchronizedQueue<PhotonPacket> _packetRecv = new SynchronizedQueue<PhotonPacket>();

    public LoadBalancingClient LoadBalancerClient
    {
        get { return _lbClient; }
    }

    public Int32 HostPlayerId
    {
        get
        {
            if (_lbClient == null)
            {
                return -1;
            }

            return _lbClient.CurrentRoom.MasterClientId;
        }
    }

    void Disconnect()
    {
        if (_lbClient != null)
        {
            _lbClient.Disconnect();
            _lbClient = null;
        }
    }

    void OnDestroy()
    {
        Disconnect();
    }

    protected new void OnDisable()
    {
        base.OnDisable();
        Disconnect();
    }

    public override void BoltStartDone()
    {
        base.BoltStartDone();

        Disconnect();

        _lbClient = new PhotonLoadBalancingClient();
        _lbClient.OnEventAction += OnEventAction;
        _lbClient.OnOpResponseAction += OnOpResponseAction;
        _lbClient.OnStateChangeAction += OnStateChangeAction;

        _lbClient.AutoJoinLobby = true;

        _lbClient.AppId = _config.AppId;
        if (_config.UseOnPremise)
        {
            _lbClient.Connect(_config.OnPremiseServerIpAddress, _config.AppId, "1.0", "", null);
        }
        else
        {
            _lbClient.ConnectToRegionMaster(_config.RegionMaster);
        }

        PunchAPI.RegisterPhotonClient(_lbClient, BoltNetwork.UdpSocket.LanEndPoint);
    }


    void Update()
    {
        if (_lbClient == null)
        {
            return;
        }

        // clear send/recv pools when getting connected
        if (_lbClient.State == ClientState.Joined && _lbClient.State != _state)
        {
            _packetSend.Clear();
            _packetRecv.Clear();
        }

        if (_lbClient.State == ClientState.JoinedLobby)
        {
            if (_roomUpdateTimer.Expired)
            {
                // update
                BoltNetwork.UpdateSessionList(FetchSessionListFromPhoton());

                // 
                _roomUpdateTimer = new Timer(ROOM_UPDATE_RATE);
            }
        }

        // poll in/out
        PollIn();
        PollOut();

        // store state
        _state = _lbClient.State;

        PunchAPI.Service();
    }

    Map<Guid, UdpSession> FetchSessionListFromPhoton()
    {
        var map = new Map<Guid, UdpSession>();

        foreach (var r in LoadBalancerClient.RoomInfoList)
        {
            if (r.Value.IsOpen && r.Value.IsVisible)
            {
                try
                {
                    if (!r.Value.CustomProperties.ContainsKey("UdpSessionId"))
                    {
                        continue;
                    }

                    PhotonSession session = new PhotonSession
                    {
                        _roomName = r.Key,
                        _id = new Guid((r.Value.CustomProperties["UdpSessionId"] as String) ?? ""),
                        _hostData = r.Value.CustomProperties["UserToken"] as Byte[],
                        _playerCount = r.Value.PlayerCount,
                        _playerLimit = r.Value.MaxPlayers,
                        _customProperties = r.Value.CustomProperties
                    };

                    map = map.Add(session.Id, session);
                }
                catch (Exception exn)
                {
                    BoltLog.Exception(exn);
                }
            }
        }

        return map;
    }

    public override void ConnectFailed(UdpEndPoint endpoint, IProtocolToken token)
    {
        if (_connectState == ConnectState.DirectPending)
        {
            ChangeState(ConnectState.DirectFailed);
        }

        if (_connectState == ConnectState.RelayPending)
        {
            ChangeState(ConnectState.RelayFailed);
        }
    }

    public override void Connected(BoltConnection connection)
    {
        if (_connectState == ConnectState.DirectPending)
        {
            ChangeState(ConnectState.DirectSuccess);
        }

        if (_connectState == ConnectState.RelayPending)
        {
            ChangeState(ConnectState.RelaySuccess);
        }
    }

    public override void BoltShutdownBegin(AddCallback registerDoneCallback)
    {
        base.BoltShutdownBegin(registerDoneCallback);
        Destroy(PhotonPoller.Instance.gameObject);
    }

    void OnStateChangeAction(ClientState obj)
    {
    }

    void OnOpResponseAction(OperationResponse obj)
    {
    }

    void OnEventAction(EventData obj)
    {

        switch (obj.Code)
        {
            // AppStats
            case 226:
                break;

            // GameList
            case 230:
                break;

            case 254:
                if (BoltNetwork.server != null)
                {
                    if ((int)obj.Parameters[ParameterCode.ActorNr] == 1)
                    {
                        BoltNetwork.server.Disconnect();
                    }
                }
                break;

            case DATA_EVENT_CODE:
                var packetPlayerId = (int)obj.Parameters[ParameterCode.ActorNr];
                var packetContents = (byte[])obj.Parameters[ParameterCode.CustomEventContent];

                _packetRecv.Enqueue(new PhotonPacket
                {
                    Data = packetContents,
                    Remote = packetPlayerId
                });
                break;

                //default:
                //  Debug.LogErrorFormat("Unknown event code {0}", obj.Code);
                //  break;

        }
    }

    void PollIn()
    {
        Boolean success;

        do
        {
            success = _lbClient.loadBalancingPeer.DispatchIncomingCommands();
        } while (success);
    }

    void PollOut()
    {
        PhotonPacket packet;

        while (_packetSend.TryDequeue(out packet))
        {
            _lbClient.loadBalancingPeer.OpRaiseEvent(DATA_EVENT_CODE, packet.Data, false, new RaiseEventOptions
            {
                CachingOption = EventCaching.DoNotCache,
                SequenceChannel = 0,
                TargetActors = new int[1] { packet.Remote }
            });
        }

        _lbClient.loadBalancingPeer.SendOutgoingCommands();
    }


    Byte[] CloneArray(Byte[] array, Int32 size)
    {
        var clone = new Byte[size];
        Buffer.BlockCopy(array, 0, clone, 0, size);
        return clone;
    }

    static public void UpdateHostInfo(System.Object protocolToken)
    {
        Instance.StartCoroutine(Instance.SetHostInfoRoutine(protocolToken: protocolToken, create: false));
    }

    static public void SetHostInfo(String servername, Boolean dedicated, System.Object protocolToken)
    {
        Instance.StartCoroutine(Instance.SetHostInfoRoutine(protocolToken, servername, dedicated));
    }

    IEnumerator SetHostInfoRoutine(System.Object protocolToken, String servername = null, Boolean dedicated = false, bool create = true)
    {
        var t = new Timer(ROOM_CREATE_TIMEOUT);

        while (_lbClient == null || _lbClient.State != ClientState.JoinedLobby && t.Waiting)
        {
            yield return null;
        }

        if (create)
        {
            if (_lbClient == null || _lbClient.State != ClientState.JoinedLobby)
            {
                BoltLog.Error("Can't call BoltNetwork.SetHostInfo when not in lobby");
                yield break;
            }
        }
        else
        {
            if (_lbClient == null || _lbClient.State != ClientState.Joined)
            {
                BoltLog.Error("Can't call BoltNetwork.SetHostInfo while not in a room");
                yield break;
            }
        }

        var maxPlayers = dedicated ? BoltNetwork.maxConnections : BoltNetwork.maxConnections + 1;
        var customRoomProperties = default(ExitGames.Client.Photon.Hashtable);

        RoomOptions roomOptions = new RoomOptions();

        // properties for lobby
        List<string> publicPropertyListName = new List<string>(new string[] {
            "UdpSessionId", "UserToken"
        });

        // check for new interface based version
        var boltPhotonCloudRoomProperties = protocolToken as PhotonCloudRoomProperties;
        if (boltPhotonCloudRoomProperties != null)
        {
            customRoomProperties = boltPhotonCloudRoomProperties.CustomRoomProperties;
            publicPropertyListName.AddRange(boltPhotonCloudRoomProperties.CustomRoomPropertiesInLobby);

            roomOptions.IsOpen = boltPhotonCloudRoomProperties.IsOpen;
            roomOptions.IsVisible = boltPhotonCloudRoomProperties.IsVisible;
        }

        // last resort, create a new empty tble
        if (customRoomProperties == null)
        {
            customRoomProperties = new ExitGames.Client.Photon.Hashtable();
        }

        // if we have a protocol token, package it into the room properties as Byte[]
        if (protocolToken != null && protocolToken is IProtocolToken)
        {
            customRoomProperties["UserToken"] = ProtocolTokenUtils.ToByteArray((IProtocolToken)protocolToken);
        }

        // session id
        if (create)
        {
            customRoomProperties["UdpSessionId"] = Guid.NewGuid().ToString();
        }

        // Setup Room Options
        roomOptions.CustomRoomProperties = customRoomProperties;
        roomOptions.CustomRoomPropertiesForLobby = publicPropertyListName.ToArray();

        if (create)
        {
            roomOptions.MaxPlayers = (byte) maxPlayers;
        }

        if (create)
        {
            // create the room with all settings
            _lbClient.OpCreateRoom(servername, roomOptions, null);
        }
        else
        {
            // update the room with all settings
            _lbClient.OpSetCustomPropertiesOfRoom(customRoomProperties);
            BoltLog.Info("Updating room properties");
        }
    }

    static public Boolean JoinSession(UdpSession session, System.Object token)
    {
        if (session.Source == UdpSessionSource.Photon)
        {
            if (Instance._connectState != ConnectState.Idle)
            {
                BoltLog.Error("Already attempting connection to a photon room");
                return true;
            }

            if (Instance._lbClient.State != ClientState.JoinedLobby)
            {
                BoltLog.Error("Can't call BoltNetwork.Connect when not in lobby");
                return true;
            }

            Instance._currentConnectRoutine = Instance.StartCoroutine(Instance.JoinSessionRoutine(session, token));
            return true;
        }
        else
        {
            return false;
        }
    }

    IEnumerator JoinSessionRoutine(UdpSession session, System.Object token)
    {
        Timer timer;

        ChangeState(ConnectState.JoinRoomPending);

        if (token is IProtocolToken)
        {
            joinToken = (IProtocolToken)token;
        }

        timer = new Timer(ROOM_JOIN_TIMEOUT);

        while (_lbClient == null && timer.Waiting)
        {
            yield return null;
        }

        _lbClient.OpJoinRoom(session.HostName);

        timer = new Timer(ROOM_JOIN_TIMEOUT);

        while (_lbClient != null && _lbClient.State != ClientState.Joined && timer.Waiting)
        {
            yield return null;
        }

        if (_lbClient == null || _lbClient.State != ClientState.Joined)
        {
            _currentConnectRoutine = null;
            BoltLog.Error("Failed to join room");

            ChangeState(ConnectState.Idle);
            yield break;
        }

        BoltLog.Info("UsePunch: " + _config.UsePunchThrough);

        if (_config.UsePunchThrough) {
            ChangeState(ConnectState.DirectPending);

            while (_connectState == ConnectState.DirectPending)
            {
                yield return null;
            }

            if (_connectState == ConnectState.DirectSuccess)
            {
                ChangeState(ConnectState.Idle);
                yield break;
            }

        }

        //BoltNetwork.Connect(new UdpEndPoint(new UdpIPv4Address((uint)_lbClient.CurrentRoom.MasterClientId), 0), ProtocolTokenUtils.ToToken((byte[])_lbClient.CurrentRoom.CustomProperties["UserToken"]));
        BoltNetwork.Connect(new UdpEndPoint(new UdpIPv4Address((uint)_lbClient.CurrentRoom.MasterClientId), 0), joinToken);
        joinToken = null;

        ChangeState(ConnectState.RelayPending);

        while (_connectState == ConnectState.RelayPending)
        {
            yield return null;
        }

        if (_connectState == ConnectState.RelaySuccess)
        {
            ChangeState(ConnectState.Idle);
        }
    }

    PhotonPacket AllocPacket(Int32 size)
    {
        PhotonPacket packet;

        if (_packetPool.TryDequeue(out packet))
        {
            Array.Resize(ref packet.Data, size);
            return packet;
        }
        else
        {
            return new PhotonPacket(size);
        }
    }

    void ChangeState(ConnectState state)
    {
        Debug.Log(string.Format("Changing Connect State: {0} => {1}", _connectState, state));

        // update
        _connectState = state;
    }

    void FreePacket(PhotonPacket packet)
    {
        _packetPool.Enqueue(packet);
    }

    struct Timer
    {
        Single _expire;

        public Timer(Single wait)
        {
            _expire = Time.realtimeSinceStartup + wait;
        }

        public Boolean Expired
        {
            get
            {
                return Time.realtimeSinceStartup >= _expire;
            }
        }

        public Boolean Waiting
        {
            get
            {
                return Time.realtimeSinceStartup < _expire;
            }
        }
    }

    static public Int32 RecvFrom(Byte[] buffer, Int32 bufferSize, ref UdpEndPoint endpoint)
    {
        PhotonPacket packet;

        if (Instance._packetRecv.TryDequeue(out packet))
        {
            // copy data
            Buffer.BlockCopy(packet.Data, 0, buffer, 0, packet.Data.Length);

            // set "sender"
            endpoint = new UdpEndPoint(new UdpIPv4Address((uint)packet.Remote), 0);

            return packet.Data.Length;
        }

        return -1;
    }

    static public Boolean RecvPoll()
    {
        return Instance._packetRecv.Count > 0;
    }

    static public Int32 SendTo(Byte[] buffer, Int32 bytesToSend, UdpEndPoint endpoint)
    {
        PhotonPacket packet;
        packet = Instance.AllocPacket(bytesToSend);
        packet.Remote = (int)endpoint.Address.Packed;

        Buffer.BlockCopy(buffer, 0, packet.Data, 0, bytesToSend);

        Instance._packetSend.Enqueue(packet);

        return bytesToSend;
    }

}