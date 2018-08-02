using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class SavePropertyBase
{
    public string jsonName;
    public PlayerSaveData parent;

    public SavePropertyBase(string name)
    {
        jsonName = name;
    }

    public abstract void SaveToJSON(JSONOutStream stream);
    public abstract void ParseJSON(JSONInStream stream);
}

[System.Serializable]
public abstract class SaveProperty<T> : SavePropertyBase, System.IEquatable<T>
{
    public delegate void SaveTransaction();
    public delegate void ChangeListener(SaveProperty<T> property);
    private event ChangeListener OnChangedEvent;

    protected T _propertyData = default(T);
    protected bool dirty = false;
    protected bool inTransaction = false;

    public virtual T value
    {
        get 
        {
            return _propertyData;
        }
        set
        {
            _propertyData = value;
            NotifyDataChanged();
        }
    }

    public bool Equals(T other)
    {
        if (_propertyData == null)
        {
            return other == null;
        }
        return _propertyData.Equals(other);
    }

    public SaveProperty(string name) : base(name){}

    public virtual void NotifyDataChanged()
    {
        dirty = true;
        if (inTransaction) return;

        parent?.NotifyDataChanged(this);
        OnChangedEvent?.Invoke(this);
        dirty = false;
    }

    public void AddChangeListener(ChangeListener listener)
    {
        OnChangedEvent -= listener;
        OnChangedEvent += listener;
    }

    public void RemoveChangeListener(ChangeListener listener)
    {
        OnChangedEvent -= listener;
    }

    /// <summary>
    /// Use if you wish to make multiple changes and only send callback when finished.
    /// </summary>
    /// <param name="transaction">Transaction.</param>
    public void ExecuteTransaction(SaveTransaction transaction)
    {
        inTransaction = true;
        transaction?.Invoke();
        inTransaction = false;

        if (dirty)
        {
            NotifyDataChanged();
        }
    }

}

[System.Serializable]
public abstract class ArraySaveProperty<T> : SaveProperty<List<T>>
{
    public delegate void ArrayChangeListener(ArraySaveProperty<T> property, List<T> addedValues);

    protected event ArrayChangeListener OnArrayChangedEvent;
    protected List<T> changedList;

    public ArraySaveProperty(string name) : base(name)
    {
        _propertyData = new List<T>();
        changedList = new List<T>();
    }

    public override List<T> value
    {
        get
        {
            throw new ArraySavePropertyAccessException();
        }

        set
        {
            throw new ArraySavePropertyAccessException();
        }
    }

    protected void SetChanges(T change)
    {
        if (OnArrayChangedEvent == null) return;
        changedList.Clear();
        changedList.Add(change);
    }

    protected void SetChanges(T[] changes)
    {
        if (OnArrayChangedEvent == null) return;
        changedList.Clear();
        changedList.AddRange(changes);
    }

    public void Add(T val)
    {
        _propertyData.Add(val);
        SetChanges(val);
        NotifyDataChanged();
    }

    public void AddRange(T[] val)
    {
        _propertyData.AddRange(val);
        SetChanges(val);
        NotifyDataChanged();
    }

    public void Remove(T val)
    {
        if (_propertyData.Contains(val))
        {
            _propertyData.Remove(val);
            NotifyDataChanged();
        }
    }

    public void Clear()
    {
        _propertyData.Clear();
        NotifyDataChanged();
    }

    public bool Contains(T val)
    {
        return _propertyData.Contains(val);
    }

    public bool ContainsAny(T[] val)
    {
        foreach (T t in val)
        {
            if (_propertyData.Contains(t))
            {
                return true;
            }
        }
        return false;
    }

    public T Get(int index)
    {
        return _propertyData[index];
    }

    public T[] GetAll()
    {
        return _propertyData?.ToArray();
    }

    public T Find(System.Predicate<T> predicate)
    {
        return _propertyData.Find(predicate);
    }

    public List<T> FindAll(System.Predicate<T> predicate)
    {
        return _propertyData.FindAll(predicate);
    }

    public override void ParseJSON(JSONInStream stream)
    {
        if (stream == null || stream.node == null) return;

        _propertyData.Clear();
        stream.List(jsonName, (int arg1, JSONInStream subStream) => 
        {
            T obj = ParseArrayElement(subStream);
            if (obj != null) _propertyData.Add(obj);
        });
    }

    public override void SaveToJSON(JSONOutStream stream)
    {
        stream.List(jsonName);
        for (int i = 0; i < _propertyData.Count; ++i)
        {
            SaveArrayElement(i, stream);
        }
        stream.End();
    }

    public override void NotifyDataChanged()
    {
        base.NotifyDataChanged();
        if (inTransaction) return;

        if (OnArrayChangedEvent != null)
        {
            OnArrayChangedEvent?.Invoke(this, changedList);
            changedList.Clear();
        }
    }

