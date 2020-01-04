using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour 
{

	public Transform seeker;
	public Transform target;
	Grid grid;

	private void Awake() 
	{
		grid = GetComponent<Grid> ();
	}

	private void Update() 
	{
		FindPath (seeker.position, target.position);
	}

	private void FindPath(Vector2 startPos, Vector2 targetPos) 
	{
		var startNode  = grid.NodeFromWorldPoint(startPos);
		var targetNode = grid.NodeFromWorldPoint(targetPos);

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
				RetracePath(startNode,targetNode);
				return;
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

	private void RetracePath(Node startNode, Node endNode) 
	{
		var path = new List<Node>();
		var currentNode = endNode;

		while (currentNode != startNode) 
		{
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		path.Reverse();
		grid.path = path;
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