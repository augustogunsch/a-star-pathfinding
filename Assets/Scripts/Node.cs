using System;
using UnityEngine;

public class Node : IComparable, IHeapable
{
	public Vector3 WorldPosition;
	private Vector3 size;
	private LayerMask unwalkableMask;
	public bool Walkable;
	public int GridX;
	public int GridY;

	// Implementation of IHeapable
	public int Index { get; set; }

	// Previous node in the pathfinding path.
	public Node ParentNode;

	public int GCost;
	public int HCost;
	public int FCost
	{
		get
		{
			return GCost + HCost;
		}
	}

	/// <summary>
	/// Creates a new instance of Node.
	/// </summary>
	/// <param name="gridX">X position of the node on the grid.</param>
	/// <param name="gridY">Y position of the node on the grid.</param>
	/// <param name="worldPosition">World position of the node.</param>
	/// <param name="size">Size of the node.</param>
	/// <param name="unwalkableMask">Unwalkable layer mask.</param>
	public Node(int gridX, int gridY, Vector3 worldPosition, float size, LayerMask unwalkableMask)
	{
		WorldPosition = worldPosition;

		GridX = gridX;
		GridY = gridY;

		this.size = new Vector3(size, size, size);
		this.unwalkableMask = unwalkableMask;

		UpdateWalkable();
	}

	/// <summary>
	/// Compares for the lowest F cost with another Node, if they 
	/// are equal compares for the highest G cost.
	/// </summary>
	/// <param name="obj">Other node.</param>
	/// <returns>1 if higher priority, 0 if equal priority, -1 if lower priority.</returns>
	public int CompareTo(object obj)
	{
		try
		{
			Node other = obj as Node;

			int priority = other.FCost.CompareTo(FCost);

			// If both have same F cost, comparing for the highest G cost instead.
			if (priority == 0)
			{
				return GCost.CompareTo(other.GCost);
			}

			return priority;
		}
		catch (NullReferenceException)
		{
			throw new ArgumentException("The object provided is not derived from Node.");
		}
	}

	public override string ToString()
	{
		return $"[{GridX}, {GridY} - F({FCost}) = G({GCost}) + H({HCost})]";
	}

	/// <summary>
	/// Updates the walkable property of the node.
	/// </summary>
	public void UpdateWalkable()
	{
		// Checks if there is a collider (unwalkableMask) at the current position.
		if (!Physics.CheckBox(WorldPosition, size, Quaternion.identity,
			unwalkableMask))
		{
			Walkable = true;
		}
	}
}