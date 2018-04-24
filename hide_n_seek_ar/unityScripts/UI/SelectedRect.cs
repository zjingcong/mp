using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedRect : MonoBehaviour {
    // selection color
    public Color rectColor = Color.cyan;
    [HideInInspector]
    public bool selectionComplete = false;
    [HideInInspector]
    public Texture2D selectedImgTex;

    // gl pos render mat
    private Material lineMat;

    // status
    private Vector3 neg = new Vector3(-1, -1, -1);
    private Vector3 start;
    private Vector3 end;
    private bool drawRectangle = false;

    private void Start()
    {
        start = neg;
        end = neg;
    }

    void Update()
    {
        /// screen interaction
        // start
        if (Input.GetMouseButtonDown(0))
        {
            drawRectangle = true;
            start = Input.mousePosition;
            Debug.Log("DrawRectangle Start At: " + start);
        }
        // end
        else if (Input.GetMouseButtonUp(0))
        {
            drawRectangle = false;
            end = Input.mousePosition;
            Debug.Log("DrawRectangle End At: " + end);
        }
    }

    // show selected rectangle
    // only run if the script is attached to the camera and is enabled
    // Will be called from camera after regular rendering is done
    void OnPostRender()
    {
        if (!lineMat)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things. In this case, we just want to use
            // a blend mode that inverts destination colors.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMat = new Material(shader);
            lineMat.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn off backface culling, depth writes, depth test.
            lineMat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            lineMat.SetInt("_ZWrite", 0);
            lineMat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
        }

        if (drawRectangle)
        {
            Vector3 currEnd = Input.mousePosition;
            lineMat.SetPass(0);

            GL.PushMatrix();
            GL.LoadPixelMatrix();
            // draw quads
            GL.Begin(GL.QUADS);
            GL.Color(new Color(rectColor.r, rectColor.g, rectColor.b, 0.1f));
            GL.Vertex3(start.x, start.y, 0);
            GL.Vertex3(currEnd.x, start.y, 0);
            GL.Vertex3(currEnd.x, currEnd.y, 0);
            GL.Vertex3(start.x, currEnd.y, 0);
            GL.End();
            // draw lines
            GL.Begin(GL.LINES);
            GL.Color(rectColor);
            GL.Vertex3(start.x, start.y, 0);
            GL.Vertex3(currEnd.x, start.y, 0);
            GL.Vertex3(currEnd.x, start.y, 0);
            GL.Vertex3(currEnd.x, currEnd.y, 0);
            GL.Vertex3(currEnd.x, currEnd.y, 0);
            GL.Vertex3(start.x, currEnd.y, 0);
            GL.Vertex3(start.x, currEnd.y, 0);
            GL.Vertex3(start.x, start.y, 0);
            GL.End();
            GL.PopMatrix();
        }

        /// set completion status and capture area
        if (start != neg && end != neg)
        {
            // capture area
            RectCap();
            // reset
            start = neg;
            end = neg;
            // disable selection
            enabled = false;
        }
        else
        {
            Destroy(selectedImgTex);
            selectionComplete = false;
        }
    }

    // capture rectangle area pixels: must put under OnPostRender()
    void RectCap()
    {
        // start: lower left, end: upper right
        int startX, startY, endX, endY;
        // make sure start is less than end
        if (start.x > end.x)
        {
            startX = (int)end.x;
            endX = (int)start.x;
        }
        else
        {
            startX = (int)start.x;
            endX = (int)end.x;
        }
        if (start.y > end.y)
        {
            startY = (int)end.y;
            endY = (int)start.y;
        }
        else
        {
            startY = (int)start.y;
            endY = (int)end.y;
        }

        int width = endX - startX + 1;
        int height = endY - startY + 1;

        selectedImgTex = new Texture2D(width, height, TextureFormat.RGB24, false);

#if UNITY_EDITOR
        // unity editor
        // in GUI (0, 0) = top left corner; in Input.mousePosition (0, 0) = lower left corner
        // if rex = new Rect(startX, startY, width, height): vertically flip
        Rect rex = new Rect(startX, Screen.height - endY, width, height);
#else
        // uwp
        Rect rex = new Rect(startX, startY, width, height);
#endif

        // Debug.Log("startX: " + startX + " startY: " + startY + " endX: " + endX + " endY: " + endY);
        // Debug.Log("width: " + width + " height: " + height);
        // Debug.Log("texture size: " + selectedImgTex.width + "x" + selectedImgTex.height);
        // Debug.Log("screen size: " + Screen.width + "x" + Screen.height);

        selectedImgTex.ReadPixels(rex, 0, 0, false);
        selectedImgTex.Apply();

        // selection complete
        selectionComplete = true;
    }
}
