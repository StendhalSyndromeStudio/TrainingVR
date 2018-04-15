using UnityEngine;
using System.Collections;


public interface IExternalDevices 
{        
    string ID
    {
        get;
    }

    void IncomingMessage(string message);
    void ReStart();
    void Stop();
    event MainSocketClient.delOutcomingMessage OutcomingMessage;
}