    public void AddArrayChangeListener(ArrayChangeListener listener)
    {
        OnArrayChangedEvent -= listener;
        OnArrayChangedEvent += listener;
    }

    public void RemoveArrayChangeListener(ArrayChangeListener listener)
    {
        OnArrayChangedEvent -= listener;
    }

    protected abstract T ParseArrayElement(JSONInStream stream);
    protected abstract void SaveArrayElement(int index, JSONOutStream stream);

    public class ArraySavePropertyAccessException : System.Exception
    {
        public ArraySavePropertyAccessException()
            : base("Do not attempt to access value directly on Array Save Properties. Use Add/Remove/Get/Contains methods")
        {}
    }

}

[System.Serializable]
public class StringArrayProperty : ArraySaveProperty<string>
{
    public StringArrayProperty(string name) : base(name) { }
    protected override string ParseArrayElement(JSONInStream stream)
    {
        string val;
        stream.Content(out val);
        return val;
    }

    protected override void SaveArrayElement(int index, JSONOutStream stream)
    {
        stream.Content(_propertyData[index]);
    }
}

[System.Serializable]
public class IntArrayProperty : ArraySaveProperty<int>
{
    public IntArrayProperty(string name) : base(name) { }
    protected override int ParseArrayElement(JSONInStream stream)
    {
        int val = -1;
        stream.Content(out val);
        return val;
    }

    protected override void SaveArrayElement(int index, JSONOutStream stream)
    {
        stream.Content(_propertyData[index]);
    }
}

[System.Serializable]
public class StringSaveProperty : SaveProperty<string>
{
    public StringSaveProperty(string name) : base(name) {}
    public override void ParseJSON(JSONInStream stream)
    {
        if (stream == null || stream.node == null) return;
        if (_propertyData == null) _propertyData = "";
        stream.ContentOptional(jsonName, ref _propertyData);
    }

    public override void SaveToJSON(JSONOutStream stream)
    {
        stream.Content(jsonName, _propertyData);
    }
}

[System.Serializable]
public class ColorSaveProperty : SaveProperty<Color>
{
    public ColorSaveProperty(string name) : base(name) {}
    public override void ParseJSON(JSONInStream stream)
    {
        if (stream == null || stream.node == null) return;
        if (_propertyData == null) _propertyData = Color.white;
        stream.ContentOptional(jsonName, ref _propertyData);
    }

    public override void SaveToJSON(JSONOutStream stream)
    {
        stream.Content(jsonName, _propertyData);
    }
}

public abstract class SerializableSaveObject
{
    protected delegate void OnChanged(SerializableSaveObject obj);
    protected event OnChanged OnChangeListener;

    public abstract void Serialize(JSONOutStream stream);
    public abstract void Parse(JSONInStream stream);
    protected virtual void OnDataChanged()
    {
        OnChangeListener?.Invoke(this);
    }
}

public class SerializableObjectProperty<T> : SaveProperty<T> where T : SerializableSaveObject
{
    public SerializableObjectProperty(T initObject, string name) : base(name) 
    {
        _propertyData = initObject;
    }

    public override void ParseJSON(JSONInStream stream)
    {
        if (stream == null || stream.node == null) return;
        if (stream.Has(jsonName))
        {
            stream.Start(jsonName);
            _propertyData.Parse(stream);
            stream.End();
        }
    }

    public override void SaveToJSON(JSONOutStream stream)
    {
        stream.Start(jsonName);
        _propertyData.Serialize(stream);
        stream.End();
    }
}

[System.Serializable]
public class BoolSaveProperty : SaveProperty<bool>
{
    public BoolSaveProperty(string name) : base(name) {}
    public override void ParseJSON(JSONInStream stream)
    {
        if (stream == null || stream.node == null) return;
        stream.ContentOptional(jsonName, ref _propertyData);
    }

    public override void SaveToJSON(JSONOutStream stream)
    {
        stream.Content(jsonName, _propertyData);
    }
}

[System.Serializable]
public class IntSaveProperty : SaveProperty<int>
{
    public IntSaveProperty(string name) : base(name) { }
    public override void ParseJSON(JSONInStream stream)
    {
        if (stream == null || stream.node == null) return;
        stream.ContentOptional(jsonName, ref _propertyData);
    }

    public override void SaveToJSON(JSONOutStream stream)
    {
        stream.Content(jsonName, _propertyData);
    }
}

[System.Serializable]
public class FloatSaveProperty : SaveProperty<float>
{
    public FloatSaveProperty(string name) : base(name) { }
    public override void ParseJSON(JSONInStream stream)
    {
        if (stream == null || stream.node == null) return;
        stream.ContentOptional(jsonName, ref _propertyData);
    }

    public override void SaveToJSON(JSONOutStream stream)
    {
        stream.Content(jsonName, _propertyData);
    }
}
