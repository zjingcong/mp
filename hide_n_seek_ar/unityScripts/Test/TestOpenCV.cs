
using UnityEngine;
using UnityEngine.UI;
using OpenCVForUnity;

public class TestOpenCV : MonoBehaviour {

    ArCamCap cap;
    public RawImage rawImage;
    Texture2D tex;
    Mat currFrame;

    private void Start()
    {
        cap = GetComponent<ArCamCap>();
    }

    private void Update()
    {
        if (cap.currentFrame == null) { return; }

        Size size = cap.currentFrame.size();
        int height = (int)size.height;
        int width = (int)size.width;
        // Debug.Log("height: " + height + " width: " + width);

        if (tex == null) { tex = new Texture2D(width, height, TextureFormat.RGBA32, false); }
        if (currFrame == null) { currFrame = new Mat(height, width, CvType.CV_8UC4); }

        Imgproc.cvtColor(cap.currentFrame, currFrame, Imgproc.COLOR_RGBA2BGRA);
        Utils.matToTexture2D(currFrame, tex);
        
        rawImage.texture = tex;
        rawImage.material.mainTexture = tex;
    }
}
