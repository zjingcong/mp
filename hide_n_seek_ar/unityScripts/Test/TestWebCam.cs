using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using OpenCvSharp.CPlusPlus;
// using OpenCvSharp;

public class TestWebCam : MonoBehaviour {

    WebCamTexture webCamTexture;
    [HideInInspector]
   //  public Mat currentFrame;

    void Start()
    {
        webCamTexture = new WebCamTexture();
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = webCamTexture;
        webCamTexture.Play();
    }

    void Update()
    {

        // convert WebCamTexture to Texture2D
        Texture2D tex = new Texture2D(webCamTexture.width, webCamTexture.height);
        tex.SetPixels(webCamTexture.GetPixels());
        tex.Apply();

    /*
        Mat mat = new Mat(webCamTexture.height, webCamTexture.width, MatType.CV_8UC4, tex.GetRawTextureData());
        Cv2.CvtColor(mat, mat, ColorConversion.BgraToRgb);
        currentFrame = mat;

        if (currentFrame == null) { return; }
        Cv2.ImShow("test", currentFrame);
    */
    }
}
