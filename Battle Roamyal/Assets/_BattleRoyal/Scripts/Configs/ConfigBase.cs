// // --
// // Author: Josh van den Heever
// // Date: 1/08/2018 @ 4:21 p.m.
// // --
using UnityEngine;
using System.Collections;

public abstract class ConfigBase : MonoBehaviour
{
    public abstract string[] GetScenes();
    public abstract string[] GetRequiredPrefabs();
    public abstract PlayerSaveData CreateSaveData();

    public static ConfigBase fromGameObject(GameObject gameObject)
    {
        return gameObject.GetComponent<ConfigBase>();
    }
}
