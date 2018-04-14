using PhotonNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AutoFigure : PhotonBehaviour, IAutoFigure, IPointerEnterHandler, IPointerExitHandler {
    #region --{IAutoFigure}--
    string IAutoFigure.text {
        set {
            this.text.Value = value;
        }
    }
    #endregion

    #region --[COMPONENT]--
    public Transform ControlTransform;

    private Canvas _canvas;
    private Text _text;
    #endregion

    [PhotonNetwork.Photon]
    private readonly UnityClientKSGT.EventValueType<string> text = new UnityClientKSGT.EventValueType<string>( String.Empty );

    float _lastMouseX = 0;

    int _mod = 1;

    private bool _selected = false;


    void Awake( ) {
        this.transform.SetParent( AutoFigureController.Canvas );
        _canvas = this.GetComponentInParent<Canvas>( );
        _text = this.GetComponentInChildren<Text>( );
    }

    void Start( ) {
        this.isOwner.AddingEvent( this, this.Owner_Change );
        this.Owner_Change( null, null );

        this.text.AddingEvent( this, this.Text_Change );
        this.Text_Change( null, null );
    }


    // Update is called once per frame
    void Update ( ) {
        if ( !_selected )
            return;
        if ( Input.GetMouseButton( 0 ) ) {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle( _canvas.transform as RectTransform, Input.mousePosition, _canvas.worldCamera, out pos );
            this.ControlTransform.position = _canvas.transform.TransformPoint( pos );
        }
        if ( Input.GetMouseButton( 1 ) ) {
            this.ControlTransform.Rotate( new Vector3( 0, 0, _mod * 2.5f ) );
        }

        if ( Input.GetMouseButton( 2 ) ) {
            PhotonNetwork.PhotonNetworkView.Destroy( this.gameObject );
        }

        if ( Input.GetAxis( "Mouse ScrollWheel" ) < 0 ) {
            this.ControlTransform.localScale += new Vector3( 0.1f, 0.1f, 0.1f );
        }
        if ( Input.GetAxis( "Mouse ScrollWheel" ) > 0 ) {
            this.ControlTransform.localScale -= new Vector3( 0.1f, 0.1f, 0.1f );
        }

        if ( _lastMouseX > Input.mousePosition.x )
            _mod = 1;
        if ( _lastMouseX < Input.mousePosition.x )
            _mod = -1;
        if ( _lastMouseX != Input.mousePosition.x ) {
            _lastMouseX = Input.mousePosition.x;
        }
    }


    public void OnPointerEnter( PointerEventData eventData ) {
        _selected = true;
    }

    public void OnPointerExit( PointerEventData eventData ) {
        _selected = false;
    }

    /// <summary>
    /// МЕТОД: Изменение текста
    /// </summary>
    private void Text_Change( object Sender, object Value ) {
        this._text.text = this.text.Value;
    }

    /// <summary>
    /// МЕТОД: Изменение хозяина
    /// </summary>
    /// <param name="Sender"></param>
    /// <param name="Value"></param>
    private void Owner_Change( object Sender, object Value ) {
        this.enabled = this.isOwner.Value == CodeOwner.@true;
    }
}
