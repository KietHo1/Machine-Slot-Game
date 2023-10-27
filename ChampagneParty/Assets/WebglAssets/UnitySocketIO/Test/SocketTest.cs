using UnityEngine;
using System.Collections;
using UnitySocketIO;
using UnitySocketIO.Events;
using System;
using UnityEngine.UI;

public class SocketTest : MonoBehaviour {

    private void Start()
    {
        ClientSC.On(Function.connect, OnConnect);
        ClientSC.On(Function.disconnect, OnDisconnect);

    }

    private void OnDisconnect(SocketIOEvent obj)
    {
        JSONObject data = obj.data;
        print(data);
    }

    private void OnConnect(SocketIOEvent obj)
    {
        //print("connect " + obj);

        //JSONObject data = new JSONObject();
        //data.AddField(Schema.uID, "abhgn");
        //ClientSC.Submit(Function.test, data);

    }
}
