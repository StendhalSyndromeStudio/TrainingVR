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

    void OnInstanceAutoFigure( string name ) {
        foreach ( var element in _autoFigurePrefab ) {
            if ( name == element.name ) {
                var autoFigure = GameObject.Instantiate( element.prefab, new Vector3( 0, 0, 0 ), new Quaternion( ), this.transform );
                autoFigure.transform.localPosition = new Vector3( 0, 0, 0 );
                return;
            }
        }
    }

    void Start( ) {
        OnInstanceAutoFigure( "стрелка" );
    }

    void Update( ) {
 
    }
}
