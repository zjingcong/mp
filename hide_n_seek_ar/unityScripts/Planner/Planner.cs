// A* Motion Planner

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Planner : MonoBehaviour{
    // inputs
    public GameObject astarObj;
    private Vector2 target2D;
    private Vector2 startPos2D;

    [HideInInspector]
    public GridMap map;
    private Vector2Int targetIndex;
    private Vector2Int startIndex;

    // results
    [HideInInspector]
    public Vector2Int[] path;   // path in index
    [HideInInspector]
    public Vector2[] posPath;   // path in world 2D position
    private bool isFind = false;

    // cost
    private float costStraight = 1.0f;
    private float costDiagonal = Mathf.Sqrt(2.0f);

    // ds used in astar
    struct AstarVoxel
    {
        public float Priority;
        public Vector2Int Index;

        public AstarVoxel(float p, Vector2Int i)
        {
            Priority = p;
            Index = i;
        }
    }
    private List<AstarVoxel> frontier;
    private float[, ] costSoFar;
    private Vector2Int[, ] cameFrom;

    private void Start()
    {
        map = astarObj.GetComponent<GridMapGen>().gridMap;
    }

    public void LaunchPlanning(Vector2 start, Vector2 target)
    {
        target2D = target;
        startPos2D = start;

        // init currentPos and targetPos
        InitPos();
        // astar
        Debug.Log("Current Heuristic Function ID: " + map.heuristic_id);
        InitMem();
        InitAstar();
        Debug.Log("Astar Initialization.");
        isFind = Plan();
        Debug.Log("Astar Planning Complete.");
        TraceBack();
        PathIndexToPos();
        Debug.Log("Path Recording Complete.");

        // Debug.Log("Memory used before collection: " + System.GC.GetTotalMemory(false));
        System.GC.Collect();
        // Debug.Log("Memory used after collection: " + System.GC.GetTotalMemory(true));
    }

    /// ========================================= Init =========================================================

    void InitPos()
    {
        targetIndex = map.PosToIndex(target2D);
        startIndex = map.PosToIndex(startPos2D);
        Debug.Log("Start Index: " + startIndex);
        Debug.Log("Target Index: " + targetIndex);
    }

    void InitMem()
    {
        frontier = new List<AstarVoxel>();
        cameFrom = new Vector2Int[map.xRes, map.yRes];
        costSoFar = new float[map.xRes, map.yRes];
    }

    void InitAstar()
    {
        // astar initialization
        // set up priority queue
        frontier.Add(new AstarVoxel(0.0f, startIndex));
        // set up history
        for (int j = 0; j < map.yRes; ++j)
        {
            for (int i = 0; i < map.xRes; ++i)
            {
                cameFrom[i, j] = new Vector2Int(-1, -1);
            }
        }
        // set up cost
        for (int j = 0; j < map.yRes; ++j)
        {
            for (int i = 0; i < map.xRes; ++i)
            {
                costSoFar[i, j] = -1.0f;
            }
        }
        costSoFar[startIndex.x, startIndex.y] = 0.0f;
    }

    /// ========================================= A* =========================================================

    // astar planning
    bool Plan()
    {
        if (!(targetIndex.x >= 0 && targetIndex.x < map.xRes && targetIndex.y >= 0 && targetIndex.y < map.yRes)) { return false; }
        if (!map.walkable[targetIndex.x, targetIndex.y]) { return false; }

        while (frontier.Count != 0)
        {
            // pop the one with the most low key value
            Vector2Int curr = RemoveMin(ref frontier).Index;

            // reach the goal
            if (curr == targetIndex)    { Debug.Log("Reach GOAL!");     return true; }

            // get the neighbouring voxels
            for (int i = -1; i <= 1; ++i)
            {
                for (int j = -1; j <= 1; ++j)
                {
                    // get next
                    int x = curr.x + i;
                    int y = curr.y + j;
                    Vector2Int next = new Vector2Int(x, y);
                    // set cost from current to next
                    float cost = costStraight;
                    if (i == j && i == -j) { cost = costDiagonal; }
                    // neighbouring voxel is in the grid and is not the original voxel
                    if ((!(i == 0 && j == 0)) && x >= 0 && x < map.xRes && y >= 0 && y < map.yRes && map.walkable[x, y])
                    {
                        float newCost = costSoFar[curr.x, curr.y] + cost;
                        if (costSoFar[x, y] == -1 || newCost < costSoFar[x, y])
                        {
                            costSoFar[x, y] = newCost;
                            float heuristic = map.HeuristicValue(next, targetIndex);
                            float priority = newCost + heuristic;
                            frontier.Add(new AstarVoxel(priority, next));
                            cameFrom[x, y] = curr;  // update history list
                        }
                    }
                }
            }
        }

        return false;
    }

    // trace back path
    void TraceBack()
    {
        ArrayList pathArrayList = new ArrayList();
        if (isFind)
        {
            pathArrayList.Add(targetIndex);
            Vector2Int curr = targetIndex;
            while (true)
            {
                Vector2Int pre = cameFrom[curr.x, curr.y];
                pathArrayList.Add(pre);
                // no valid path
                if (pre.x == -1)
                {
                    Debug.Log("No Path Finding.");
                    pathArrayList.Clear();
                    break;
                }
                // find path
                if (pre == startIndex)
                {
                    Debug.Log("Finding Path.");
                    break;
                }
                curr = pre;
            }
        }
        // reverse
        pathArrayList.Reverse();
        path = (Vector2Int[])pathArrayList.ToArray(typeof(Vector2Int));
    }

    // store world 2d pos path
    void PathIndexToPos()
    {
        posPath = new Vector2[path.Length];
        if (path.Length != 0)
        {
            for (int i = 0; i < path.Length; ++i)
            {
                Vector2Int ix = path[i];
                Vector2 pos = map.Index2PosCenter(ix);
                posPath[i] = pos;
            }
        }
    }

    // remove-min for frontier
    private AstarVoxel RemoveMin(ref List<AstarVoxel> list)
    {
        list = list.OrderBy(o => o.Priority).ToList<AstarVoxel>();
        AstarVoxel popElt = list[0];
        list.RemoveAt(0);
        return popElt;
    }

    /// ========================================= Visualization =========================================================

    // visualization
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        Gizmos.color = new Color(1.0f, 0.0f, 1.0f, 0.8f);
        for (int i = 0; i < path.Length - 1; i++)
        {
            Vector2Int fromIndex, toIndex;
            fromIndex = path[i];
            toIndex = path[i + 1];
            Vector2 from2D, to2D;
            from2D = map.Index2PosCenter(fromIndex);
            to2D = map.Index2PosCenter(toIndex);
            Gizmos.DrawLine(new Vector3(from2D.x, map.worldHeight, from2D.y), new Vector3(to2D.x, map.worldHeight, to2D.y));
        }
    }
}
