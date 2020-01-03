using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Transform target;
    public LayerMask unwalkableMask;
    public float nodeRadius;
    public Vector2 gridWorldSize;

    private Node[,] grid;

    private float nodeDiameter;
    private int gridSizeX;
    private int gridSizeY;

    private void Start() 
    {
        nodeDiameter = nodeRadius *2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);

        CreateGrid();
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

				grid[x,y] = new Node(walkable,worldPoint);
			}
		}
	}

    public Node NodeFromWorldPoint(Vector2 worldPosition) 
    {
        float percentX = (worldPosition.x - transform.position.x) / gridWorldSize.x + 0.5f - (nodeRadius / gridWorldSize.x);
        float percentY = (worldPosition.y - transform.position.y) / gridWorldSize.y + 0.5f - (nodeRadius / gridWorldSize.y);
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY-1) * percentY);

		return grid[x,y];
	}

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position,new Vector2(gridWorldSize.x,gridWorldSize.y));

        if (grid != null) 
        {
            var playerNode = NodeFromWorldPoint(target.position);

			foreach (Node n in grid) 
            {
				Gizmos.color = n.walkable ? Color.white : Color.red;

                if (n.Equals(playerNode))
                {
                    Gizmos.color = Color.cyan;
                }

				Gizmos.DrawSphere(n.worldPosition, nodeRadius);
			}
		}
    }
}

public class Node
{
    public bool walkable;
    public Vector2 worldPosition;

    public Node(bool walkable, Vector2 worldPosition)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
    }
}
