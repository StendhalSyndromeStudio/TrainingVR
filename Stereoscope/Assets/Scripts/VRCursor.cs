using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRCursor : MonoBehaviour {

    public float _distance;
    public Vector3 _scale;
    public GameObject Target {
        get {
            RaycastHit hitPoint;
            var ray = new Ray(transform.parent.position, transform.parent.forward);
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if ( Physics.Raycast( ray, out hitPoint, Mathf.Infinity ) )
            {
                if ( hitPoint.transform != null )
                {
                    if ( hitPoint.distance > 0 )
                    {
                        //Debug.Log( "_distance = " + _distance );
                        //Debug.Log( "hitPoint.distance = " + hitPoint.distance );
                        /*double diff = _distance - hitPoint.distance;
           
                        Debug.Log( "diff = " + diff );
                        var scaleDiff = diff / _distance; // + (diff >= 0 ? 1 : -1);
                        Debug.Log( "scaleDiff = " + scaleDiff );*/
                        _distance = hitPoint.distance;
                        //var scalediffVec = new Vector3(scaleDiff,scaleDiff,scaleDiff);

                        transform.localScale = _scale * _distance;
                        transform.position = transform.parent.position + transform.parent.forward * _distance;

                    }
                    else
                    {
                        //_distance = 10;
                        //var scalediffVec = new Vector3(scaleDiff,scaleDiff,scaleDiff);

                        transform.localScale = _scale * _distance;
                        transform.position = transform.parent.position + transform.parent.forward * _distance;
                    }


                    //transform.position = hitPoint.transform.position;
                }
                else
                {
                    //_distance = 10;
                    //var scalediffVec = new Vector3(scaleDiff,scaleDiff,scaleDiff);

                    transform.localScale = _scale * _distance;
                    transform.position = transform.parent.position + transform.parent.forward * _distance;
                }
                return hitPoint.transform.gameObject;
                /*if ( hitPoint.collider.tag == "Ground" )
                {
                    Debug.Log( "Hit ground" );
                }

                if ( hitPoint.collider.tag == "Object" )
                {
                    Debug.Log( "Hit object" );
                }*/
            }
            else {
                //_distance = 10;
                //var scalediffVec = new Vector3(scaleDiff,scaleDiff,scaleDiff);

                transform.localScale = _scale * _distance;
                transform.position = transform.parent.position + transform.parent.forward * _distance;
            }
            return null;
        }
    }
    // Use this for initialization

    private bool ValidScale ( ) {
        return ( _scale.x > 0 ) && ( _scale.y > 0 ) && ( _scale.z > 0 );
    }

    private bool ValidDistance ( )
    {
        return _distance > 0;
    }

    bool FixScale ( ) {
        if ( !ValidScale( ) )
        {

            _scale = transform.localScale / ( ValidDistance( ) ? _distance : Vector3.Distance( transform.parent.position, transform.position ) );
            return true;
        }
        return false;
    }

    bool FixDistance ( )
    {
        if ( !ValidDistance( ) )
        {
            _distance = Vector3.Distance( transform.parent.position, transform.position );
            return true;
        }
        return false;
    }

    void Start () {
        FixScale( );
        FixDistance( );

    }
	
	// Update is called once per frame
	void Update () {
        transform.position = transform.parent.position + transform.parent.forward * _distance;
	}
}
