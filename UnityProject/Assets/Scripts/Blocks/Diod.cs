using PhotonNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Diod : PhotonBehaviour {

    [System.Serializable]
    public struct Status {
        public string name;
        public Color status;
    }

    public List<Status> status = new List<Status>( );
    Renderer _renderer;

    [PhotonNetwork.Photon]
    private readonly UnityClientKSGT.EventValueType<int> _status = new UnityClientKSGT.EventValueType<int>(-1);

    // Use this for initialization
    void Awake ( ) {
        _renderer = gameObject.GetComponent<Renderer>( );
    }

    void Start( ) {
        this._status.AddingEvent( this, this.Status_Change );
        onChangeStatus( 0 );
    }

    
    private void Status_Change( object Sender, object Value ) {
        if ( this._status.Value < status.Count ) {
            var value = status[ this._status.Value ];
            _renderer.material.color = value.status;
        }
    }

    public void onChangeStatus( int i ) {
        if ( this.isOwner.Value != CodeOwner.@true ) return;
        this._status.Value = i;
    }
}
