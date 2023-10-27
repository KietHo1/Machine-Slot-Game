using System;
using System.Runtime.InteropServices;
using UnityEngine;

//all function in javascript is add to myplugin mergeInto(LibraryManager.library, MyPlugin);
public class JavaSControl 
{
    [DllImport("__Internal")]
    public static extern string GetTerminalID();

    [DllImport("__Internal")]
    public static extern string GetShopID();

    [DllImport("__Internal")]
    public static extern string GetGameID();

    [DllImport("__Internal")]
    public static extern string GoToLobby();


    public static string TryGetTerminalID()
    {
        try
        {
            string value = GetTerminalID();
            return value;
        } catch(Exception e)
        {
            return "1";
        }
    }

    public static string TryGetShopID()
    {
        try
        {
            string value = GetShopID();
            return value;
        }
        catch (Exception e)
        {
            return "1";
        }
    }

    public static string TryGetGameID()
    {
        try
        {
            string value = GetGameID();
            return value;
        }
        catch (Exception e)
        {
            return "1";
        }
    }

}
