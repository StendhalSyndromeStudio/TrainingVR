using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StereoscopeEye : MonoBehaviour {
    public enum EyeType { Right, Left }

    public EyeType Type;

    public Camera Apple { get; private set; }
    // Use this for initialization

    public bool IsCamera ( ) {
        return Apple != null;
    }

    public void NotCameraLog ( ) {
        if ( !IsCamera( ) )
        {
            Debug.LogError( "An eye must be a camera!" );
        }
    }

	void Start () {
        Apple = GetComponent<Camera>( );
	}
	
	// Update is called once per frame
	void Update () {
        NotCameraLog( );

    }
}
