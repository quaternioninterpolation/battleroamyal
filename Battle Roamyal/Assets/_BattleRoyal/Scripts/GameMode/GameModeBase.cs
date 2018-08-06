// // --
// // Author: Josh van den Heever
// // Date: 1/08/2018 @ 4:47 p.m.
// // --
using UnityEngine;
using System.Collections;

public class GameModeBase : Bolt.EntityEventListener<IGameModeState>
{
    private static GameModeBase activeGameMode = null;

    public static T GetActiveGameMode<T>() where T : GameModeBase
    {
        return activeGameMode as T;
    }

    public static GameModeBase GetActiveGameMode()
    {
        return activeGameMode;
    }

    protected virtual void OnEnable()
    {
        activeGameMode = this;
    }

    protected virtual void OnDisable()
    {
        if (activeGameMode == this)
        {
            activeGameMode = null;
        }
    }

    protected virtual void Awake()
    {
        //Stub
    }

    public virtual void OnPlayerConnected(BoltConnection player)
    {
        MessageManager.AddMsg("Player connected " + player, Color.green);
    }


    public virtual void OnPlayerDisconnected(BoltConnection player)
    {
        //Stub
        MessageManager.AddMsg("Player disconnected " + player, Color.green);
    }
}
