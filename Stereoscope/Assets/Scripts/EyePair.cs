using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EyePair : MonoBehaviour {
    [SerializeField]
    private StereoscopeEye _Left;

    [SerializeField]
    private StereoscopeEye _Right;

    [SerializeField]
    private VRCursor _Cursor;

    public StereoscopeEye Left { get { return _Left; } }
    public StereoscopeEye Right { get { return _Right; } }

    public VRCursor Cursor { get { return _Cursor; } }

    private bool InitLeftEye ( ) {
        if ( _Left != null )
        {
            _Left.Type = StereoscopeEye.EyeType.Left;
            return true;
        }
        var leftTransform = transform.Find( "leftEye" );
        if ( leftTransform == null )
        {
            return false;
        }
        var eye = leftTransform.gameObject.GetComponent<StereoscopeEye>( );
        if ( eye == null || eye.Type != StereoscopeEye.EyeType.Left )
        {
            return false;
        }
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
        if ( rightTransform == null )
        {
            return false;
        }
        var eye = rightTransform.gameObject.GetComponent<StereoscopeEye>( );
        if ( eye == null || eye.Type != StereoscopeEye.EyeType.Right )
        {
            return false;
        }
        _Right = eye;
        return true;
    }

    private bool InitCursor ( )
    {
        if ( _Cursor != null )
        {
            return true;
        }
        var cursorTransform = transform.Find( "cursor" );
        if ( cursorTransform == null )
        {
            return false;
        }
        _Cursor = cursorTransform.gameObject.GetComponent<VRCursor>( );
        return _Cursor != null;
    }

    private bool InitEyes ( ) {
        return InitLeftEye( ) && InitRightEye( );
    }

    // Use this for initialization
    void Start () {
        if ( !InitEyes( ) ) {
            Debug.LogError( "Can't init eyes! Make sure you are set left and right eyes" );
        }

        if ( !InitCursor( ) ) {
            Debug.LogError( "Can't init cursor!" );
        }
     }
	
	// Update is called once per frame
	void Update () {
        if ( _Cursor == null ) {
            Debug.Log( "Cursor is null!" );
        }
        else {
            var target = _Cursor.Target;

            if ( target != null )
            {
                Debug.Log( target.name );
            }
            if ( Input.GetMouseButtonUp( 0 ) ) {
                Debug.Log( "MouseUp" );
                if ( target != null )
                {
                    var button = target.GetComponent<Button>( );
                    if ( button != null )
                    {
                        UnityEngine.EventSystems.PointerEventData data = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
                        button.OnPointerClick( data );
                        //button.onClick.Invoke( );
                    }
                }
                
            }
        }
        
	}
}
