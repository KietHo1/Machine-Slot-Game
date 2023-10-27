
using System;

public static class extendClass
{
    public static void GetField(this JSONObject ob, ref int var, Schema s)
    {
        ob.GetField(ref var, s.ToString());
    }




    internal static int GetIField(this JSONObject ob, Schema s)
    {
        int value = 0;
        ob.GetField(ref value, s);
        return value;
    }

    public static void GetField(this JSONObject ob, ref string var, Schema s)
    {
        ob.GetField(ref var, s.ToString());
    }

    public static void GetField(this JSONObject ob, ref bool var, Schema s)
    {
        ob.GetField(ref var, s.ToString());
    }

    public static void GetField(this JSONObject ob, ref float var, Schema s)
    {
        ob.GetField(ref var, s.ToString());
    }

    public static JSONObject GetField(this JSONObject ob, Schema s)
    {
        return ob.GetField(s.ToString());
    }

	 public static String GetSField(this JSONObject ob, Schema s)
    {
        JSONObject value = ob.GetField(s);
        if (value != null) return value.str;

        return "";
    }


    public static void GetField(this JSONObject ob, ref long var, Schema s)
    {
        float fval = 0;
        ob.GetField(ref fval, s.ToString());

        var = (long)fval;
    }

    public static void AddField(this JSONObject ob, Schema s, int var)
    {
        ob.AddField(s.ToString(), var);
    }

    public static void AddField(this JSONObject ob, Schema s, string var)
    {
        ob.AddField(s.ToString(), var);
    }

    public static void AddField(this JSONObject ob, Schema s, float var)
    {
        ob.AddField(s.ToString(), var);
    }

    public static void AddField(this JSONObject ob, Schema s, bool var)
    {
        ob.AddField(s.ToString(), var);
    }

    public static void AddField(this JSONObject ob, Schema s, JSONObject var)
    {
        ob.AddField(s.ToString(), var);
    }


    public static void SetField(this JSONObject ob, Schema s, float var)
    {
        ob.SetField(s.ToString(), var);
    }

    public static void SetField(this JSONObject ob, Schema s, JSONObject var)
    {
        ob.SetField(s.ToString(), var);
    }


    public static void RemoveField(this JSONObject ob, Schema s)
    {
        ob.RemoveField(s.ToString());
    }
}

public enum Schema
{
    status,
    balance,
    terminal_id,
    shop_id,
    game_id,
    balance_point,
    name,
    max_bet,
    min_bet,
    min_line,
    max_win,
    percent,
    bet,
    line,
    win,
    bonus,
    auto_bonus,
}

public enum Function
{
    connect,
    disconnect,
    reconnect,
    CheckVersion,
    ForceClose,
    UserReconnect,
    update_balance,
    getTerminalInfo,
    getGameInfo,
    connect_server
}