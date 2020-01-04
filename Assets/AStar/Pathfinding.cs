using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Pathfinding : MonoBehaviour 
{
	Grid grid;

	private void Awake() 
	{
		grid = GetComponent<Grid> ();
		PathRequestHandler.Initialize(this);
	}

	public void StartFindPath(Vector2 startPos, Vector2 targetPos)
	{
		StartCoroutine(FindPath(startPos, targetPos));
	}

	private IEnumerator FindPath(Vector2 startPos, Vector2 targetPos) 
	{
		var startNode  = grid.NodeFromWorldPoint(startPos);
		var targetNode = grid.NodeFromWorldPoint(targetPos);

		var waypoints = new Vector2[0];
		var success = false;

		if (startNode.walkable && targetNode.walkable)
		{
			var openSet   = new Heap<Node>(grid.MaxSize);
			var closedSet = new HashSet<Node>();

			openSet.Add(startNode);

			while (openSet.Count > 0) 
			{
				var currentNode = openSet.RemoveFirst();
				closedSet.Add(currentNode);

				//if target node found, exit from loop
				if (currentNode == targetNode) 
				{
					success = true;
					RetracePath(startNode,targetNode);
					break;
				}

				foreach (Node neighbour in grid.GetNeighbours(currentNode)) 
				{
					if (!neighbour.walkable || closedSet.Contains(neighbour)) 
					{
						continue;
					}

					int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
					if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) 
					{
						neighbour.gCost = newCostToNeighbour;
						neighbour.hCost = GetDistance(neighbour, targetNode);
						neighbour.parent = currentNode;

						if (!openSet.Contains(neighbour))
							openSet.Add(neighbour);
					}
				}
			}
		}
		
		yield return null;
		if (success)
		{
			waypoints = RetracePath(startNode,targetNode);
		}
		PathRequestHandler.FinishedProcessingPath(waypoints, success);
	}

    public void StartFindPath(Vector3 pathStart, Vector3 pathEnd)
    {
        throw new NotImplementedException();
    }

    Vector2[] RetracePath(Node startNode, Node endNode) 
	{
		var path = new List<Node>();
		var currentNode = endNode;

		while (currentNode != startNode) 
		{
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		var waypoints = SimplifyPath(path);
		Array.Reverse(waypoints);
		return waypoints;
	}

	Vector2[] SimplifyPath(List<Node> path)
	{
		var waypoints = new List<Vector2>();
		var dirOld = Vector2.zero;
		for (int i = 1; i < path.Count; i++)
		{
			var newDir = new Vector2(path[i-1].gridX - path[i].gridX, path[i-1].gridY - path[i].gridY);
			if (newDir != dirOld)
			{
				waypoints.Add(path[i].worldPosition);
			}
			dirOld = newDir;
		}
		return waypoints.ToArray();
	}

	private static int GetDistance(Node nodeA, Node nodeB) 
	{
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);
		return 14*dstX + 10 * (dstY-dstX);
	}
}