using UnityEngine;
using System;
using System.Text;
using System.Net.Sockets;
using System.Linq;
using System.Collections.Generic;

public class AsynchronousClientExample : IDisposable
{
    public enum EEncoding {
        ASCII,
        UTF8,
        Unicode,
        BigEndianUnicode,
        Default,
        UTF32
    }

    public enum TypeHelper {
        none,
        UInt16,
        UInt32,
        UInt64,
    }
    public class StateObject : IDisposable
    {
        // Client socket.
        public System.Net.Sockets.Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();

        private bool _isStop = true;
        private object locker_isStop = new object();
        public bool isStop
        {
            get { lock (this.locker_isStop) { return this._isStop; } }
            set { lock (this.locker_isStop) { this._isStop = value; } }
        }

        public void Dispose()
        {
            this.buffer = null;           
            this.sb = null;
        }
    }

    public delegate void delIncomingMessage_String(AsynchronousClientExample sender, string message);
    public delegate void delIncomingMessage_Bytes(AsynchronousClientExample sender, byte[] message);
    public delegate void delConnectSocket(AsynchronousClientExample sender, bool status);
    
    // ManualResetEvent instances signal completion.
    private System.Threading.ManualResetEvent connectDone = new System.Threading.ManualResetEvent(false);
    private System.Threading.ManualResetEvent sendDone = new System.Threading.ManualResetEvent(false);

    
    /*private System.Threading.ManualResetEvent _receiveDone = new System.Threading.ManualResetEvent(false);
    private object lockerFor_receiveDone = new object();
    private System.Threading.ManualResetEvent receiveDone
    {
        get { lock (this.lockerFor_receiveDone) { return this._receiveDone; } }
    }*/

    private System.Net.IPAddress _Server_IP = System.Net.IPAddress.Loopback;
    private int server_PORT = 20000;
    public bool isRun = false;
    private System.Timers.Timer threadSocket;
    private System.Net.Sockets.Socket ClientServer;
    private StateObject CurrentState;
    

    public delIncomingMessage_String IncomingMessage = null;
    public delIncomingMessage_Bytes IncomingBytes = null;
    public delConnectSocket OnChangeStatusConnect = null;
    public SocketClient Parent_SocketClient = null;

    /// <summary>
    /// символ конца строки
    /// </summary>
    private string end_line = "<EOF>";

    /// <summary>
    /// тип кодировки
    /// </summary>
    private EEncoding encoding_type = EEncoding.ASCII;    
    public bool isConnect
    {
        get { return this.GET_isConnect(); }
    }
    private bool GET_isConnect()
    {
        try
        {
            return this.ClientServer.Connected;
        }
        catch
        {
            return false;
        }
    }
   
    private IReadDataFromSocketClass m_ReadDataFromSocketClass;

    public AsynchronousClientExample(string ipadress, int port, SocketClient client, string endl = "<EOF>", EEncoding code = EEncoding.ASCII, TypeHelper typehHeader = TypeHelper.none )
    {        
        if (!System.Net.IPAddress.TryParse(ipadress, out this._Server_IP))
        {
            return;
        }

        if ((port <= 0) || (port >= ushort.MaxValue))
        {
            return;
        }
        if (typehHeader != TypeHelper.none) {
            this.m_ReadDataFromSocketClass = new ReadDataFromSocketClass(this, endl, code, typehHeader);
        }  

        this.Parent_SocketClient = client;
        this.server_PORT = port;
        this.end_line = endl;
        this.encoding_type = code;        
        this.isRun = true;
        
        this.threadSocket = new System.Timers.Timer(100);
        //this.threadSocket.SynchronizingObject = this.synchronizeInvoke;
        this.threadSocket.Stop();
    }

    internal void Update() {
        if (this.m_ReadDataFromSocketClass != null) {
            this.m_ReadDataFromSocketClass.Update( );
        }        
    }


    public void StartClient()
    {
        //Action action = () =>
        {
            try
            {                
                // Create a TCP/IP socket.
                if (this.ClientServer == null)
                {
                    this.ClientServer = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                    this.CurrentState = new StateObject();
                    this.ClientServer.ReceiveTimeout = 10;
                    this.ClientServer.SendTimeout = 500;
                }
                //if (!this.ClientServer.Connected)
                {
                    this.threadSocket.Elapsed += this.threadSocket_Connect;
                    this.threadSocket.Interval = 1000;
                    this.threadSocket.Start();
                    //this.threadSocket_Connect(null, null);
                }
            }
            catch { }
        };
        //this.synchronizeInvoke.Invoke(action, null);
    }

