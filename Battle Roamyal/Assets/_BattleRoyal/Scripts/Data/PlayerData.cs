using UnityEngine;
using System.Collections;

[System.Serializable]
public class PlayerData
{
    public StringSaveProperty playerName = new StringSaveProperty("playerName");
    public ColorSaveProperty playerColor = new ColorSaveProperty("playerColor");
    public PlayerSaveData saveData;

    public PlayerData() 
    {
        //Defaults
        playerName.value = "Player";
        playerColor.value = Color.white;
    }

    public PlayerData(string json)
    {
        ParseJSON(json);
    }

    public bool isReturningUser
    {
        get 
        {
            return saveData.IsReturningUser();
        }
    }

    public PlayerData ParseJSON(string json) {

        JSONInStream stream = new JSONInStream(json);
        ParseData(stream);
        return this;
    }

    public string Serialize()
    {
        JSONOutStream outStream = new JSONOutStream();
        SerializeData(outStream);
        return outStream.Serialize();
    }

    protected virtual void ParseData(JSONInStream stream)
    {
        if (stream != null && stream.node != null)
        {
            playerName.ParseJSON(stream);
            playerColor.ParseJSON(stream);

            if (stream.Has("saveData"))
            {
                stream.Start("saveData");
                saveData?.ParseData(stream);
                stream.End();
            }
        }
    }

    protected virtual void SerializeData(JSONOutStream stream)
    {
        playerName.SaveToJSON(stream);
        playerColor.SaveToJSON(stream);

        if (saveData != null) 
        {
            stream.Start("saveData");
            saveData.SerializeData(stream);
            stream.End();
        }
    }
}
