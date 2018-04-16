// agent controller to follow path

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour {

    public Transform target;
    public float speed = 1.0f;

    private CharacterController charController;

    // astar
    private Planner planner;

    // move along the path
    int currentWayPoint;
    public float distanceTh = 0.1f;

    // Use this for initialization
    void Start ()
    {
        charController = GetComponent<CharacterController>();
        planner = GetComponent<Planner>();
        currentWayPoint = 0;
        // launch replan every 0.1s
        InvokeRepeating("Replan", 0.01f, 0.1f);
	}

    void Replan()
    {
        // replan when target changes position
        if (target.hasChanged)
        {
            Debug.Log("Target or CurrentPos Change! Target: " + target.position + "Current Pos: " + target.position);
            target.hasChanged = false;
            // replan the path
            Vector2 startVec2 = new Vector2(transform.position.x, transform.position.z);
            Vector2 targetVec2 = new Vector2(target.position.x, target.position.z);
            planner.LaunchPlanning(startVec2, targetVec2);
            // change current way point
            currentWayPoint = 0;
        }
    }

    private void FixedUpdate()
    {
        // no valid path: stay still
        if (planner.posPath.Length <= 0)    { return; }

        Debug.Log("Update!");
        // init
        if (currentWayPoint == 0) { currentWayPoint++; }
        Vector2 currPos2D = new Vector2(transform.position.x, transform.position.z);
        Vector2 currWay2D = planner.posPath[currentWayPoint];
        if (Vector2.Distance(currPos2D, currWay2D) <= distanceTh)
        {
            Debug.Log("Reach current way point!");
            currentWayPoint++;
        }
        Vector2 dir2D = currWay2D - currPos2D;
        
        Debug.Log("Index Path Length: " + planner.path.Length);
        Debug.Log("Pos Path Length: " + planner.posPath.Length);
        Debug.Log("currentWayPoint: " + currentWayPoint);

        Vector3 dir = new Vector3(dir2D.x, 0, dir2D.y);
        dir = dir.normalized;

        // reach the goal
        if (currentWayPoint == planner.posPath.Length - 1)
        {
            Debug.Log("CATCH!");
            return;
        }

        // move agent
        Vector3 velocity = speed * dir;
        charController.SimpleMove(velocity);
        // face direction
        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    /// ========================================= Visualization =========================================================

    // visualization
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        if (planner.posPath.Length <= 0) { return; }

        Gizmos.color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
        Vector2 currPos2D = new Vector2(transform.position.x, transform.position.z);
        Vector2 currWay2D = planner.posPath[currentWayPoint];
        if (Vector2.Distance(currPos2D, currWay2D) <= distanceTh)
        {
            currentWayPoint++;
        }
        Vector2 dir2D = currWay2D - currPos2D;

        Vector3 dir = new Vector3(dir2D.x, 0, dir2D.y);
        Vector3 from = new Vector3(currPos2D.x, planner.map.worldHeight, currPos2D.y);
        Vector3 to = from + dir;
        Vector3 curr = new Vector3(currWay2D.x, planner.map.worldHeight, currWay2D.y);

        Ray r = new Ray(from, dir);
        Gizmos.DrawRay(r);
        Gizmos.DrawSphere(to, 0.1f);
        Gizmos.color = new Color(1.0f, 0.0f, 1.0f, 1.0f);
        Gizmos.DrawSphere(curr, 0.1f);
        Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        Gizmos.DrawSphere(from, 0.1f);
     }
}
