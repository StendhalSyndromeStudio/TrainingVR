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

    private GameObject prevTarget;

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
        prevTarget = null;
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
            if ( prevTarget != null ) {
                //ISelectHandler, IDeselectHandler
                var buttons = prevTarget.GetComponents<UnityEngine.EventSystems.IDeselectHandler>( );
                if ( buttons != null )
                {
                    Debug.Log( "button != null" );

                    foreach ( var item in buttons )
                    {
                        var data = new UnityEngine.EventSystems.BaseEventData(UnityEngine.EventSystems.EventSystem.current);
                        item.OnDeselect( data );

                    }

                    //button.onClick.Invoke( );
                }
            }
            prevTarget = null;
        }
        else {
            var target = _Cursor.Target;

            if ( target != null )
            {
                //Debug.Log( target.name );
            }

            if ( prevTarget != null && target != prevTarget )
            {
                //ISelectHandler, IDeselectHandler
                var buttons = prevTarget.GetComponents<UnityEngine.EventSystems.IDeselectHandler>( );
                if ( buttons != null )
                {
                    Debug.Log( "button != null" );

                    foreach ( var item in buttons )
                    {
                        var data = new UnityEngine.EventSystems.BaseEventData(UnityEngine.EventSystems.EventSystem.current);
                        item.OnDeselect( data );

                    }

                    //button.onClick.Invoke( );
                }
            }

            if ( Input.GetMouseButtonDown( 0 ) ) {
                if ( target != null )
                {
                    Debug.Log( target.name );
                    var buttons = target.GetComponents<UnityEngine.EventSystems.IPointerDownHandler>( );
                    if ( buttons != null )
                    {
                        Debug.Log( "button != null" );

                        foreach ( var item in buttons )
                        {
                            var data = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
                            data.button = UnityEngine.EventSystems.PointerEventData.InputButton.Left;
                            var pos = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
                            data.position = pos;
                            data.pressPosition = pos;
                            data.selectedObject = target;
                            item.OnPointerDown( data );

                        }

                        //button.onClick.Invoke( );
                    }
                }
            }
            
            if ( Input.GetMouseButtonUp( 0 ) ) {
                Debug.Log( "MouseUp" );
                if ( target != null )
                {
                    Debug.Log( target.name );
                    var buttons = target.GetComponents<UnityEngine.EventSystems.IPointerClickHandler>( );
                    if ( buttons != null )
                    {
                        Debug.Log( "button != null" );
                        
                        foreach(var item in buttons )
                        {
                            var data = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
                            data.button = UnityEngine.EventSystems.PointerEventData.InputButton.Left;
                            var pos = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
                            data.position = pos;
                            data.pressPosition = pos;
                            data.selectedObject = target;
                            item.OnPointerClick( data );
                            
                        }
                        
                        //button.onClick.Invoke( );
                    }
                }
                if ( target != null )
                {
                    Debug.Log( target.name );
                    var buttons = target.GetComponents<UnityEngine.EventSystems.IPointerUpHandler>( );
                    if ( buttons != null )
                    {
                        Debug.Log( "button != null" );

                        foreach ( var item in buttons )
                        {
                            var data = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
                            data.button = UnityEngine.EventSystems.PointerEventData.InputButton.Left;
                            var pos = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
                            data.position = pos;
                            data.pressPosition = pos;
                            data.selectedObject = target;
                            item.OnPointerUp( data );

                        }

                        //button.onClick.Invoke( );
                    }
                }
            }


            if ( target != null && target != prevTarget )
            {
                var buttons = target.GetComponents<UnityEngine.EventSystems.ISelectHandler>( );
                if ( buttons != null )
                {
                    Debug.Log( "button != null" );

                    foreach ( var item in buttons )
                    {
                        var data = new UnityEngine.EventSystems.BaseEventData(UnityEngine.EventSystems.EventSystem.current);
                        data.selectedObject = target;
                        item.OnSelect( data );

                    }

                    //button.onClick.Invoke( );
                }
            }
            prevTarget = target;


        }
        
	}
}
