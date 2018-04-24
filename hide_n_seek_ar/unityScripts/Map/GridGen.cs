// set up grid based on input resolutions and transform

using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class GridGen : MonoBehaviour {

    // grid resolution
    public int xRes, yRes;  // the number of grids
    public float unitsize;
    [HideInInspector]
    public Transform trans;

    private Mesh mesh;
    private Vector3[] vertices;

    private void Awake()
    {
        trans = GetComponent<Transform>();
    }

    private void Update()
    {
        Generate();
    }

    private void Generate()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";

        vertices = new Vector3[(xRes + 1) * (yRes + 1)];
        for (int j = 0, k = 0; j <= yRes; j++)
        {
            for (int i = 0; i <= xRes; i++, k++)
            {
                Vector3 vec = new Vector3(i * unitsize, 0, j * unitsize);
                vec += trans.position;
                vertices[k] = vec;
            }
        }
        mesh.vertices = vertices;
    }

    /// ========================================= Visualization =========================================================

    // visualization
    private void OnDrawGizmos()
    {
        if (vertices == null) { return; }
        if (vertices.Length <= 0) { return; }

        // draw lines
        Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        // draw horizontal lines
        for (int j = 0; j <= yRes; j++)
        {
            for (int i = 0; i < xRes; i++)
            {
                Vector3 from, to;
                from = vertices[j * (xRes + 1) + i];
                to = vertices[j * (xRes + 1) + i + 1];
                Gizmos.DrawLine(from, to);
            }
        }
        // draw verticle lines
        for (int j = 0; j < yRes; j++)
        {
            for (int i = 0; i <= xRes; i++)
            {
                Vector3 from, to;
                from = vertices[j * (xRes + 1) + i];
                to = vertices[(j + 1) * (xRes + 1) + i];
                Gizmos.DrawLine(from, to);
            }
        }
    }
}
