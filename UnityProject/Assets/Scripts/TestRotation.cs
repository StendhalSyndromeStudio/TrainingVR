using PhotonNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotation : PhotonBehaviour {


	// Use this for initialization
	void Start () {
        this.isOwner.AddingEvent( this, this.Onwer_Change );
        this.Onwer_Change( null, null );

    }

    private void Onwer_Change( object Sender, object Value ) {
        switch ( this.isOwner.Value ) {
            case CodeOwner.@true: {
                    this.StartCoroutine( this.Rotation( ) );
                }break;
        }
    }

    private IEnumerator Rotation( ) {
        while ( true ) {
            yield return new WaitForSeconds( 0.1f );
            this.transform.Rotate( new Vector3( 1, 1, 1 ), 10 );
        }
    }
    
}
