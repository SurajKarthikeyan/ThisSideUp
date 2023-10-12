using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using System;
using Unity.Jobs;


/**
 * Function in charge of finding the best path using A* algorithm
 */
public class Pathfinding : MonoBehaviour
{
    private Grid grid; 
    [SerializeField]
    private int diagonalCost;   //Cost of moving diagonally
    [SerializeField]
    private int directCost;     // Cost of moving directly(horizontal or vertical)

    [SerializeField] private int maxOpenSetSize;

    [SerializeField] private PathRequestManager requestManager;
    
    private void Awake()
    {
        grid = GetComponent<Grid>();
        requestManager = GetComponent<PathRequestManager>();
    }

    /**
     * Function to find a path to the target node
     * param: Vector2 startPos (starting position) and Vector2 targetPos(target or end position)
     * 
     */
    public void FindPath(Vector2 startPos, Vector2 targetPos)
    {
        Vector2[] wayPoints = new Vector2[0];
        bool pathSucceess = false;  // bool for success

        Node startNode = grid.NodeFromWorldPoint(startPos); //Create starting and ending nodes
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode.isWalkable && targetNode.isWalkable)
        {
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);      //Create data structure to store the open and closed sets for A*
            HashSet<Node> closedSet = new HashSet<Node>();

            openSet.AddToHeap(startNode);     // Add the starting node to the open set by default

            while (openSet.Count > 0)       //As long as the open set is not empty
            {
                Node currentNode = openSet.RemoveFirst();       // unexplored nodes
                closedSet.Add(currentNode);                     //Explored Nodes
                
                if (currentNode == targetNode)      // if we reached our target
                {
                    pathSucceess = true;
                    break;
                }

                foreach (Node neighbor in grid.GetNeighbors(currentNode))   //Get all neighbors and iterate
                {
                    if (!neighbor.isWalkable || closedSet.Contains(neighbor))   // if the neighbor node is not walkable or its in the closed set, dont bother with it
                    {
                        continue;
                    }

                    int movementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor) + neighbor.movementPenalty;    //Calculate new gCost of the neighbor node assuming current node is explored
                    if (movementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))     // if the new value is less than its current gCost or neighbor is not in the open set to begin with
                    {
                        neighbor.gCost = movementCostToNeighbor;        //Set the new gCost to the calculated value
                        neighbor.hCost = GetDistance(neighbor, targetNode);     //Set the hCost to the distance from it to the target node

                        neighbor.parent = currentNode; // Set the parent of the neighbor node to the current node

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.AddToHeap(neighbor);
                        }
                        else
                        {
                            openSet.UpdateItem(neighbor);
                        }
                    }
                }
            }
        }
        if (pathSucceess)
        {
           wayPoints = RetracePath(startNode, targetNode);
           pathSucceess = wayPoints.Length > 0;
        }
        
        requestManager.FinishedProcessingPath(wayPoints, pathSucceess);

    }

    public struct PathfindingStruct : IJob
    {
        public void Execute()
        {
            
        }
    }


    Vector2[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        Vector2[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;
    }

    Vector2[] SimplifyPath(List<Node> path)
    {
        List<Vector2> waypoints = new List<Vector2>();
        Vector2 directionOld = Vector2.zero;
        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i].worldPosition);
            }

            directionOld = directionNew;
        }

        return waypoints.ToArray();

    }
    

    /**
     *
     * Get the distance between two nodes based on diagonalCost and directCost
     * param: Node Alpha, Node Beta
     * Function takes into account the fact that you can move diagonally and horizontally/vertically
     */
    int GetDistance(Node Alpha, Node Beta)
    {
        int distanceX = Math.Abs(Alpha.gridX - Beta.gridX);
        int distanceY = Math.Abs(Alpha.gridY - Beta.gridY);
        //We can move diagonally by the lesser of the two and horizontally by the difference of the two to get to Beta
        if (distanceX > distanceY)
        {
            return diagonalCost * distanceY + directCost * (distanceX - distanceY);
        }
        else
        {
            return diagonalCost * distanceX + directCost * (distanceY - distanceX);

        }
    }

    /*
     * Start the find path
     */
    public void StartFindPath(Vector2 startPos, Vector2 targetPos)
    {
        FindPath(startPos, targetPos);
    }
}
