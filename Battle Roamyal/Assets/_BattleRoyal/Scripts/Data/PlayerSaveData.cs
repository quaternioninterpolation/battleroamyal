using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

[System.Serializable]
public abstract class PlayerSaveData
{
    public delegate void DataUpdatedListener(PlayerSaveData data, SavePropertyBase changedProperty);
    protected event DataUpdatedListener EventUpdatedListeners;
    protected SavePropertyBase[] saveProperties;

    private bool init = false;

    public void AddChangeListener(DataUpdatedListener listener)
    {
        EventUpdatedListeners -= listener;
        EventUpdatedListeners += listener;
    }

    public void RemoveChangeListener(DataUpdatedListener listener)
    {
        EventUpdatedListeners -= listener;
    }

    public virtual void NotifyDataChanged(SavePropertyBase changedProperty)
    {
        EventUpdatedListeners?.Invoke(this, changedProperty);
    }

    public virtual void ParseData(JSONInStream stream)
    {
        if (!init)
        {
            UnityEngine.Debug.LogError("UH OH! you need to call InitSaveProperties in your custom PlayerSaveData! or it won't work!");
            return;
        }

        foreach (SavePropertyBase prop in saveProperties)
        {
            prop.ParseJSON(stream);
        }
    }

    public virtual void SerializeData(JSONOutStream stream)
    {
        if (!init)
        {
            UnityEngine.Debug.LogError("UH OH! you need to call InitSaveProperties in your custom PlayerSaveData! or it won't work!");
            return;
        }

        foreach (SavePropertyBase prop in saveProperties)
        {
            prop.SaveToJSON(stream);
        }
    }

    protected void InitSaveProperties(Type propertyContainingClass)
    {
        InitSaveProperties(propertyContainingClass.GetListOfStaticPublicMemberValues<SavePropertyBase>());
    }

    protected void InitSaveProperties(SavePropertyBase[] saveProperties)
    {
        this.saveProperties = saveProperties;
        foreach (SavePropertyBase prop in saveProperties)
        {
            prop.parent = this;
        }
        init = true;
    }

    public abstract bool IsReturningUser();
}
