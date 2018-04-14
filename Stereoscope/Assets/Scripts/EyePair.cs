using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyePair : MonoBehaviour {
    [SerializeField]
    private StereoscopeEye _Left;

    [SerializeField]
    private StereoscopeEye _Right;

    public StereoscopeEye Left { get { return _Left; } }
    public StereoscopeEye Right { get { return _Right; } }

    private bool InitLeftEye ( ) {
        if ( _Left != null )
        {
            _Left.Type = StereoscopeEye.EyeType.Left;
            return true;
        }
        var leftTransform = transform.Find( "leftEye" );
        if ( leftTransform == null ) return false;
        var eye = leftTransform.gameObject.GetComponent<StereoscopeEye>( );
        if ( eye == null || eye.Type != StereoscopeEye.EyeType.Left ) return false;
        _Left = eye;
        return true;
    }

    private bool InitRightEye ( )
    {
        if ( _Right != null )
        {
            _Right.Type = StereoscopeEye.EyeType.Right;
            return true;
        }
        var rightTransform = transform.Find( "rightEye" );
        if ( rightTransform == null ) return false;
        var eye = rightTransform.gameObject.GetComponent<StereoscopeEye>( );
        if ( eye == null || eye.Type != StereoscopeEye.EyeType.Right ) return false;
        _Right = eye;
        return true;
    }

    private bool InitEyes ( ) {
        return InitLeftEye( ) && InitRightEye( );
    }

    // Use this for initialization
    void Start () {
        if ( !InitEyes( ) ) {
            Debug.LogError( "Can't init eyes! Make sure you are set left and right eyes" );
        }
        
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
