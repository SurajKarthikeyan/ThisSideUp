using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.PlayerLoop;


public class UNIT : MonoBehaviour
{
    public Transform target;
    public float speed;
    public Vector2[] path;
    public int targetIndex;
    public Vector3 lastUpdatedPos;

    public Vector3 curWaypoint = new Vector3(0f, 0f, 0f);
    private void Awake()
    {
        lastUpdatedPos = target.position;
    }

    /*
     * Request paths on Start
     */
    private void Start()
    {
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }

    private void Update()
    {
        if(target.position != lastUpdatedPos)   // check if the target has moved
        {
            if (path.Length != 0)    // last node exists
            {
    
                if (path.Last() == (Vector2)transform.position)
                {
                    
                    PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
                    lastUpdatedPos = transform.position;
                }
    
            }
            else
            {
                PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
                lastUpdatedPos = transform.position;
            }
        }
    }

    /*
     * Check if a path was found
     * If so, end previous coroutine
     * Start a new version of it
     */
    public void OnPathFound(Vector2[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
            targetIndex = 0;
        }
    }


    /*
     * Follow path coroutine
     */
    IEnumerator FollowPath()
    {

        Vector3 currentWaypoint = path[0];
        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }
            yield return null;
            curWaypoint = currentWaypoint;
            //transform.position = Vector2.MoveTowards(transform.position, currentWaypoint, speed);
        }
    }
    
    
    
    
    
////////////////////////////////////////////         ON DRAW GIZMO       ///////////////////////////////////////////////
    public void OnDrawGizmos() 
    {
        if (path != null) 
        {
            for (int i = targetIndex; i < path.Length; i ++) 
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector2.one);

                if (i == targetIndex) {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else {
                    Gizmos.DrawLine(path[i-1],path[i]);
                }
            }
        }
    }
}
