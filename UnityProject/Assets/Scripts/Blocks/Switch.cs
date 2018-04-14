using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Switch : MonoBehaviour {

    public event Action<int> onChangeSwitch;

    [System.Serializable]
    public struct Status {
        public string name;
        public Vector3 pos;
        public Vector3 rot;
    }

    public List<Status> status;

	// Use this for initialization
	void Start ( ) {
        this.transform.localPosition = status [ currentPos ].pos;
        this.transform.localRotation = Quaternion.Euler( status [ currentPos ].rot );
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    int currentPos = 0;

    void OnMouseUp( ) {
        Debug.Log( "Click" );
        currentPos = ( currentPos == 0 ) ? 1 : 0;
        this.transform.localPosition = status [ currentPos ].pos;
        this.transform.localRotation = Quaternion.Euler( status [ currentPos ].rot );
        if ( onChangeSwitch != null )
            onChangeSwitch( currentPos );
    }
}
