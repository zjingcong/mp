using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGetData : MonoBehaviour {

    public GameObject astarObj;
    public Transform target;
    public Transform currentPos;

    private GridMap map;
    private Vector2Int targetIndex;
    private Vector2Int currentPosIndex;

    // Use this for initialization
    void Start () {
        map = astarObj.GetComponent<GridMapGen>().gridMap;
        Vector2 target2D = new Vector2(target.position[0], target.position[2]);
        Vector2 currPos2D = new Vector2(currentPos.position[0], currentPos.position[2]);
        targetIndex = map.PosToIndex(target2D);
        currentPosIndex = map.PosToIndex(currPos2D);

        print(targetIndex);
        print(currentPosIndex);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
