using PhotonNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AutoFigureController : PhotonBehaviour {
    [System.Serializable]
    public struct AutoFigureElement {
        public string Name;
        public string Resource;
    }
    
    public List<AutoFigureElement> _autoFigurePrefab;

    /// <summary>
    /// СВОЙСТВО: Родитель для автофигур
    /// </summary>
    public static Transform Canvas {
        get { return instance.transform; }
    }
    /// <summary>
    /// ПОЛЕ: Объект управления автофигурами
    /// </summary>
    private static AutoFigureController instance;

    void Awake( ) {
        AutoFigureController.instance = this;
    }

    void Start( ) {
        this.isOwner.AddingEvent( this, this.Owner_Change );
        this.Owner_Change( null, null );
    }

    private void Owner_Change( object Sender, object Value ) {
        switch ( this.isOwner.Value ) {
            case CodeOwner.@true: {
                    OnInstanceAutoFigure( "стрелка", "1" );
                    OnInstanceAutoFigure( "круг", "1" );
                }
                break;
        }
    }

    void OnInstanceAutoFigure( string name, string text = "" ) {
        foreach ( var element in _autoFigurePrefab ) {
            if ( name == element.Name ) {
                var autoFigure = PhotonNetwork.PhotonNetworkView.Instantiate( element.Resource, new Vector3( 0, 0, 0 ), new Quaternion( ) );
                autoFigure.transform.localPosition = new Vector3( 0, 0, 0 );
                var interfaceFigure = autoFigure.GetComponent<IAutoFigure>( );
                if ( interfaceFigure != null )
                    interfaceFigure.text = text;
                else
                    Debug.LogError( "empty auto figure interface" );
                return;
            }
        }
    }
}
