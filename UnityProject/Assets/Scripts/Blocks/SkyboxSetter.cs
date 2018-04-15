using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxSetter : MonoBehaviour {
    public Material skyboxMaterial;
	// Use this for initialization
	void Start () {
        RenderSettings.skybox = skyboxMaterial;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
