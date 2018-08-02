//
// Copyright (c) 2016 Easy Editor 
// All Rights Reserved 
//  
//

using UnityEngine;
using UnityEditor;
using UEObject = UnityEngine.Object;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using EasyEditor.ReorderableList;

namespace EasyEditor
{
    /// <summary>
    /// Render a list in the inspector. The list UI allows to change element position, or remove them from the list in a very easy way.
    /// Adding the attribute [Selectable] to the list allows to select item to render the details of their field below the list.
    /// </summary>
    [RenderType(typeof(IList))]
    public class ReorderableListRenderer : FieldRenderer
    {
        public Action<int, SerializedProperty> OnItemInserted, OnItemBeingRemoved;

        public delegate void ItemMovedDelegate(int oldIndex, int newIndex, SerializedProperty list);
        public event ItemMovedDelegate OnItemMoved;

        public Action<Rect , int > OnDrawItem,OnDrawItemBackground; 

        private SerializedProperty list;
        private ReorderableListControl listControl;
        private EESerializedPropertyAdaptor listAdaptor;
       
        bool isReadOnly = false;
        bool isSelectable = false;

        public override void CreateAsset(string path)
        {
            Utils.CreateAssetFrom<ReorderableListRenderer>(this, "List_" + _label, path);
        }

        public override void InitializeFromEntityInfo(EntityInfo entityInfo)
        {
            base.InitializeFromEntityInfo(entityInfo);

            isReadOnly = (AttributeHelper.GetAttribute<ReadOnlyAttribute>(entityInfo.fieldInfo) != null);
            isSelectable = (AttributeHelper.GetAttribute<SelectableAttribute>(entityInfo.fieldInfo) != null);

            list = FieldInfoHelper.GetSerializedPropertyFromPath(entityInfo.propertyPath, entityInfo.serializedObject);
            listControl = new ReorderableListControl();

            listControl.ItemInserted += ListControl_OnItemInsertedHandler;
            listControl.ItemRemoving += ListControl_OnItemRemovingHandler;
            listControl.ItemMoved += ListControl_OnItemMovedHandler;

            if(isReadOnly)
            {
                listControl.Flags = ReorderableListFlags.DisableReordering 
                        | ReorderableListFlags.DisableContextMenu 
                        | ReorderableListFlags.HideAddButton
                        | ReorderableListFlags.HideRemoveButtons;
            }

			listAdaptor = new EESerializedPropertyAdaptor(serializedProperty, isReadOnly);
            listAdaptor.OnItemSelected += ListAdaptor_HandleOnItemSelected;
            listAdaptor.OnItemInserted += ListAdaptor_HandleOnItemInserted;
            listAdaptor.OnDrawItem += ListAdaptor_HandleOnDrawItem;
            listAdaptor.OnDrawItemBackground += ListAdaptor_HandleOnDrawItemBackground;
        }

        private void ListControl_OnItemInsertedHandler(object sender, ItemInsertedEventArgs args) {
            if(OnItemInserted != null)
            {
                OnItemInserted(args.ItemIndex, list);
            }
        }
        
        private void ListControl_OnItemRemovingHandler(object sender, ItemRemovingEventArgs args) {
            if(OnItemBeingRemoved != null)
            {
                OnItemBeingRemoved(args.ItemIndex, list);
            }
        }

        private void ListControl_OnItemMovedHandler(object sender, ItemMovedEventArgs args) {
            if(OnItemMoved != null)
            {
                OnItemMoved(args.OldItemIndex, args.NewItemIndex, list);
            }
        }
            
        private void ListAdaptor_HandleOnDrawItem(Rect rect, int index)
        {
            if(OnDrawItem!=null)
            {
                OnDrawItem(rect,index);
            }
        }

        private void ListAdaptor_HandleOnDrawItemBackground(Rect rect, int index)
        {
            if(OnDrawItemBackground!=null)
            {
                OnDrawItemBackground(rect,index);
            }
        }

        private void ListAdaptor_HandleOnItemInserted(int index, SerializedProperty list) {
            if(OnItemMoved != null)
            {
                OnItemInserted(index, list);
            }
        }

        InlineClassRenderer inlineClassRenderer;
        private void ListAdaptor_HandleOnItemSelected(int index, SerializedProperty list)
        {
            if(isSelectable)
            {
                inlineClassRenderer = null;

                SerializedProperty propertyToRender = list.GetArrayElementAtIndex(index);
                if(propertyToRender.propertyType == SerializedPropertyType.ObjectReference || propertyToRender.propertyType == SerializedPropertyType.Generic)
                {
                    object listElement = FieldInfoHelper.GetObjectFromPath(propertyToRender.propertyPath, serializedObject.targetObject);

                    if(listElement != null)
                    {
                        EntityInfo info = new EntityInfo(listElement.GetType(), 
                                                     serializedObject, propertyToRender.propertyPath);
                        inlineClassRenderer = InspectorItemRenderer.CreateRenderer<InlineClassRenderer>();
                        inlineClassRenderer.InitializeFromEntityInfo(info);

                        if(propertyToRender.propertyType == SerializedPropertyType.Generic)
                        {
                            inlineClassRenderer.FoldoutTitle = propertyToRender.displayName;
                        }
                    }
                }
            }
        }

        public override void Render(Action preRender = null)
        {
            base.Render(preRender);

            directParentSerializedObject.Update();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUI.indentLevel * 10f * Settings.indentation);

            if(inlineClassRenderer != null)
            {
                EditorGUILayout.BeginVertical(InspectorStyle.DefaultStyle.inlineFoldableBackgroundStyle);
            }
            else
            {
                EditorGUILayout.BeginVertical();
            }

            ReorderableListGUI.Title(_label);

            listControl.Draw(listAdaptor);

            if(inlineClassRenderer != null)
            {
                inlineClassRenderer.Render(preRender);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            directParentSerializedObject.ApplyModifiedProperties();
        }
    }
}