    public void StopClient( ) {
        System.Timers.Timer timer = new System.Timers.Timer( 1000 );
        timer.Elapsed += ( s, e ) => {

            System.Timers.Timer _timer = s as System.Timers.Timer;
            _timer.Enabled = false;
            //Debug.Log( "<color=blue> Stop Client </color>" );

            // Release the socket.        
            this.isRun = false;
            if ( this.threadSocket != null ) {
                this.threadSocket.Stop( );
                this.threadSocket.Dispose( );
            }
            sendDone.Set( );
            try {
                this.CurrentState.isStop = true;
            }
            catch { }
            //receiveDone.Set();
            if ( this.ClientServer != null ) {
                if ( this.ClientServer.Connected ) {
                    this.ClientServer.Shutdown( System.Net.Sockets.SocketShutdown.Both );
                    this.ClientServer.Close( );
                }
                this.ClientServer = null;
            }
            //Debug.Log("Server STOP");
        };
        timer.Enabled = true;

        //this.synchronizeInvoke.Invoke(action, null);
    }

    private void threadSocket_Connect(object sender, EventArgs e)
    {
        this.threadSocket.Stop();
        try
        {
            //Debug.Log("-->> threadSocket_Connect : " + this.ClientServer.Connected.ToString());
            if (this.ClientServer.Connected)
            {
                {
                    try
                    {
                        this.threadSocket.Stop();
                        this.threadSocket.Elapsed -= this.threadSocket_Connect;
                        this.threadSocket.Elapsed += this.threadSocket_Receive;
                        this.threadSocket.Interval = 10;
                        this.threadSocket.Start();
                    }
                    catch { }
                };
                return;
            }
            System.Net.IPEndPoint remoteEP = new System.Net.IPEndPoint(this._Server_IP, this.server_PORT);
            // Connect to the remote endpoint.
            this.connectDone.Set();
            var ar = this.ClientServer.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), this.ClientServer);
            this.connectDone.WaitOne();
            this.ClientServer.EndConnect(ar);
            //Debug.Log("<<-- threadSocket_Connect : " + this.ClientServer.Connected.ToString());
        }
        catch { }
        this.threadSocket.Start();
    }
    
    private void ConnectCallback(IAsyncResult ar)
    {
        System.Net.Sockets.Socket client = ar.AsyncState as System.Net.Sockets.Socket;
        if (client == null)
        {
            connectDone.Set();
            return;
        }
        try
        {
            // Retrieve the socket from the state object.            
            // Complete the connection.
            //client.EndConnect(ar);

            Debug.Log("Socket connected to " + client.RemoteEndPoint.ToString());
            try
            {
                if (this.OnChangeStatusConnect != null) this.OnChangeStatusConnect(this, true);
            }
            catch { }
            // Signal that the connection has been made.
            connectDone.Set();
        }
        catch
        {
            connectDone.Set(); 
        }
        //this.SendMessage("Hello!!! " + client.LocalEndPoint.ToString());
    }

    private void threadSocket_Receive(object sender, EventArgs e)
    {        
        {
            try
            {
                if (this.ClientServer == null) return;
                this.CurrentState.workSocket = this.ClientServer;
//Debug.Log("this.ClientServer: <<threadSocket_Receive>> is Stop: " + this.CurrentState.isStop.ToString());
                if (this.CurrentState.isStop)
                {
                    this.CurrentState.isStop = false;
                    this.ClientServer.BeginReceive(this.CurrentState.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), this.CurrentState);
                }
                //receiveDone.WaitOne();                    
            }
            catch
            {
                //Action action = () =>
                {
                    this.threadSocket.Stop();
                    this.threadSocket.Elapsed -= this.threadSocket_Receive;
                    this.ClientServer = null;
                    this.CurrentState.Dispose();
                    this.CurrentState = null;
                    if (this.isRun) this.StartClient();
                };
                //this.synchronizeInvoke.Invoke(action, null);
                
            }
        }
    }

    
    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the state object and the client socket 
            // from the asynchronous state object.            
            StateObject state = ar.AsyncState as StateObject;
            if (state == null) return;                                 
            System.Net.Sockets.Socket client = state.workSocket;
            int bytesRead = 0;
            try
            {
                // Read data from the remote device.
                bytesRead = client.EndReceive(ar);
            }
            catch { }

            //Debug.LogWarning( string.Format( "<-- ReceiveCallback count: {0}", bytesRead ) );

            if ( bytesRead > 0)
            {
                
                if (this.m_ReadDataFromSocketClass != null) {
                    //Debug.LogWarning( "<--m_ReadDataFromSocketClass" + bytesRead );
                    byte[ ] buffer = new byte[bytesRead];
                    Array.Copy(state.buffer, 0, buffer, 0, bytesRead);
                    this.m_ReadDataFromSocketClass.ReadDataFromSocket(ref buffer, 0, bytesRead);
                } else {
                    //Debug.LogWarning( "<--null" + bytesRead );
                    this.HandlerStateObject(state, 0, bytesRead);
                }                                        
            }
            else
            {
                try
                {
                    //Debug.Log("Сервер отключился!!!");
                    if ( this.OnChangeStatusConnect != null ) {
                        this.OnChangeStatusConnect( this, false );
                    }
                    client.Shutdown(System.Net.Sockets.SocketShutdown.Both);
                    client.Close();
                }
                catch
                {
                    this.threadSocket.Stop();
                    this.threadSocket.Elapsed -= this.threadSocket_Receive;
                    this.ClientServer = null;
                    this.CurrentState.Dispose();
                    this.CurrentState = null;
                    if (this.isRun) this.StartClient();
                }                

            }
