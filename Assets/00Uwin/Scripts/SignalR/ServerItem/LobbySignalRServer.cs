using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.SignalR;
using BestHTTP.SignalR.Hubs;
using BestHTTP.SignalR.Messages;
using Facebook.MiniJSON;
using LitJson;
using UnityEngine;

public class LobbySignalRServer : ISignalRServer
{
    #region Properties
    protected IEnumerator ieAutoCallPingpong;
    #endregion

    #region SignalR
    protected override void OnConnected(Connection con)
    {
        base.OnConnected(con);

        if (OnSRSEvent != null)
        {
            OnSRSEvent.Invoke(SRSConst.ON_CONNECTED, null);
        }
    }

    protected override void OnClosed(Connection con)
    {
        base.OnClosed(con);

        if (OnSRSEvent != null)
        {
            OnSRSEvent.Invoke(SRSConst.ON_CLOSED, null);
        }
    }

    protected override void OnError(Connection con, string err)
    {
        base.OnError(con, err);

        //        StopPingpong();

        if (OnSRSEvent != null)
        {
            OnSRSEvent.Invoke(SRSConst.ON_ERROR, new object[] { err });
        }
    }

    protected override void OnReconnecting(Connection con)
    {
        base.OnReconnecting(con);

        //        StopPingpong();

        if (OnSRSEvent != null)
        {
            OnSRSEvent.Invoke(SRSConst.ON_RECONNECTING, null);
        }
    }

    protected override void OnReconnected(Connection con)
    {
        base.OnReconnected(con);

        //        StartPingpong();

        if (OnSRSEvent != null)
        {
            OnSRSEvent.Invoke(SRSConst.ON_RECONNECTED, null);
        }
    }

    protected override void OnStateChanged(Connection con, ConnectionStates oldState, ConnectionStates newState)
    {
        base.OnStateChanged(con, oldState, newState);
    }

    protected override void OnNonHubMessage(Connection con, object data)
    {
        base.OnNonHubMessage(con, data);

        VKDebug.LogWarning("OnNonHubMessage");
    }
    #endregion

    #region Hub Response

    protected void HubUpdateMoneyUser(Hub hub, MethodCallMessage msg)
    {
        Debug.Log("OnSRSHubEventLobby HubUpdateMoneyUser ");
        //if (OnSRSHubEvent != null)
        //{
        //    OnSRSHubEvent.Invoke(SRSConst.ENTER_LOBBY, msg.Arguments);
        //}
    }

    protected void HubUpdateMoneyLobby(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.UpdateMoneyLobby, msg.Arguments);
        }
    }

    protected void HubKickUser(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.KICK_USER, msg.Arguments);
        }
    }
    #endregion

    #region Hub Send method

    public void HubCallEnterLobby(int moneyType)
    {
        _hub.Call("EnterLobby", (int)moneyType);
    }

    public void HubCallBet(int moneyType, double betAmount, int betSide)
    {
        _hub.Call("Bet", (int)moneyType, betAmount, betSide);
    }

    public void HubCallGetMessage()
    {
        _hub.Call("GetMessage");
    }

    public void HubCallText(string msg)
    {
        _hub.Call("Text", msg);
    }
    #endregion

    #region Hub Method
    protected override void RegisterHubFunction()
    {
        base.RegisterHubFunction();
        Debug.Log("OnSRSHubEventLobby RegisterHubFunction");
        // listener
        _hub.On("UpdateMoneyUser", HubUpdateMoneyUser);
        _hub.On("UpdateMoneyLobby", HubUpdateMoneyLobby);
       
    }

    #endregion
}

