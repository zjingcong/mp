using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestCap : MonoBehaviour {

    Texture2D tex;
    public RawImage rawImage;

	// Use this for initialization
	void Start () {
        var width = 80;
        var height = 100;
        var startX = 650;
        var startY = 440;

        tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        Rect rex = new Rect(0, 0, Screen.width, Screen.height);

        tex.ReadPixels(rex, 0, 0);
        tex.Apply();

        rawImage.texture = tex;
        rawImage.material.mainTexture = tex;

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