//receiveDone.Set();
            state.isStop = true;
        }
        catch { }
    }

   
    private void HandlerStateObject(StateObject stateObject, int indexStart, int count) {
        // There might be more data, so store the data received so far.
        //Debug.Log( "<--HandlerStateObject" );

        for(int index = indexStart; index < count; index++ ) {
            this.ReadBuffer( stateObject, index );
        }

        
    }

    private void ReadBuffer( StateObject stateObject,  int index ) {
        stateObject.sb.Append( Encoding.ASCII.GetString( stateObject.buffer, index, 1 ) );
        string message = stateObject.sb.ToString( );
        if ( IndexOf( message ) > -1 ) {
            try {
                //Debug.Log( "<--count: " + count + " [" + message + "]" );
                if ( this.IncomingMessage != null ) {
                    this.IncomingMessage( this, message );
                }
            }
            catch { }
            stateObject.sb.Remove( 0, stateObject.sb.Length );
        }
    }

    private int IndexOf( string message ) {
        if (this.end_line == '\0'.ToString( ) ) {
            return message.IndexOf( '\0' );
        }
        return message.IndexOf( end_line );
    }


    internal void SendMessage(string message, bool useHeader = false )
    {
        try
        {
            if (!this.ClientServer.Connected) return;
            // Send test data to the remote device.
            //Send(this.ClientServer, message + "<EOF>", code);
            if ( useHeader ) {
                Send( this.ClientServer, message, useHeader );
            }
            else {
                Send( this.ClientServer, message + end_line );
            }
            sendDone.WaitOne();
        }
        catch { }
    }

    private void Send(System.Net.Sockets.Socket client, string data, bool useHeader = false )
    {
//Debug.Log( string.Format( "-->:[{0}]", data ) );
        // Convert the string data to byte data using ASCII encoding.
        byte[ ] byteData;// = Encoding.ASCII.GetBytes( data );

        switch ( encoding_type ) {
            case EEncoding.UTF8:
                byteData = Encoding.UTF8.GetBytes( data );
                break;

            case EEncoding.UTF32:
                byteData = Encoding.UTF32.GetBytes( data );
                break;

            case EEncoding.Unicode:
                byteData = Encoding.Unicode.GetBytes( data );
                break;

            case EEncoding.BigEndianUnicode:
                byteData = Encoding.BigEndianUnicode.GetBytes( data );
                break;

            case EEncoding.Default:
                byteData = Encoding.Default.GetBytes( data );
                break;

            default:
                byteData = Encoding.ASCII.GetBytes( data );
                break;
        }
        List<byte> buffer = new List<byte>( );
        if ( useHeader ) {
            byte[ ] header = BitConverter.GetBytes( ( Int64 ) byteData.Length );
            Array.Reverse( header );
            buffer.AddRange( header );
        }
        buffer.AddRange( byteData );
        try
        {
            // Begin sending the data to the remote device.
            client.BeginSend( buffer.ToArray( ), 0, buffer.Count, 0, new AsyncCallback( SendCallback ), client );
        }
        catch { }        
    }

    private void SendCallback(IAsyncResult ar)
    {
        try
        {
            // Signal that all bytes have been sent.
            sendDone.Set();

            // Retrieve the socket from the state object.
            System.Net.Sockets.Socket client = (System.Net.Sockets.Socket)ar.AsyncState;

            // Complete sending the data to the remote device.
            client.EndSend(ar);

        }
        catch { }
    }
    
    public void Dispose()
    {
        try
        {
            this.StopClient();
        }
        catch { }
    }

    public void ReStart()
    {
        this.threadSocket.Stop();
        try
        {
            this.threadSocket.Elapsed -= this.threadSocket_Receive;
        }
        catch { }
        try
        {
            this.threadSocket.Elapsed -= this.threadSocket_Connect;
        }
        catch { }
        this.ClientServer = null;
        this.CurrentState.Dispose();
        this.CurrentState = null;
        if (this.isRun) this.StartClient();
    }
}
