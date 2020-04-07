using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Pathfinding
{
	/// <summary>
	/// Finds a path between two points on the grid.
	/// </summary>
	/// <param name="from">initial position</param>
	/// <param name="to">target position</param>
	/// <returns>path array</returns>
	public static Node[] FindPath(Vector3 from, Vector3 to)
	{
		List<Node> openSet = new List<Node>();
		HashSet<Node> closedSet = new HashSet<Node>();
		Node initialNode = NodeGrid.Instance.GetNodeFromWorldPosition(from);
		Node targetNode = NodeGrid.Instance.GetNodeFromWorldPosition(to);
		initialNode.HCost = GetDistance(initialNode, targetNode);
		openSet.Add(initialNode);
		while(openSet.Count > 0)
		{
			Node currentNode = openSet[0];

			foreach(var node in openSet)
				if (node.FCost < currentNode.FCost)
					currentNode = node;

			openSet.Remove(currentNode);
			closedSet.Add(currentNode);

			if(currentNode.Equals(targetNode))
			{
				return RetracePath(initialNode, targetNode);
			}

			Node[] neighbours = NodeGrid.Instance.GetNeighbours(currentNode);
			foreach(var neighbour in neighbours)
			{
				if (!neighbour.Walkable)
					continue;
				if (closedSet.Contains(neighbour))
					continue;

				int newGCost = GetDistance(currentNode, neighbour) + currentNode.GCost;
				bool openSetHasNotNeighbour = !openSet.Contains(neighbour);
				if (openSetHasNotNeighbour || newGCost < neighbour.GCost)
				{
					if (openSetHasNotNeighbour)
						openSet.Add(neighbour);

					neighbour.ParentNode = currentNode;
					neighbour.GCost = newGCost;
					neighbour.HCost = GetDistance(neighbour, targetNode);
				}
			}
		}
		throw new NoPathException($"No path between positions {from} and {to}.");
	}

	private static int GetDistance(Node origin, Node target)
	{
		int xDistance = Math.Abs(origin.GridX - target.GridX);
		int yDistance = Math.Abs(origin.GridY - target.GridY);

		int distance;

		if(xDistance > yDistance)
			distance = 14 * yDistance + 10 * (xDistance - yDistance);
		else
			distance = 14 * xDistance + 10 * (yDistance - xDistance);

		return distance;
	}

	private static Node[] RetracePath(Node startNode, Node currentNode)
	{
		List<Node> path = new List<Node>();
		do
		{
			path.Add(currentNode);
			currentNode = currentNode.ParentNode;
		}
		while (!currentNode.Equals(startNode));
		path.Reverse();
		return path.ToArray();
	}
}
