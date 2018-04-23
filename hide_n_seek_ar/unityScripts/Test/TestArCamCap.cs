using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using OpenCvSharp;
// using OpenCvSharp.CPlusPlus;
using OpenCVForUnity;
using Vuforia;
using UnityEngine.UI;

public class TestArCamCap : MonoBehaviour
{

    private bool mAccessCameraImage = true;
    [HideInInspector]
    public Mat currentFrame;
    public RawImage rawImage;

    /*
        // The desired camera image pixel format
    #if UNITY_EDITOR
        private Vuforia.Image.PIXEL_FORMAT mPixelFormat = Vuforia.Image.PIXEL_FORMAT.RGBA8888;  // or GRAYSCALE
    #elif UNITY_ANDROID
       private Vuforia.Image.PIXEL_FORMAT mPixelFormat =  Vuforia.Image.PIXEL_FORMAT.RGB888;
    #elif UNITY_IOS
        private Vuforia.Image.PIXEL_FORMAT mPixelFormat =  Vuforia.Image.PIXEL_FORMAT.RGB888;
    #endif
    */
    private Vuforia.Image.PIXEL_FORMAT mPixelFormat = Vuforia.Image.PIXEL_FORMAT.RGBA8888;  // or GRAYSCALE

    // Boolean flag telling whether the pixel format has been registered
    private bool mFormatRegistered = false;
    void Start()
    {
        // Register Vuforia life-cycle callbacks:
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        VuforiaARController.Instance.RegisterOnPauseCallback(OnPause);
        VuforiaARController.Instance.RegisterTrackablesUpdatedCallback(OnTrackablesUpdated);
    }
    /// <summary>
    /// Called when Vuforia is started
    /// </summary>
    private void OnVuforiaStarted()
    {
        // Try register camera image format
        if (CameraDevice.Instance.SetFrameFormat(mPixelFormat, true))
        {
            Debug.Log("Successfully registered pixel format " + mPixelFormat.ToString());
            mFormatRegistered = true;
        }
        else
        {
            Debug.LogError("Failed to register pixel format " + mPixelFormat.ToString() +
                "\n the format may be unsupported by your device;" +
                "\n consider using a different pixel format.");
            mFormatRegistered = false;
        }
    }
    /// <summary>
    /// Called when app is paused / resumed
    /// </summary>
    private void OnPause(bool paused)
    {
        if (paused)
        {
            Debug.Log("App was paused");
            UnregisterFormat();
        }
        else
        {
            Debug.Log("App was resumed");
            RegisterFormat();
        }
    }
    /// <summary>
    /// Called each time the Vuforia state is updated
    /// </summary>
    private void OnTrackablesUpdated()
    {
        if (mFormatRegistered)
        {
            if (mAccessCameraImage)
            {
                Vuforia.Image image = CameraDevice.Instance.GetCameraImage(mPixelFormat);
                if (image != null)
                {
                    /*
                    string imageInfo = mPixelFormat + " image: \n";
                    imageInfo += " size: " + image.Width + " x " + image.Height + "\n";
                    imageInfo += " bufferSize: " + image.BufferWidth + " x " + image.BufferHeight + "\n";
                    imageInfo += " stride: " + image.Stride;
                    Debug.Log(imageInfo);
                    */
                    byte[] pixels = image.Pixels;
                    // Debug.Log("pixels length: " + pixels.Length);
                    if (pixels != null && pixels.Length > 0)
                    {
                        /*
                        Texture2D tex = new Texture2D(image.Width, image.Height, TextureFormat.RGB24, false);
                        image.CopyToTexture(tex);
                        rawImage.texture = tex;
                        rawImage.material.mainTexture = tex;
                        */

                        currentFrame = new Mat(image.Height, image.Width, CvType.CV_8UC4);
                        currentFrame.put(0, 0, pixels);
                        Texture2D tex = new Texture2D(image.Width, image.Height, TextureFormat.RGBA32, false);
                        Utils.matToTexture2D(currentFrame, tex);

                        rawImage.texture = tex;
                        rawImage.material.mainTexture = tex;

                        // Imgcodecs.imwrite("D:\\dpa\\unity3d\\Hide_n_Seek_AR\\TestTmpFiles\\test_frame_pre.png", currentFrame);
                        Imgproc.cvtColor(currentFrame, currentFrame, Imgproc.COLOR_BGRA2RGBA);
                        // Imgcodecs.imwrite("D:\\dpa\\unity3d\\Hide_n_Seek_AR\\TestTmpFiles\\test_frame_post.png", currentFrame);


                        /*
                        Texture2D tex = new Texture2D(image.Width, image.Height, TextureFormat.RGBA32, false);
                        tex.LoadRawTextureData(pixels);
                        tex.Apply();
                        rawImage.texture = tex;
                        rawImage.material.mainTexture = tex;
                        */

                        // Debug.Log("texture length: " + tex.GetRawTextureData().Length);

                        /*
                        Mat mat = new Mat(image.Height, image.Width, MatType.CV_8UC4, pixels);
                        Cv2.CvtColor(mat, mat, ColorConversion.BgraToRgb);
                        Cv2.ImShow("test", mat);
                        */
                    }
                }
            }
        }
    }
    /// <summary>
    /// Unregister the camera pixel format (e.g. call this when app is paused)
    /// </summary>
    private void UnregisterFormat()
    {
        Debug.Log("Unregistering camera pixel format " + mPixelFormat.ToString());
        CameraDevice.Instance.SetFrameFormat(mPixelFormat, false);
        mFormatRegistered = false;
    }
    /// <summary>
    /// Register the camera pixel format
    /// </summary>
    private void RegisterFormat()
    {
        if (CameraDevice.Instance.SetFrameFormat(mPixelFormat, true))
        {
            Debug.Log("Successfully registered camera pixel format " + mPixelFormat.ToString());
            mFormatRegistered = true;
        }
        else
        {
            Debug.LogError("Failed to register camera pixel format " + mPixelFormat.ToString());
            mFormatRegistered = false;
        }
    }
}
