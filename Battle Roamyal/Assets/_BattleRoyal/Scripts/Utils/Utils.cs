// // --
// // Author: Josh van den Heever
// // Date: 1/08/2018 @ 4:22 p.m.
// // --
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public static class Utils
{
    public static bool IsPointerOverUI(EventSystem ev)
    {
        bool result = false;

        if (ev != null)
        {
            result |= ev.IsPointerOverGameObject();
            
            for (int i = 0; i < Input.touchCount; ++i)
            {
                var touch = Input.touches[i];
                result |= ev.IsPointerOverGameObject(touch.fingerId);
            }
        }

        return result;
    }
}
