// capture ARcam from Vuforia and store in OpenCV mat

using UnityEngine;
using Vuforia;
using OpenCVForUnity;

public class ArCamCap : MonoBehaviour
{
    private bool mAccessCameraImage = true;
    [HideInInspector]
    public Mat currentFrame;

#if UNITY_EDITOR
    private Vuforia.Image.PIXEL_FORMAT mPixelFormat = Vuforia.Image.PIXEL_FORMAT.RGBA8888;
#else
    private Vuforia.Image.PIXEL_FORMAT mPixelFormat = Vuforia.Image.PIXEL_FORMAT.RGB888;
#endif

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
                    byte[] pixels = image.Pixels;
                    // Debug.Log("Height: " + image.Height + " Width: " + image.Width);
                    
                    if (pixels != null && pixels.Length > 0)
                    {
                        if (mPixelFormat == Vuforia.Image.PIXEL_FORMAT.RGBA8888)
                        {
                            currentFrame = new Mat(image.Height, image.Width, CvType.CV_8UC4);
                            currentFrame.put(0, 0, pixels);
                            Imgproc.cvtColor(currentFrame, currentFrame, Imgproc.COLOR_BGRA2RGBA);
                            // Debug.Log("current pixel format: RGBA8888");
                        }
                        else if (mPixelFormat == Vuforia.Image.PIXEL_FORMAT.RGB888)
                        {
                            currentFrame = new Mat(image.Height, image.Width, CvType.CV_8UC3);
                            currentFrame.put(0, 0, pixels);
                            Imgproc.cvtColor(currentFrame, currentFrame, Imgproc.COLOR_BGR2RGBA);
                            // Debug.Log("current pixel format: RGB888");
                        }
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
