using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Btn : MonoBehaviour {

    public event Action<bool> onPress;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnMouseDown( ) {
        if ( onPress != null )
            onPress( true );
    }

    void OnMouseUp( ) {
        if ( onPress != null ) {
            onPress( false );
        }
    }
}
