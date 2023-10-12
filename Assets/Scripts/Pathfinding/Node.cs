using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * Node class used for describing a node
 * Has FCost, GCost and HCost parameters
 */
public class Node : IHeapItem<Node>
{
    public bool isWalkable; // if the node is walkable
    public Vector2 worldPosition; // actual world pos of the node
    
    public int gridX;
    public int gridY;
    
    public int gCost;   // Cost of moving to the initial cell to the current(this) cell
    public int hCost;   // hurestic value, estimation of the cost to move from current(this) cell to the intended destination

    public Node parent;
    
    [SerializeField] 
    private int heapIndex;

    public int movementPenalty;

    
    /**
     * Create Node object through constructor and set local vars to inputted vars
     */
    public Node(bool _walkable, Vector2 _worldPosition, int _gridX, int _gridY, int _movementPenalty)
    {
        isWalkable = _walkable;
        worldPosition = _worldPosition;
        gridX = _gridX;
        gridY = _gridY;
        movementPenalty = _movementPenalty;
    }

    public int FCost {
        get {
            return gCost + hCost;
        }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Node compareNode)
    {
        int compare = FCost.CompareTo(compareNode.FCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(compareNode.hCost);
        }

        return -compare;
    }
}
