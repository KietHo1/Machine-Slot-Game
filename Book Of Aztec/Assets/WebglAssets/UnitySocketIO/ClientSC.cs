using System;
using UnityEngine;
using UnitySocketIO;
using UnitySocketIO.Events;

public class ClientSC : Singleton<ClientSC>
{

    public SocketIOController mSocket;

    protected override void Awake()
    {
        base.Awake();

        Application.targetFrameRate = 60;
    }


    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

    }


    public static void Submit(Function func, JSONObject json = null)
    {
        if (!Instance.mSocket) return;

        if (json != null)
            Instance.mSocket.Emit(func.ToString(), json.ToString());
        else if (json == null)
        {
            Instance.mSocket.Emit(func.ToString());
        }
    }

    public static void On(Function func, Action<SocketIOEvent> callback)
    {
        if (Instance.mSocket)
            Instance.mSocket.On(func.ToString(), callback);
    }

    public static void Off(Function func, Action<SocketIOEvent> callback)
    {
        if (Instance.mSocket)
            Instance.mSocket.Off(func.ToString(), callback);
    }

    internal static bool isLocal
    {
        get { return Instance.mSocket.settings.isLocal; }
    }
}