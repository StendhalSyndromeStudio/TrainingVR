using UnityEngine;
using System.Collections;
using ExitGames.Client.Photon;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Timers;

public enum CodeHandler : byte
{
    DebugReturn,
    OnEvent,
    OnOperationResponse,
    OnStatusChanged,
}

public class ThreadPhotonServer : IPhotonPeerListener
{    
    public string Ip_Server = "127.0.0.1";
    public string Port_Server = "4530";
    public string APP_NAME = "MyServerPhoton";
    private PhotonServerTCP Parent = null;

    private bool _Update_Status = true;
    public bool Update_Status
    {
        get { return _Update_Status;  }
        set { _Update_Status = value; }
    }
        
    private int timer = 50;

    private PhotonPeer PhotonPeer;

    // КОНСТРУКТОР
    public ThreadPhotonServer(PhotonServerTCP _parent, string _Ip, string _Port)
    {
        Parent = _parent;
        Ip_Server = _Ip;
        Port_Server = _Port;

        PhotonPeer = new PhotonPeer(this, ConnectionProtocol.Tcp);
        Debug.Log("ThreadPhotonServer " + Ip_Server + " " + Port_Server+" Update: "+ Update_Status.ToString());
        ConnectToServer();
        
    }
    

    // МЕТОД УСТАНОВКИ СОЕДИНЕНИЯ
    private void ConnectToServer()
    {
        PhotonPeer.Connect(Ip_Server + ":" + Port_Server, APP_NAME);
    }

    public void ServerUpdate()
    {        
        while (Update_Status)
        {

            try
            {
                if (PhotonPeer != null)
                    PhotonPeer.Service();

                Thread.Sleep(timer);
            }
            catch(Exception ex)
            {
                UnityEngine.Debug.Log("____________ServerUpdate : " + ex.ToString());
            }
        }
    }


    #region ИНТЕРФЕЙС PHOTON CONTROL

    public void DebugReturn(DebugLevel level, string message)
    {
        
    }

    public void OnEvent(EventData eventData)
    {
        try
        {
            Parent.Dic_CodeHandler(CodeHandler.OnEvent, eventData);
        }
        catch(Exception ex)
        {
            UnityEngine.Debug.Log("____________OnEvent: " + ex.ToString());
        }
    }

    public void OnOperationResponse(OperationResponse operationResponse)
    {
        try
        {
            Parent.Dic_CodeHandler(CodeHandler.OnOperationResponse, operationResponse);
        }
        catch(Exception ex)
        {
            UnityEngine.Debug.Log("____________OnOperationResponse: " + ex.ToString());
        }
    }

    public void OnStatusChanged(StatusCode statusCode)
    {
        try
        {
            if (statusCode != StatusCode.Connect)
            {               
                ConnectToServer();
            }
            Parent.Dic_CodeHandler(CodeHandler.OnStatusChanged, statusCode);
        }
        catch(Exception ex)
        {
            UnityEngine.Debug.Log("____________OnStatusChanged: " + ex.ToString());
        }

    }

    #endregion

    internal void OpCustom(PhotonServerTCP parent, byte operation, Dictionary<byte, object> dictionary, bool sendReliable)
    {
        if (Parent != parent)
        {
            Parent = parent;
        }
        PhotonPeer.OpCustom(operation, dictionary, sendReliable);
    }

    public void Disconnect()
    {
        System.Timers.Timer timerOnDisconnet = new System.Timers.Timer(1000);
        timerOnDisconnet.Elapsed += this.OnTimerOnDisconnet_Elapsed;
        timerOnDisconnet.Enabled = true;
    }

    private void OnTimerOnDisconnet_Elapsed(object sender, ElapsedEventArgs e)
    {
        System.Timers.Timer timerOnDisconnet = sender as System.Timers.Timer;
        timerOnDisconnet.Enabled = false;
        this.Update_Status = false;
        UnityEngine.Debug.Log("User PhotonServer Disconnect");
    }
}
