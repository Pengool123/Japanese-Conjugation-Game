using System.IO;
using System.Net;
using Godot;
namespace jp_conj_game.scripts;
using System.Text.Json;
using System.Text.Json.Nodes;
using FileAccess = Godot.FileAccess;

public class Config
{
    private static string _path = "./json/config.json";
    private static JsonNode _json;
    
    public static bool wasInLvlMenu = false;
    public static ushort currLvl;
    public static bool isVerb;
    
    public static void Load()
    {
        if (FileAccess.FileExists(_path))
        {
            string jsonText = File.ReadAllText(_path);
            GD.Print(jsonText);
            _json = JsonNode.Parse(jsonText);
        }
        else
        {
            GD.Print("Config file cannot be open");
        }
    }
    
    public static void Save()
    {
        if (_json != null && FileAccess.FileExists(_path))
        {
            string updatedJson = _json.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_path, updatedJson);
        }
        else
        {
            GD.Print("saving to config failed");
        }
    }

    //lvl data
    public static ushort GetMaxVerbLvl()
    {
        return (ushort)_json["Data"]["MaxLvlVerb"];
    }

    public static void SetMaxVerbLvl(ushort lvl)
    {
        _json["Data"]["MaxLvlVerb"] = lvl;
        Save();
    }

    public static ushort GetMaxAdjLvl()
    {
        return (ushort)(_json["Data"]["MaxLvlAdj"]);
    }

    public static void SetMaxAdjLvl(ushort adj)
    {
        _json["Data"]["MaxLvlAdj"] = adj;
        Save();
    }
    
    //settings
    
    //Irregular Godan status
    public static bool getIrrGStatus()
    {
        return (bool)_json["Settings"]["OnlyIrrGo"];
    }

    public static void setIrrGStatus(bool status)
    {
        _json["Settings"]["OnlyIrrGo"] = status;
        Save();
    }

    public static bool getTimerStatus()
    {
        return (bool)_json["Settings"]["TimerStatus"];
    }

    public static void setTimerStatus(bool status)
    {
        _json["Settings"]["TimerStatus"] = status;
        Save();
    }

    public static ushort GetTimerLen()
    {
        return (ushort)_json["Settings"]["TimerTime"];
    }

    public static void SetTimerLen(ushort newTime)
    {
        _json["Settings"]["TimerTime"] = newTime;
        Save();
    }
    
}