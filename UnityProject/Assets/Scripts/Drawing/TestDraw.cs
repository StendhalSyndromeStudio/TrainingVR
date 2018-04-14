using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestDraw : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    public GameObject prefab;

    public Text text;

    bool pressed = false;
	// Update is called once per frame
	void Update ( ) {
		if ( Input.GetMouseButton( 0 ) ) {
            if ( text != null ) {       
                text.text = Input.mousePosition.x + " " + Input.mousePosition.y + " " + Input.mousePosition.z;
            }
        }
        if ( Input.GetMouseButton( 1 ) ) {
            if ( prefab != null ) {
                GameObject.Instantiate( prefab, Input.mousePosition, new Quaternion( ) );
            }
        }
	}
}
