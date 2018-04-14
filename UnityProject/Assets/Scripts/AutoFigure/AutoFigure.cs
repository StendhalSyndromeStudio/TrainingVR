using PhotonNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AutoFigure : PhotonBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler {
    public Canvas canvas;
    void Awake( ) {
        canvas = this.GetComponentInParent<Canvas>( );
    }
	// Use this for initialization
	void Start () {
		
	}

    float _lastMouseX = 0;
    int _mod = 1;

	// Update is called once per frame
	void Update ( ) {
        if ( !_selected )
            return;
        if ( Input.GetMouseButton( 0 ) ) {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle( canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out pos );
            this.transform.position = canvas.transform.TransformPoint( pos );
        }
        if ( Input.GetMouseButton( 1 ) ) {
            this.transform.Rotate( new Vector3( 0, 0, _mod * 5 ) );
        }

        if ( Input.GetAxis( "Mouse ScrollWheel" ) < 0 ) {
            this.transform.localScale += new Vector3( 0.1f, 0.1f, 0.1f );
        }
        if ( Input.GetAxis( "Mouse ScrollWheel" ) > 0 ) {
            this.transform.localScale -= new Vector3( 0.1f, 0.1f, 0.1f );
        }

        if ( _lastMouseX > Input.mousePosition.x )
            _mod = 1;
        if ( _lastMouseX < Input.mousePosition.x )
            _mod = -1;
        if ( _lastMouseX != Input.mousePosition.x ) {
            _lastMouseX = Input.mousePosition.x;
        }
    }

    private bool _selected = false;

    public void OnPointerDown( PointerEventData eventData ) { 
        //Debug.Log( "OnMouseDown" );
    }

    public void OnPointerUp( PointerEventData eventData ) {
        //Debug.Log( "OnMouseUp" );
    }

    public void OnPointerEnter( PointerEventData eventData ) {
        _selected = true;
    }

    public void OnPointerExit( PointerEventData eventData ) {
        _selected = false;
    }
}
