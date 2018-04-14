using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoFigureController : MonoBehaviour {

    public Canvas myCanvas;
    public Text text;

    void Start( ) {

    }

    public RectTransform prefab;

    Vector3 start;

    void Update( ) {
        var start = Camera.main.ScreenToWorldPoint( new Vector3( Input.mousePosition.x, Input.mousePosition.y, 0 ) );
        var stop = Camera.main.ScreenToWorldPoint( new Vector3( Input.mousePosition.x, Input.mousePosition.y, 45 ) );
        var ray = Camera.main.ScreenPointToRay( new Vector3( Input.mousePosition.x, Input.mousePosition.y, 45 ) );
        Debug.DrawRay( Camera.main.transform.position, ray.direction, Color.red, 100 );
        Debug.DrawLine( start, stop, Color.red );

        if ( Input.GetMouseButton( 0 ) ) {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle( myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.worldCamera, out pos );
            prefab.position = myCanvas.transform.TransformPoint( pos );
        }
        
        if ( Input.GetMouseButton( 1 ) ) {
            if ( ( Screen.width / 2 < Input.mousePosition.x ) ) {
                prefab.Rotate( new Vector3( 0, 0, 5 ) );
            }
            else {
                prefab.Rotate( new Vector3( 0, 0, -5 ) );
            }  
        }

        if ( Input.GetAxis( "Mouse ScrollWheel" ) < 0 ) {
            Debug.Log( "+" );
            prefab.localScale += new Vector3( 0.1f, 0.1f, 0.1f );
        }
        if ( Input.GetAxis( "Mouse ScrollWheel" ) > 0 ) {
            Debug.Log( "-" );
            prefab.localScale -= new Vector3( 0.1f, 0.1f, 0.1f );
        }
    }
}
