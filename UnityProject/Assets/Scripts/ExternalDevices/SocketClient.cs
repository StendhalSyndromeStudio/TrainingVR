using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;
using System;

public class SocketClient : MonoBehaviour {
    public string identifier;
    public string server_IP;
    public int server_PORT;
    public bool isRun = true;
    public AsynchronousClientExample AsynchronousClient = null;

    public event AsynchronousClientExample.delIncomingMessage_String IncomingMessage = null;
    public event AsynchronousClientExample.delIncomingMessage_Bytes IncomingBytes = null;
    public event AsynchronousClientExample.delConnectSocket OnChangeStatusConnect = null;

    private List<byte[ ]> _ListMessageByServer = new List<byte[ ]>( );
    private object lockerFor_ListMessageByServer = new object( );
    private List<byte[ ]> ListMessageByServer {
        get { lock ( this.lockerFor_ListMessageByServer ) { return this._ListMessageByServer; } }
    }

    void Awake( ) {
        this.enabled = false;
    }
    

    public void StartClient( string end_line = "<EOF>", AsynchronousClientExample.EEncoding code = AsynchronousClientExample.EEncoding.ASCII , AsynchronousClientExample.TypeHelper typeHelper = AsynchronousClientExample.TypeHelper.none) {
        if ( this.AsynchronousClient == null ) {
            this.AsynchronousClient = new AsynchronousClientExample( this.server_IP, this.server_PORT, this, end_line, code, typeHelper);
            this.AsynchronousClient.IncomingMessage = this.Handler_IncomingMessage;
            this.AsynchronousClient.IncomingBytes = this.Handler_IncomingBytes;
            this.AsynchronousClient.OnChangeStatusConnect = this.Handler_StatusConnect;
            this.AsynchronousClient.StartClient( );
        }

        if ( !this.AsynchronousClient.isRun ) {
            this.AsynchronousClient.StartClient( );
        }

        this.enabled = typeHelper != AsynchronousClientExample.TypeHelper.none;
    }


    void Start() {
        if (this.AsynchronousClient == null) {
            this.StartClient( );
        }
    }

    void Update( ) {
        this.AsynchronousClient.Update( );
    }

    public void Setup( string identifier, string server_IP, int server_PORT ) {
        this.server_IP = server_IP;
        this.server_PORT = server_PORT;
        this.identifier = identifier;
    }

    public void Setup( XmlNode xmlNode ) {
        foreach ( XmlAttribute att in xmlNode.Attributes ) {
            switch ( att.Name ) {
                case "ip":
                    this.server_IP = att.Value;
                    break;

                case "port":
                    int.TryParse( att.Value, out this.server_PORT );
                    break;

                case "id":
                    this.identifier = att.Value;
                    break;
            }
        }
    }

    void OnApplicationQuit( ) {
        if ( this.AsynchronousClient != null ) {
            this.AsynchronousClient.StopClient( );
        }
    }

    internal void SendMessageToServer( string message, bool useHeader = false ) {
        this.AsynchronousClient.SendMessage( message, useHeader );
    }

    private void Handler_IncomingMessage( AsynchronousClientExample sender, string message ) {
        if ( this.IncomingMessage != null ) {
            this.IncomingMessage( sender, message );
        }
    }

    private void Handler_IncomingBytes(AsynchronousClientExample sender, byte[] message) {
        if (this.IncomingBytes != null) {
            this.IncomingBytes(sender, message);
        }
    }

    private void Handler_StatusConnect( AsynchronousClientExample sender, bool status ) {
        try {
            if ( this.OnChangeStatusConnect != null )
                this.OnChangeStatusConnect( sender, status );
        }
        catch { }
    }
}
