
using System;
using System.Collections;
using UnityEngine;
using UnitySocketIO.Events;

public class GameFunc : Singleton<GameFunc>
{

    string sID = null;
    public static bool isConnect = false;

    public static Action OnDisconnectAction, OnFreeze;

    public static bool isOnLine
    {
        get
        {
            print(Application.platform);
            return Application.platform != RuntimePlatform.WindowsEditor;
        }
    }

    // Use this for initialization
    void Start()
    {

        ClientSC.On(Function.connect, OnConnect);
        ClientSC.On(Function.ForceClose, OnForceClose);

        ClientSC.On(Function.getTerminalInfo, OnGetTerminalInfo);
        ClientSC.On(Function.getGameInfo, OnGetGameInfo);

        ClientSC.On(Function.update_balance, OnUpdateBalance);

    }

    static int numberLoadingData = 0;

    internal static IEnumerator LoadingData()
    {
        while (numberLoadingData > 0) yield return null;
    }

    public static void LoadTerminalInfo()
    {
        JSONObject data = new JSONObject();
        data.AddField(Schema.terminal_id, UserData.terminal_id);
        ClientSC.Submit(Function.getTerminalInfo, data);

        numberLoadingData++;
    }

    private void OnGetTerminalInfo(SocketIOEvent obj)
    {
        UserData.LoadData(obj.data);

        numberLoadingData--;
    }

    public static void LoadGameInfo()
    {
        JSONObject data = new JSONObject();
        data.AddField(Schema.shop_id, UserData.shop_id);
        data.AddField(Schema.game_id, GameSetting.game_id);
        ClientSC.Submit(Function.getGameInfo, data);

        numberLoadingData++;

    }

    private void OnGetGameInfo(SocketIOEvent obj)
    {
        GameSetting.LoadData(obj.data);
        numberLoadingData--;
    }


    private void OnForceClose(SocketIOEvent obj)
    {
        Application.Quit();
    }


    private void OnConnect(SocketIOEvent obj)
    {
        isConnect = true;
        UserData.terminal_id = JavaSControl.TryGetTerminalID();

        JSONObject data = new JSONObject();
        data.AddField(Schema.terminal_id, UserData.terminal_id);
        ClientSC.Submit(Function.connect_server, data);
    }


    private void OnUpdateBalance(SocketIOEvent obj)
    {
        UserData.LoadBalance(obj.data);
    }

    //update history play
    public static void updateHistory(float bet, float line, float win, float bonus)
    {
        if (!isOnLine) return;

        JSONObject data = new JSONObject();
        data.AddField(Schema.terminal_id, UserData.terminal_id);
        data.AddField(Schema.game_id, GameSetting.game_id);
        data.AddField(Schema.bet, bet);
        data.AddField(Schema.line, line);
        data.AddField(Schema.win, win);
        data.AddField(Schema.bonus, bonus);

        ClientSC.Submit(Function.updateHistory, data);
    }


}