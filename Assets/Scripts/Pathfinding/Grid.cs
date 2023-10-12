using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


/**
 * Grid Class
 * Used to create and describe the grid used by nodes
 * For pathfinding using A*
 */
public class Grid : MonoBehaviour
{
    public LayerMask unwalkableMask;
    public LayerMask elevatedMask;
    public LayerMask aboveGroundMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    private Node[,] grid;
    private float nodeDiameter;
    private int gridSizeX, gridSizeY;
    
    public bool displayGridGizmo;
    [SerializeField] private int elevatedMovementPenalty = 10;


    private void Awake()
    {
        nodeDiameter = nodeRadius * 2; // Calculate diameter of node diameter
        
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter); // calculate x and y size of grid based on world size and node diameter
        
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid(); // Create grid function call
    }


    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }
    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        
        Vector2 worldBottomLeft = new Vector2(transform.position.x - Vector2.right.x * gridWorldSize.x / 2,
            transform.position.y - Vector2.up.y * gridWorldSize.y / 2); // Calculate bottom left corner of the world
        
        for (int i = 0; i < gridSizeX; i++) // Iterate over the x and y points to get xy coordiantes
        {
            for (int k = 0; k < gridSizeY; k++)
            {
                Vector2 worldPoint = worldBottomLeft + Vector2.right * (i * nodeDiameter + nodeRadius) +
                                     Vector2.up * (k * nodeDiameter + nodeRadius); // calculate world point of the node
                
                bool walkable = !(Physics2D.OverlapCircle(worldPoint, nodeRadius,unwalkableMask)); // check if its walkable based on layer mask
                bool elevated = Physics2D.OverlapCircle(worldPoint, nodeRadius, elevatedMask);
                bool aboveGround = Physics2D.OverlapCircle(worldPoint, nodeRadius, aboveGroundMask);
                int movementPenaltyConstructor = 0; // movement penalty value to be set and reset, fed into Node Constructor
                if ((elevated && walkable) || (elevated && aboveGround))
                {
                    movementPenaltyConstructor = elevatedMovementPenalty;
                }

                grid[i, k] = new Node(walkable, worldPoint, i, k, movementPenaltyConstructor); // set the node to walkable or unwalkable based on previous bool
            }
        }
        

    }

    /*
     * Function to return all the neighboring nodes
     * Returns a List<Node> data structure
     * params: Node checkNode - the node we want to check the nieghbors of
     */
    public List<Node> GetNeighbors(Node checkNode)
    {
        List<Node> neighbors = new List<Node>();    //Create data structure to return
        for (int i = -1; i <= 1; i++)
        {
            for (int k = -1; k <= 1; k++)        // iterate in a 3x3 pattern for both x and y
            {
                if (k == 0 && i == 0)       // if both are 0, just continue as that is checkNode
                {
                    continue;
                }

                int checkX = checkNode.gridX + i;       // using the grid location from node object and offset from loop
                int checkY = checkNode.gridY + k;       // set the indices to search in the grid

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    Node nodeToAdd = grid[checkX, checkY];
                    neighbors.Add(nodeToAdd);
                }
            }
        }

        return neighbors;
    }
    
    
    public Node NodeFromWorldPoint(Vector2 worldPosition)
    {
        float percentX = (worldPosition.x - transform.position.x + gridWorldSize.x / 2) / gridWorldSize.x;  // Calculate world position into percentages
        float percentY = (worldPosition.y - transform.position.y + gridWorldSize.y / 2) / gridWorldSize.y;  // Percentage of how far along the grid it is

        percentX = Mathf.Clamp01(percentX); // Clamp percentages
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);   // Get the indices, '-1' is to keep within bounds and multiply by percent to get how far along the way
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);   // Rounded to integers to get int indicies

        return grid[x, y];  // return Node at that index

    }

    
    /*
     * On Draw Gizmos Stuff
     */
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, gridWorldSize);
        if (grid != null && displayGridGizmo)
        { 
            foreach (Node n in grid)
            {
                Gizmos.color = (n.isWalkable) ? Color.white : Color.red;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }
}

