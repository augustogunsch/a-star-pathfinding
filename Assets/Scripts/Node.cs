using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Node
{
	public Vector3 WorldPosition;
	public Vector3 VectorSize;
	private float size;
	public bool Walkable;
	public int GridX;
	public int GridY;

	public Node(int gridX, int gridY, Vector3 worldPosition)
	{
		this.WorldPosition = worldPosition;
		size = NodeGrid.Instance.NodeSize;

		this.GridX = gridX;
		this.GridY = gridY;

		VectorSize = new Vector3(size, size, size);

		if (!Physics.CheckBox(WorldPosition, VectorSize, Quaternion.identity,
			NodeGrid.Instance.UnwalkableMask))
		{
			Walkable = true;
		}
	}

	public int GCost;
	public int HCost;
	public int FCost
	{
		get
		{
			return GCost + HCost;
		}
	}
	public Node ParentNode;

}