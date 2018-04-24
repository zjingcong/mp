// generate grid map for astar

using UnityEngine;

[ExecuteInEditMode]
public class GridMapGen : MonoBehaviour {

    public float castHeight = 1f;
    [HideInInspector]
    public int heuIndex = 0;

    private Mesh mesh;
    private Vector3[] vertices;
    private bool[] walkableData;

    public GridMap gridMap;

    private void Awake()
    {
        gridMap = new GridMap();
        Scan();
        UpdateGridMap();
    }

    public void Scan()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        Mesh meshCopy = Mesh.Instantiate(mf.sharedMesh) as Mesh;
        mesh = mf.mesh = meshCopy;

        vertices = mesh.vertices;

        walkableData = new bool[vertices.Length];
        for (int i = 0; i < vertices.Length; ++i)
        {
            walkableData[i] = true;
            Vector3 origin = vertices[i];
            Vector3 down = new Vector3(0, -1, 0);
            if (Physics.Raycast(origin, down, castHeight))
            {
                walkableData[i] = false;
            }
        }
        Debug.Log("Construct Grid Map.");
    }

    public void UpdateGridMap()
    {
        GridGen gridGen = GetComponent<GridGen>();
        gridMap.SetGrid(ref gridGen);   // set up grid data
        gridMap.SetWalkable(ref walkableData);  // set up walkable data
        gridMap.heuristic_id = heuIndex;   // set up heuristic function
        Debug.Log("Update GridMap Data." + "Current Heuristic Function ID: " + gridMap.heuristic_id);
    }

    /// ========================================= Visualization =========================================================

    private void OnDrawGizmos()
    {
        // draw vertices
        for (int i = 0; i < vertices.Length; i++)
        {
            if (walkableData[i]) { Gizmos.color = new Color(0.0f, 1.0f, 0.0f, 0.5f); }
            else { Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.5f); }
            Gizmos.DrawSphere(vertices[i], 0.2f);
        }
    }
}
