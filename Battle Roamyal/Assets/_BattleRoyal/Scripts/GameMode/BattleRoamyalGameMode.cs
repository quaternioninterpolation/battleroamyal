// // --
// // Author: Josh van den Heever
// // Date: 1/08/2018 @ 7:57 p.m.
// // --
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleRoamyalGameMode : GameModeBase
{
    public class BRPlayerData
    {
        public BoltConnection connection;
        public string name;
        public BoltEntity player;
    }

    public Dictionary<BoltConnection, BRPlayerData> playerData = new Dictionary<BoltConnection, BRPlayerData>();
    public Bolt.PrefabId playerPrefab;
    public SpawnPosition[] spawnPositions;

    protected override void Awake()
    {
        base.Awake();

        spawnPositions = GetComponentsInChildren<SpawnPosition>();
    }

    public override void OnPlayerConnected(BoltConnection player)
    {
        base.OnPlayerConnected(player);

        if (BoltNetwork.isServer)
        {
            //Create new player
            SVSpawnPlayer(player);
        }
    }

    protected virtual void SVSpawnPlayer(BoltConnection connection)
    {
        //Create a new player
        var spawn = spawnPositions.GetRandom();
        BoltEntity playerEnt = BoltNetwork.Instantiate(playerPrefab, spawn.transform.position, Quaternion.identity);
        playerEnt.AssignControl(connection);
        PlayerController player = playerEnt.GetComponent<PlayerController>();
    }

    public override void OnPlayerDisconnected(BoltConnection player)
    {
        base.OnPlayerDisconnected(player);
    }
}
