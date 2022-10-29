using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JsonHelper
{
    public static JObject ConvertToJson(string str)
    {
        //PrefsWrapper.SetFloat("test", 1.1f);
        //string str = "{\"Key\":{\"type\":\"string\",\"value\":\"kati\"},\"Key2\":{\"type\":\"int\",\"value\":5}}";       
        JObject json = JObject.Parse(str);
        return json;

    }
    public static string DeleteKey(string jsonstr, string key)
    {
        JObject json = JObject.Parse(jsonstr);
        json.Remove(key);
        return json.ToString();

    }
}
