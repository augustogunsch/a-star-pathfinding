using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Node grid for the pathfinding system.
/// </summary>
public class NodeGrid : MonoBehaviour
{

	[SerializeField]
	private float nodeSize = 1;
	private float halfNodeSize;
	public float NodeSize { get { return nodeSize; } }
	public float HalfNodeSize { get { return halfNodeSize; } }
	public int NodeCount { get { return gridSizeX * gridSizeY; } }

	private Transform terrainTransform;

	private Node[,] nodes;

	public LayerMask UnwalkableMask;

	private int gridSizeX;
	private int gridSizeY;

	private float worldXSize;
	private float worldZSize;

	private float worldXCorner;
	private float worldY;
	private float worldZCorner;
	private Vector3 worldGridCorner
	{
		get
		{
			return new Vector3(worldXCorner, worldY, worldZCorner);
		}
	}

	private void Awake()
	{
		terrainTransform = gameObject.GetComponent<Transform>();

		halfNodeSize = NodeSize / 2;

		worldXSize = terrainTransform.lossyScale.x;
		worldZSize = terrainTransform.lossyScale.z;

		gridSizeX = (int)(worldXSize / nodeSize);
		gridSizeY = (int)(worldZSize / nodeSize);

		worldXCorner = terrainTransform.position.x - terrainTransform.lossyScale.x / 2;
		worldY = terrainTransform.lossyScale.y + terrainTransform.position.y;
		worldZCorner = terrainTransform.position.z - terrainTransform.lossyScale.z / 2;

		DrawGrid();
	}

	private void DrawGrid()
	{
		nodes = new Node[gridSizeX, gridSizeY];

		// Creating each node.
		for (int x = 0; x < gridSizeX; x++)
		{
			for (int y = 0; y < gridSizeY; y++)
			{
				nodes[x, y] = new Node(x, y, CalcNodePosition(x, y), nodeSize, UnwalkableMask);
			}
		}
	}

	/// <summary>
	/// Gets the nearest node from a world position.
	/// </summary>
	/// <param name="position">Position to be evaluated.</param>
	/// <returns>Nearest node.</returns>
	public Node GetNodeFromWorldPosition(Vector3 position)
	{
		// Compesates the difference of the grid world position.
		Vector3 localPosition = (position - worldGridCorner);

		// Adjusts the scale.
		localPosition /= nodeSize;

		// Places the position inside the grid in case it is not.
		int x = Mathf.Clamp((int)localPosition.x, 0, (int)gridSizeX - 1);
		int y = Mathf.Clamp((int)localPosition.z, 0, (int)gridSizeY - 1);

		return nodes[x, y];
	}

	private Vector3 CalcNodePosition(int gridX, int gridY)
	{
		Vector3 position = new Vector3(gridX * nodeSize, 0, gridY * nodeSize);

		// Adding compesation for the difference in coordinates.
		position += worldGridCorner;

		// Aligning node.
		position += new Vector3(HalfNodeSize, 0, HalfNodeSize);

		return position;
	}

	/// <summary>
	/// Gets all the surrounding nodes of a node.
	/// </summary>
	/// <param name="node">Node to have its neighbors taken.</param>
	/// <returns>Neighbors of node.</returns>
	public Node[] GetNeighbors(Node node)
	{
		List<Node> neighbours = new List<Node>();
		for (int x = node.GridX - 1; x <= node.GridX + 1; x++)
		{
			for (int y = node.GridY - 1; y <= node.GridY + 1; y++)
			{
				if (x >= 0 && x < gridSizeX &&
					y >= 0 && y < gridSizeY &&
					x + y != 0)
					neighbours.Add(nodes[x, y]);
			}
		}
		return neighbours.ToArray();
	}

	// DEBUG: draws the grid as gizmos once the game starts.
	//public void OnDrawGizmos()
	//{
	//    if(nodes != null)
	//    {
	//        Gizmos.color = Color.black;
	//        foreach(var node in nodes)
	//        {
	//            Gizmos.DrawWireCube(node.WorldPosition + Vector3.up * 4, Vector3.one * nodeSize);
	//        }
	//    }
	//}
}
