using System.Collections;
using System.Collections.Generic;
using System;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

/*
 * Class used to create and store multiple paths
 * Handles requests from multiple objects to path to a single object(target)
 */
public class PathRequestManager : MonoBehaviour
{
    [SerializeField]
    private Queue<PathRequest> pathRequestsQueue = new Queue<PathRequest>();    //Queue to store requests
    [SerializeField]
    private PathRequest currentPathRequest;

    [SerializeField] private Pathfinding pathfinding; // Reference to PF class 
    [SerializeField] private bool isProcessingPath; // are we processing a path?
    

    public static PathRequestManager instance;

    private void Awake()
    {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }

    public static void RequestPath(Vector2 pathStart, Vector2 pathEnd, Action<Vector2[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);     // Create the new request
        instance.pathRequestsQueue.Enqueue(newRequest);         // Queue it in the queue
        instance.TryProcessNext();
    }

    /*
     * Checks if we are currently processing or not
     */
    private void TryProcessNext()
    {
        if (!isProcessingPath && pathRequestsQueue.Count>0)
        {
            currentPathRequest = pathRequestsQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }
    }

    public void FinishedProcessingPath(Vector2[] path, bool success)
    {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }
    

    struct PathRequest
    {
        public Vector2 pathStart;
        public Vector2 pathEnd;
        public Action<Vector2[], bool> callback;

        public PathRequest(Vector2 _pathStart, Vector2 _pathEnd, Action<Vector2[], bool> _callback)
        {
            pathStart = _pathStart;
            pathEnd = _pathEnd;
            callback = _callback;
        }
    }
    
    
}
