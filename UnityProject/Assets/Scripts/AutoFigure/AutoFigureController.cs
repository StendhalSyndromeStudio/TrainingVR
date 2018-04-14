using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoFigureController : MonoBehaviour {
    [System.Serializable]
    public struct AutoFigureElement {
        public string name;
        public GameObject prefab;
    }
    
    public List<AutoFigureElement> _autoFigurePrefab;

    void OnInstanceAutoFigure( string name, string text = "" ) {
        foreach ( var element in _autoFigurePrefab ) {
            if ( name == element.name ) {
                var autoFigure = GameObject.Instantiate( element.prefab, new Vector3( 0, 0, 0 ), new Quaternion( ), this.transform );
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

    void Start( ) {
        OnInstanceAutoFigure( "стрелка", "1" );
        OnInstanceAutoFigure( "круг", "1" );
    }

    void Update( ) {
 
    }
}
