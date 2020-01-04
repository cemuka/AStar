﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public bool displayGridGizmos;
    public LayerMask unwalkableMask;
    public float nodeRadius;
    public Vector2 gridWorldSize;

    private Node[,] grid;

    private float nodeDiameter;
    private int gridSizeX;
    private int gridSizeY;

    private void Awake() 
    {
        nodeDiameter = nodeRadius *2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);

        CreateGrid();
    }

    public int MaxSize 
    {
		get =>  gridSizeX * gridSizeY;
	}

    private void CreateGrid() 
    {
		grid = new Node[gridSizeX,gridSizeY];
		var worldBottomLeft = (Vector2)transform.position - Vector2.right * gridWorldSize.x/2 - Vector2.up * gridWorldSize.y/2;

		for (int x = 0; x < gridSizeX; x ++) {
			for (int y = 0; y < gridSizeY; y ++) {
				var worldPoint = worldBottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius) + Vector2.up * (y * nodeDiameter + nodeRadius);
                // if no collider2D is returned by overlap circle, then this node is walkable
				var walkable = (Physics2D.OverlapCircle(worldPoint,nodeRadius,unwalkableMask) == null);

				grid[x,y] = new Node(walkable, worldPoint, x, y);
			}
		}
	}

    	public List<Node> GetNeighbours(Node node) {
		List<Node> neighbours = new List<Node>();

		for (int x = -1; x <= 1; x++) 
        {
			for (int y = -1; y <= 1; y++) 
            {
				if (x == 0 && y == 0)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
					neighbours.Add(grid[checkX,checkY]);
				}
			}
		}

		return neighbours;
	}
	

	public Node NodeFromWorldPoint(Vector3 worldPosition) 
    {
		float percentX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
		float percentY = (worldPosition.y + gridWorldSize.y/2) / gridWorldSize.y;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY-1) * percentY);
		return grid[x,y];
	}

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position,new Vector2(gridWorldSize.x,gridWorldSize.y));

        if (grid != null && displayGridGizmos)
        {
            foreach (var n in grid)
            {
                Gizmos.color =    n.walkable ? Color.white : Color.black;
                Gizmos.DrawSphere(n.worldPosition, nodeRadius);
            }
        }
       
    }
}


