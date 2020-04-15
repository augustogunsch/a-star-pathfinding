using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Node grid for the pathfinding system.
/// </summary>
public class NodeGrid : MonoBehaviour
{
	public bool DrawGridGizmos;
	[SerializeField]
	private float nodeSize = 1;
	private float halfNodeSize;
	public float NodeSize { get { return nodeSize; } }
	public float HalfNodeSize { get { return halfNodeSize; } }
	public int NodeCount { get { return gridSizeX * gridSizeY; } }

	private Transform terrainTransform;

	private Node[,] nodes;

	public LayerMask UnwalkableMask;
	[SerializeField]
	public WalkableMask[] WalkableMasks;

	private int gridSizeX;
	private int gridSizeY;

	private float worldXSize;
	private float worldZSize;

	private float worldXCorner;
	private float worldY;
	private float worldZCorner;

	public int BrushSize;

	private Vector3 WorldGridCorner
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

	private void Start()
	{
		foreach (var penalty in WalkableMasks)
		{
			if (penalty.MovementPenalty > largestPenalty)
				largestPenalty = penalty.MovementPenalty;
		}
	}

	private void DrawGrid()
	{
		nodes = new Node[gridSizeX, gridSizeY];

		// Creating each node.
		for (int x = 0; x < gridSizeX; x++)
		{
			for (int y = 0; y < gridSizeY; y++)
			{
				Vector3 position = CalcNodePosition(x, y);
				int movementPenalty = GetMovementPenalty(position);
				nodes[x, y] = new Node(x, y, position, nodeSize, UnwalkableMask, movementPenalty);
			}
		}

		BlurWeights(BrushSize);
	}

	/// <summary>
	/// Gets the nearest node from a world position.
	/// </summary>
	/// <param name="position">Position to be evaluated.</param>
	/// <returns>Nearest node.</returns>
	public Node GetNodeFromWorldPosition(Vector3 position)
	{
		// Compesates the difference of the grid world position.
		Vector3 localPosition = (position - WorldGridCorner);

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
		position += WorldGridCorner;

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

	private int largestPenalty = 0;
	public void OnDrawGizmos()
	{
		if (nodes != null && DrawGridGizmos)
		{
			foreach (var node in nodes)
			{
				if (!node.Walkable)
				{
					Gizmos.color = new Color(1, 0, 0, 0.8f);
				}
				else
				{
					Gizmos.color = Color.Lerp(Color.white, Color.black,
						Mathf.InverseLerp(0, largestPenalty, node.Weight));
				}
				Gizmos.DrawCube(node.WorldPosition + Vector3.up * 0, Vector3.one * nodeSize);
			}
		}
	}

	[Serializable]
	public class WalkableMask
	{
		public LayerMask LayerMask;
		public int MovementPenalty;
	}

	private int GetMovementPenalty(Vector3 position)
	{
		Int32 allWalkableMasks = 0;

		foreach (var layer in WalkableMasks)
		{
			allWalkableMasks |= layer.LayerMask.value;
		}

		RaycastHit hit;
		Physics.Raycast(position + Vector3.up * 50, Vector3.down, out hit, 100, allWalkableMasks,
			QueryTriggerInteraction.Ignore);

		return GetPenaltyFromLayer(hit.collider.gameObject.layer);
	}

	private int GetPenaltyFromLayer(int layer)
	{
		foreach (var lay in WalkableMasks)
		{
			if (lay.LayerMask.value == Math.Pow(2, layer))
			{
				return lay.MovementPenalty;
			}
		}
		throw new ArgumentException("The provided layer is not in the WalkableMasks array.");
	}

	private void BlurWeights(int brushSize)
	{
		int[,] horizontalDirection = new int[gridSizeX, gridSizeY];

		// Must bea an odd number
		int kernelSize = brushSize * 2 + 1;
		int kernelArea = kernelSize * kernelSize;
		int halfKernel = brushSize;
		int halfKernelPlus = halfKernel + 1;

		// Horizontal way
		for (int y = 0; y < gridSizeY; y++)
		{
			// First node
			int result = 0;
			for (int i = -halfKernel; i < halfKernelPlus; i++)
			{
				int index = Mathf.Clamp(i, 0, halfKernel);
				result += nodes[index, y].Weight;
			}
			horizontalDirection[0, y] = result;

			// Remainder
			for (int x = 1; x < gridSizeX; x++)
			{
				int removeIndex = Mathf.Clamp(x - halfKernelPlus, 0, gridSizeX - 1);
				int addIndex = Mathf.Clamp(x + halfKernel, 0, gridSizeX - 1);

				result += nodes[addIndex, y].Weight - nodes[removeIndex, y].Weight;
				horizontalDirection[x, y] = result;
			}
		}

		// Vertical way
		for (int x = 0; x < gridSizeX; x++)
		{
			// First node
			int result = 0;
			for (int i = -halfKernel; i < halfKernelPlus; i++)
			{
				int index = Mathf.Clamp(i, 0, halfKernel);
				result += horizontalDirection[x, index];
			}
			nodes[x, 0].Weight = result / kernelArea;

			// Remainder
			for (int y = 1; y < gridSizeY; y++)
			{
				int removeIndex = Mathf.Clamp(y - halfKernelPlus, 0, gridSizeY - 1);
				int addIndex = Mathf.Clamp(y + halfKernel, 0, gridSizeY - 1);

				result += horizontalDirection[x, addIndex] - horizontalDirection[x, removeIndex];
				nodes[x, y].Weight = result / kernelArea;
			}
		}
	}
}
