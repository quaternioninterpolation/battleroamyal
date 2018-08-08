// // --
// // Author: Josh van den Heever
// // Date: 1/08/2018 @ 7:57 p.m.
// // --
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

    public override void SV_OnPlayerConnected(BoltConnection player)
    {
        base.SV_OnPlayerConnected(player);

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

    public override void SV_OnPlayerDisconnected(BoltConnection player)
    {
        base.SV_OnPlayerDisconnected(player);

        //Destroy their player
        BoltEntity[] controlledEntities = BoltNetwork.entities.Where((ent) => ent.IsController(player)).ToArray();
        //Destroy them all
        foreach (var entity in controlledEntities)
        {
            BoltNetwork.Destroy(entity.gameObject);
        }
    }
}
