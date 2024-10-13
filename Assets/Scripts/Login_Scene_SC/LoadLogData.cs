using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

[Serializable]
public class LogData  
{
    public string ID;
    public LogData() { ID = "";  }
}

public class LoadLogData : MonoBehaviour
{
    string dataPath = "";
    LogData logData = null;
    public LogData Data
    {
        get
        {
            if (logData == null)
                return null;
            return logData;
        }
        set
        {
            logData = value;
        }
    }

    private void Awake()
    {
        //Debug.Log(Application.persistentDataPath);
        LoadData();
    }


    public void LoadData()
    {
        dataPath = Application.persistentDataPath + "/LogData.json";
        if (File.Exists(dataPath))
        {
            string jsonFile = File.ReadAllText(dataPath);   
            LogData newLogData = JsonUtility.FromJson<LogData>(jsonFile);
            logData = newLogData;
        }
        else
        {
            LogData writeLogData = new LogData();
            logData = writeLogData;
            string textLogs = JsonUtility.ToJson(writeLogData);
            File.WriteAllText(dataPath, textLogs);
        }
    }

    public void RenewLog(string log)
    {
       
        if (logData == null)
            logData = new LogData();
        logData.ID = log;
        string textLogs = JsonUtility.ToJson(logData);
        File.WriteAllText(dataPath, textLogs);
    }
}
