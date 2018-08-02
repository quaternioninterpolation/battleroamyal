//
// Copyright (c) 2016 Easy Editor 
// All Rights Reserved 
//  
//

using UnityEngine;
using UEObject = UnityEngine.Object;
using UnityEditor;
using System;
using System.Collections;
using System.Reflection;

namespace EasyEditor
{
    /// <summary>
    /// Render a button in the inspector. This button trigger a method that belongs to the editor target object.
    /// </summary>
	public class ButtonRenderer : InspectorItemRenderer {

        public string _label = "";
		public string tooltip = "";

        public override void CreateAsset(string path)
        {
            Utils.CreateAssetFrom<ButtonRenderer>(this, "Button_" + _label, path);
        }

		public override void InitializeFromEntityInfo (EntityInfo entityInfo)
		{
			base.InitializeFromEntityInfo (entityInfo);

			EETooltipAttribute tooltipAttribute = AttributeHelper.GetAttribute<EETooltipAttribute> (entityInfo.methodInfo);
			if (tooltipAttribute != null && !string.IsNullOrEmpty(tooltipAttribute.tooltip)) 
			{
				tooltip = tooltipAttribute.tooltip;
			}

			this._label = string.IsNullOrEmpty(_inspectorAttribute.label) ? 
                ObjectNames.NicifyVariableName(this.entityInfo.methodInfo.Name) :
                _inspectorAttribute.label;
		}

        public override void Render(Action preRender = null)
        {
            base.Render(preRender);

            Rect position = EditorGUILayout.GetControlRect(false);
            position.x += EditorGUI.indentLevel * 15f;
            position.width -= EditorGUI.indentLevel * 15f;
            if (GUI.Button(position, new GUIContent(_label, tooltip)))
            {
                if(typeof(Editor).IsAssignableFrom(entityInfo.caller.GetType()))
                {
                    entityInfo.methodInfo.Invoke(entityInfo.caller, null);
                }
                else
                {
                    if(typeof(MonoBehaviour).IsAssignableFrom(entityInfo.caller.GetType()) || 
                       typeof(ScriptableObject).IsAssignableFrom(entityInfo.caller.GetType()))
                    {
                        for(int i = 0; i < serializedObject.targetObjects.Length; i++)
                        {
                            entityInfo.methodInfo.Invoke(serializedObject.targetObjects[i], null);
                        }
                    }
                    else
                    {
                        for(int i = 0; i < entityInfo.callers.Length; i++)
                        {
                            entityInfo.methodInfo.Invoke(entityInfo.callers[i], null);
                        }
                    }
                }
            }
        }
	}
}