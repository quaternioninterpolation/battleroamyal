// // --
// // Author: Josh van den Heever
// // Date: 1/08/2018 @ 4:26 p.m.
// // --
using UnityEngine;
using System.Collections;

public class SceneNamePopup : EasyEditor.PopupAttribute
{
    public SceneNamePopup()
        : base(BRConfig.SceneNamesArray)
    {
    }
}
