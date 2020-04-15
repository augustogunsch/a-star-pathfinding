using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
	public NodeGrid Grid;

	internal void RequestPath(Vector3 from, Vector3 to, Action<Vector3[], bool> callback)
	{
		StartCoroutine(FindPath(from, to, callback));
	}
	/// <summary>
	/// Finds a path between two points on the grid.
	/// </summary>
	/// <param name="from">initial position</param>
	/// <param name="to">target position</param>
	/// <returns>path array</returns>
	private IEnumerator FindPath(Vector3 from, Vector3 to, Action<Vector3[], bool> callback)
	{
		/* F cost = G cost + H cost
		 * G cost = distance from origin to node
		 * H cost = distance from node to target */

		// Open set => Nodes that are yet to be evaluated.
		BinaryHeap<Node> openSet = new BinaryHeap<Node>(Grid.NodeCount);
		// Closes set => Nodes that have been claimed by another.
		HashSet<Node> closedSet = new HashSet<Node>();

		Node initialNode = Grid.GetNodeFromWorldPosition(from);
		Node targetNode = Grid.GetNodeFromWorldPosition(to);

		// The initial node also must pass through the first iteration.
		//initialNode.HCost = GetDistance(initialNode, targetNode);
		openSet.Add(initialNode);

		while (openSet.Count > 0)
		{
			// Removing and getting the node with the lowest F cost (highest G cost in ties).
			Node currentNode = openSet.RemoveFirst();
			closedSet.Add(currentNode);

			// Returning the path if the node is the target.
			if (currentNode.Equals(targetNode))
			{
				callback.Invoke(RetracePath(initialNode, targetNode), true);
				yield break;
			}

			// Getting the surrounding nodes.
			Node[] neighbors = Grid.GetNeighbors(currentNode);

			// Deciding what to do with each neighbor.
			foreach (var neighbor in neighbors)
			{
				if (!neighbor.Walkable)
					continue;
				if (closedSet.Contains(neighbor))
					continue;

				/* newGCost refers to the new hypothetical G cost of the neighbor 
				 (it could have been assigned by another node and now be lower by this path).*/
				int newGCost = GetDistance(currentNode, neighbor) + currentNode.GCost + 
					neighbor.Weight;
				bool openSetHasNotNeighbor = !openSet.Contains(neighbor);
				if (openSetHasNotNeighbor || newGCost < neighbor.GCost)
				{
					// Parent node is used to retrace the path once it has been found.
					neighbor.ParentNode = currentNode;
					neighbor.GCost = newGCost;
					neighbor.HCost = GetDistance(neighbor, targetNode);

					if (openSetHasNotNeighbor)
						openSet.Add(neighbor);
					else
					{
						/* The reason for this line is that if the neighbor is already in the
						open set, it certainly got its G cost lowered and must me sorted
						up in the binary heap.*/
						openSet.SortUp(neighbor);
					}
				}
			}
		}
		yield return null;
		callback.Invoke(null, false);
	}

	private static int GetDistance(Node origin, Node target)
	{
		int xDistance = Math.Abs(origin.GridX - target.GridX);
		int yDistance = Math.Abs(origin.GridY - target.GridY);

		int distance;

		// (heuristics) using the formula min(x, y) * 14 + 10 * max(x, y) to get the distance.
		if (xDistance > yDistance)
			distance = 14 * yDistance + 10 * (xDistance - yDistance);
		else
			distance = 14 * xDistance + 10 * (yDistance - xDistance);

		return distance;
	}

	private static Vector3[] RetracePath(Node startNode, Node currentNode)
	{
		List<Node> path = new List<Node>();
		do
		{
			path.Add(currentNode);
			currentNode = currentNode.ParentNode;
		}
		while (!currentNode.Equals(startNode));

		Vector3[] points = SimplifyPath(path);

		Array.Reverse(points);
		return points;
	}

	private static Vector3[] SimplifyPath(List<Node> nodePath)
	{
		List<Vector3> pointPath = new List<Vector3>();
		Vector3 previousDirection = Vector3.zero;

		for(int i = 1; i < nodePath.Count; i++)
		{
			Vector3 thisDirection = nodePath[i].WorldPosition - nodePath[i - 1].WorldPosition;
			if(thisDirection != previousDirection)
			{
				pointPath.Add(nodePath[i].WorldPosition);
			}
			previousDirection = thisDirection;
		}
		return pointPath.ToArray();
	}
}
