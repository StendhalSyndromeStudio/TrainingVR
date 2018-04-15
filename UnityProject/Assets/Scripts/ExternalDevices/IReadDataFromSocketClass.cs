using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

public interface IReadDataFromSocketClass {
    void ReadDataFromSocket(ref byte[] buffer, int startIndex, int count);
    void Update();
}

public class ReadDataFromSocketClass : IReadDataFromSocketClass {
    private interface IConvertHelper {
        int Size {
            get;
        }
        int ToSize(byte[] data);
    }
    private class ConvertHelper_UInt16 : IConvertHelper {
        public int Size {
            get {
                return sizeof(UInt16);
            }
        }

        public int ToSize(byte[] data) {
            return BitConverter.ToUInt16(data, 0);
        }
    }
    private class ConvertHelper_UInt32 : IConvertHelper {
        public int Size {
            get {
                return sizeof(UInt32);
            }
        }

        public int ToSize(byte[] data) {
            return (int)BitConverter.ToUInt32(data, 0);
        }
    }
    private class ConvertHelper_UInt64 : IConvertHelper {
        public int Size {
            get {
                return sizeof(UInt64);
            }
        }

        public int ToSize(byte[] data) {
            return (int)BitConverter.ToUInt64(data, 0);
        }
    }

    private delegate void HandlerReceiveBytesDelegate(AsynchronousClientExample.StateObject stateObject, byte[] bytes);

    private byte[] _bufferReceive = new byte[1024];
    private object lockerFor_bufferReceive = new object( );
    private byte[] bufferReceive {
        get {
            lock (this.lockerFor_bufferReceive) {
                return this._bufferReceive;
            }
        }
        set {
            lock (this.lockerFor_bufferReceive) {
                this._bufferReceive = value;
            }
        }
    }

    private byte[] _dataSize = new byte[sizeof(UInt16)];
    private int MaxSize = 1024 * 1024 * 1024;
    private int position;
    
    private string end_line;
    private AsynchronousClientExample Parent;
    private readonly Encoding m_Encoding;
    private IConvertHelper m_Convert;

    private readonly UnityClientKSGT.EventValueType<int> m_Length = new UnityClientKSGT.EventValueType<int>(0);
    private readonly UnityClientKSGT.EventValueType<int> m_Size = new UnityClientKSGT.EventValueType<int>(0);
    private readonly List<byte> m_MessageBuffer = new List<byte>( );
    private readonly List<byte> m_SizeBuffer = new List<byte>( );

    private readonly List<byte> _m_ReceiveBuffer = new List<byte>( );
    private readonly object lockerFor_m_ReceiveBuffer = new object( );
    private List<byte> ReceiveBuffer {
        get {
            lock (this.lockerFor_m_ReceiveBuffer) {
                return this._m_ReceiveBuffer;
            }
        }
    }

    public ReadDataFromSocketClass(
        AsynchronousClientExample parent,         
        string end_line,
        AsynchronousClientExample.EEncoding code,
        AsynchronousClientExample.TypeHelper typehHeader) {        
        this.end_line = end_line;
        this.Parent = parent;

        switch (code) {
            case AsynchronousClientExample.EEncoding.ASCII: {
                    this.m_Encoding = Encoding.ASCII;
                }
                break;
            case AsynchronousClientExample.EEncoding.BigEndianUnicode: {
                    this.m_Encoding = Encoding.BigEndianUnicode;
                }
                break;
            case AsynchronousClientExample.EEncoding.Default: {
                    this.m_Encoding = Encoding.Default;
                }
                break;
            case AsynchronousClientExample.EEncoding.Unicode: {
                    this.m_Encoding = Encoding.Unicode;
                }
                break;
            case AsynchronousClientExample.EEncoding.UTF32: {
                    this.m_Encoding = Encoding.UTF32;
                }
                break;
            case AsynchronousClientExample.EEncoding.UTF8: {
                    this.m_Encoding = Encoding.UTF8;
                }
                break;            
        }
        switch (typehHeader) {
            case AsynchronousClientExample.TypeHelper.UInt16: {
                    this.m_Convert = new ConvertHelper_UInt16( );
                }
                break;
            case AsynchronousClientExample.TypeHelper.UInt32: {
                    this.m_Convert = new ConvertHelper_UInt32( );
                }
                break;
            case AsynchronousClientExample.TypeHelper.UInt64: {
                    this.m_Convert = new ConvertHelper_UInt64( );
                }
                break;
        }        
        this.m_Length.AddingEvent(this, this.Length_Change);
        this.m_Size.AddingEvent(this, this.Size_Change);
    }

    private void Size_Change(object Sender, object Value) {
        if (this.m_Size.Value == this.m_Convert.Size) {
            byte[] dataSize = this.m_SizeBuffer.ToArray( );
            this.m_SizeBuffer.Clear( );
            Array.Reverse(dataSize);
            this.m_Length.Value = this.m_Convert.ToSize(dataSize);           
            this.m_Size.Value = 0;            
        }
    }

    private void HandlerReceiveBytes_Bytes(byte[] bytes) {
        try {
            if (this.Parent.IncomingBytes != null) {
                this.Parent.IncomingBytes(this.Parent, bytes);


            }
        } catch { }
    }

    private void HandlerReceiveBytes_String(byte[] bytes) {
        try {

            if (this.Parent.IncomingMessage == null) {
                return;
            }
            string message =this.m_Encoding.GetString(bytes, 0, bytes.Length);
            try {
                this.Parent.IncomingMessage(this.Parent, message);
            } catch { }
        } catch { }
    }

    private void Length_Change(object Sender, object Value) {
        if ((this.m_Length.Value == 0) && (this.m_MessageBuffer.Count != 0)) {            
            byte[] data = this.m_MessageBuffer.ToArray( );
            this.m_MessageBuffer.Clear( );
            this.HandlerReceiveBytes_String(data);
            this.HandlerReceiveBytes_Bytes(data);
        }
    }


    public void Update() {
        lock (this.lockerFor_m_ReceiveBuffer) {
            foreach (byte value in this._m_ReceiveBuffer) {
                this.Read(value);
            }            
            this._m_ReceiveBuffer.Clear( );
        }
    }

    private void Read(byte value) {
        if (this.m_Length.Value <= 0) {
            this.m_SizeBuffer.Add(value);
            this.m_Size.Value++;
        } else {
            this.m_MessageBuffer.Add(value);
            this.m_Length.Value--;
        }
    }
    
    public void ReadDataFromSocket(ref byte[] buffer, int startIndex, int receive_count) {
        this.ReceiveBuffer.AddRange(buffer);        
    }

    private string ByteToHex(byte[] buffer) {
        string[] array = buffer.Select((element) => {
            return element.ToString("X2");
        }).ToArray();
        return String.Join(", ", array);
    }

    private void ArrayRotateLeft(ref byte[] array) {
        for (int i = 1; i < array.Length; i++) {
            array[i - 1] = array[i];
        }
    }
}
