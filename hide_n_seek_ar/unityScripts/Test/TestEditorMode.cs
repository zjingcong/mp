using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TestEditorMode : MonoBehaviour {
    // Use this for initialization
    void Start () {
        if (Application.isEditor)   print("unity editor start");
        if (Application.isPlaying)  print("unity play mode start");
    }
	
	// Update is called once per frame
	void Update () {
        if (Application.isEditor)   print("unity editor update");
        if (Application.isPlaying)  print("unity play mode update");
    }
}
