using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using PhotonNetwork;

[System.Serializable]
public class Switch : PhotonBehaviour {

    public event Action<int> onChangeSwitch;

    [System.Serializable]
    public struct Status {
        public string name;
        public Vector3 pos;
        public Vector3 rot;
    }

    public List<Status> status;


    private int currentPos = 0;

    [PhotonNetwork.Photon]
    private readonly UnityClientKSGT.EventValueType<int> _status = new UnityClientKSGT.EventValueType<int>( -1 );

    // Use this for initialization
    void Start( ) {
        this.isOwner.AddingEvent( this, this.Owner_Change );
        this._status.AddingEvent( this, this.Status_Change );

        this.Owner_Change( null, null );
        this._status.Value = this.currentPos;
    }

    
    void OnMouseUp( ) {
        if (this.isOwner.Value != CodeOwner.@true ) { return; }
        this._status.Value = ( this._status.Value == 0 ) ? 1 : 0;
    }

    /// <summary>
    /// МЕТОД: Изменение хозяина
    /// </summary>
    private void Owner_Change( object Sender, object Value ) {
        this._status.Remove( this, this.ChangeSwitch );
        switch ( this.isOwner.Value ) {
            case CodeOwner.@true: {
                    this._status.AddingEvent( this, this.ChangeSwitch );
                }break;
        }
    }

    /// <summary>
    /// МЕТОД: Изменение состояния
    /// </summary>
    private void Status_Change( object Sender, object Value ) {
        int currentPos = this._status.Value;
        this.transform.localPosition = status[ currentPos ].pos;
        this.transform.localRotation = Quaternion.Euler( status[ currentPos ].rot );
        
    }

    /// <summary>
    /// МЕТОД: Организация события "onChangeSwitch"
    /// </summary>
    private void ChangeSwitch(object sender, object value ) {
        int currentPos = ( int ) value;
        if ( onChangeSwitch != null )
            onChangeSwitch( currentPos );
    }

}
