using PhotonNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AutoFigure : PhotonBehaviour, IAutoFigure, IPointerEnterHandler, IPointerExitHandler {
    private Canvas _canvas;
    private Text _text;
    void Awake( ) {
        this.transform.SetParent( AutoFigureController.Canvas, false );

        _canvas = this.GetComponentInParent<Canvas>( );
        _text = this.GetComponentInChildren<Text>( );
    }
	// Use this for initialization
	void Start ( ) {
		
	}

    public string text {
        set {
            if ( _text != null )
                _text.text = value;
        }
    }

    float _lastMouseX = 0;
    int _mod = 1;

	// Update is called once per frame
	void Update ( ) {
        if ( !_selected )
            return;
        if ( Input.GetMouseButton( 0 ) ) {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle( _canvas.transform as RectTransform, Input.mousePosition, _canvas.worldCamera, out pos );
            this.transform.position = _canvas.transform.TransformPoint( pos );
        }
        if ( Input.GetMouseButton( 1 ) ) {
            this.transform.Rotate( new Vector3( 0, 0, _mod * 2.5f ) );
        }

        if ( Input.GetMouseButton( 2 ) ) {
            Destroy( this.gameObject );
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

    public void OnPointerEnter( PointerEventData eventData ) {
        _selected = true;
    }

    public void OnPointerExit( PointerEventData eventData ) {
        _selected = false;
    }
}
