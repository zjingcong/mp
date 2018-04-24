using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestRectSelectCap : MonoBehaviour {

    public RawImage rawImage;
    public GameObject ArCam;
    SelectedRect selectedRect;
    RectTransform rectXform;

    // Use this for initialization
    void Start () {
        selectedRect = ArCam.GetComponent<SelectedRect>();
        rectXform = rawImage.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update () {
        ShowSelection();
    }

    void ShowSelection()
    {
        if (selectedRect.selectionComplete)
        {
            Texture2D tex = selectedRect.selectedImgTex;
            rawImage.texture = tex;
            rawImage.material.mainTexture = tex;
            rectXform.sizeDelta = new Vector2(tex.width, tex.height);
            // byte[] bytes = selectedRect.selectedImgTex.EncodeToPNG();
            // System.IO.File.WriteAllBytes("D:\\dpa\\unity3d\\Hide_n_Seek_AR\\TestTmpFiles\\test_selected_cap.png", bytes);
        }
    }
}
