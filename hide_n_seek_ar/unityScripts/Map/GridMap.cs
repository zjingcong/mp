// custom data structure to store grid information

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap
{
    // grid
    public int xRes, yRes;
    public float unitsize;
    public Vector2 llc;
    [HideInInspector]
    public float worldHeight;

    public int heuristic_id = 0;    // 0 - Euclidean, 1 - Manhattan, 2 - Diagonal
    public bool[, ] walkable;

    // use for heuristic value calculation
    private float D = 1.0f;
    private float D2 = Mathf.Sqrt(2.0f);

    public void SetGrid(ref GridGen grid)
    {
        xRes = grid.xRes;
        yRes = grid.yRes;
        llc = new Vector2(grid.trans.position[0], grid.trans.position[2]);
        unitsize = grid.unitsize;
        worldHeight = grid.trans.position[1];
    }

    public void SetWalkable(ref bool[] walkableData)
    {
        walkable = new bool[xRes + 1, yRes + 1];
        for (int j = 0, k = 0; j <= yRes; ++j)
        {
            for (int i = 0; i <= xRes; ++i, ++k)
            {
                walkable[i, j] = walkableData[k];
            }
        }
    }

    public Vector2Int PosToIndex(Vector2 pos)
    {
        Vector2 temp = pos - llc;
        int i = Mathf.FloorToInt(temp[0] / unitsize);
        int j = Mathf.FloorToInt(temp[1] / unitsize);
        Vector2Int index = new Vector2Int(i, j);
        return index;
    }

    public Vector2 IndexToPosLLC(Vector2Int index)
    {
        float x = index[0] * unitsize;
        float y = index[1] * unitsize;
        Vector2 pos = new Vector2(x, y) + llc;
        return pos;
    }

    public Vector2 Index2PosCenter(Vector2Int index)
    {
        float x = (index[0] + 0.5f) * unitsize;
        float y = (index[1] + 0.5f) * unitsize;
        Vector2 pos = new Vector2(x, y) + llc;
        return pos;
    }

    public float HeuristicValue(Vector2Int curr_index, Vector2Int target_index)
    {
        float value = 0;
        int dx, dy;
        switch (heuristic_id)
        {
            // Euclidean
            case 0:
                value = D * Vector2.Distance(curr_index, target_index);
                break;

            // Manhattan
            case 1:
                dx = Mathf.Abs(curr_index.x - target_index.x);
                dy = Mathf.Abs(curr_index.y - target_index.y);
                value = D * (dx + dy);
                break;

            // Diagonal
            case 2:
                dx = Mathf.Abs(curr_index.x - target_index.x);
                dy = Mathf.Abs(curr_index.y - target_index.y);
                // D=1, D2=2: Chebyshev distance; D=1, D2=sqrt(2): octile distance
                value = D * (dx + dy) + (D2 - 2 * D) * Mathf.Min(dx, dy);
                break;

            default:
                Debug.LogError("Unrecognized Option");
                break;
        }
        return value;
    }
}
