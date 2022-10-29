using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PrefsWrapper
{

    public static void ImportDataFromCloud(string globalSave)
    {
        DeleteAll();
        var json = JsonHelper.ConvertToJson(globalSave);
        foreach (var e in json)
        {
            var jsonValue = JsonHelper.ConvertToJson(e.Value.ToString());
            string key = e.Key;
            string type = jsonValue["type"].ToString();
            string value = jsonValue["value"].ToString();
            switch (type)
            {
                case "float":
                    SetFloat(key, float.Parse(value));
                    break;
                case "int":
                    SetInt(key, System.Convert.ToInt32(value));
                    break;
                case "string":
                    SetString(key, value);
                    break;
            }
        }
        Save();
    }
    public static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }

    public static void DeleteKey(string key)
    {

        PlayerPrefs.DeleteKey(key);
        var globalSave = PlayerPrefs.GetString("GlobalSave", "{}");
        globalSave = JsonHelper.DeleteKey(globalSave, key);
        PlayerPrefs.SetString("GlobalSave", globalSave);
    }

    public static float GetFloat(string key, float defaultValue)
    {
        if (!PlayerPrefs.HasKey(key)) return defaultValue;
        else return PlayerPrefs.GetFloat(key);
    }

    public static float GetFloat(string key)
    {
        if (!PlayerPrefs.HasKey(key)) return 0;
        else return PlayerPrefs.GetFloat(key);
    }

    public static int GetInt(string key, int defaultValue)
    {
        if (!PlayerPrefs.HasKey(key)) return defaultValue;
        else return PlayerPrefs.GetInt(key);
    }

    public static int GetInt(string key)
    {
        if (!PlayerPrefs.HasKey(key)) return 0;
        else return PlayerPrefs.GetInt(key);
    }

    public static string GetString(string key, string defaultValue)
    {
        if (!PlayerPrefs.HasKey(key)) return defaultValue;
        else return PlayerPrefs.GetString(key);
    }

    public static string GetString(string key)
    {
        if (!PlayerPrefs.HasKey(key)) return "";
        else return PlayerPrefs.GetString(key);
    }

    public static bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    public static void Save()
    {
        PlayerPrefs.Save();
    }

    public static void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        saveToGlobal(key, value);
    }

    public static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        saveToGlobal(key, value);
    }

    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        saveToGlobal(key, value);
    }

    public static void saveToGlobal(string key, float value)
    {
        if (!PlayerPrefs.HasKey("GlobalSave")) PlayerPrefs.SetString("GlobalSave", "{}");
        var globalSave = PlayerPrefs.GetString("GlobalSave");
        globalSave = JsonHelper.DeleteKey(globalSave, key);
        var shouldAddComma = globalSave == "{}" ? "" : ",";
        globalSave = globalSave.Substring(0, globalSave.Length - 1) + shouldAddComma + string.Format("\"{0}\":{{\"type\":\"{1}\",\"value\":{2}}}", key, "float", value.ToString().Replace(",", ".")) + "}";
        PlayerPrefs.SetString("GlobalSave", globalSave);

    }
    public static void saveToGlobal(string key, int value)
    {
        if (!PlayerPrefs.HasKey("GlobalSave")) PlayerPrefs.SetString("GlobalSave", "{}");
        var globalSave = PlayerPrefs.GetString("GlobalSave");
        globalSave = JsonHelper.DeleteKey(globalSave, key);
        var shouldAddComma = globalSave == "{}" ? "" : ",";
        globalSave = globalSave.Substring(0, globalSave.Length - 1) + shouldAddComma + string.Format("\"{0}\":{{\"type\":\"{1}\",\"value\":{2}}}", key, "int", value) + "}";
        PlayerPrefs.SetString("GlobalSave", globalSave);

    }
    public static void saveToGlobal(string key, string value)
    {
        if (!PlayerPrefs.HasKey("GlobalSave")) PlayerPrefs.SetString("GlobalSave", "{}");
        var globalSave = PlayerPrefs.GetString("GlobalSave");
        globalSave = JsonHelper.DeleteKey(globalSave, key);
        var shouldAddComma = globalSave == "{}" ? "" : ",";
        globalSave = globalSave.Substring(0, globalSave.Length - 1) + shouldAddComma + string.Format("\"{0}\":{{\"type\":\"{1}\",\"value\":\"{2}\"}}", key, "string", value) + "}";
        PlayerPrefs.SetString("GlobalSave", globalSave);

    }

}
