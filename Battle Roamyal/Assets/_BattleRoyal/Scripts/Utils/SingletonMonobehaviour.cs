// // --
// // Author: Josh van den Heever
// // Date: 1/08/2018 @ 4:10 p.m.
// // --
using UnityEngine;
using System.Collections;

public class SingletonMonobehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T s_instance = null;

    public static T Instance
    {
        get
        {
            return s_instance;
        }
    }


    protected virtual void Awake()
    {
        s_instance = GetComponent<T>();
    }

    protected virtual void OnDestroy()
    {
        s_instance = null;
    }
}
