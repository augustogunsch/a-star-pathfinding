using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGrid : MonoBehaviour
{

    [SerializeField]
    private float nodeSize = 1;
    private float halfNodeSize;
    public float NodeSize { get { return nodeSize; } }
    public float HalfNodeSize { get { return halfNodeSize; } }


    private Transform terrainTransform;

    private Node[,] nodes;

    public static NodeGrid Instance;

    private bool gridDrawn;

    public LayerMask UnwalkableMask;

    private int sizeX;
    private int sizeY;

    float worldXCorner;
    float worldY;
    float worldZCorner;
    Vector3 worldGridCorner {
        get
        {
            return new Vector3(worldXCorner, worldY, worldZCorner);
        } 
    }

    void Awake()
    {
        terrainTransform = gameObject.GetComponent<Transform>();
    }

    private void Start()
    {
        Instance = this;

        halfNodeSize = NodeSize / 2;

        sizeX = (int)(terrainTransform.lossyScale.x / nodeSize);
        sizeY = (int)(terrainTransform.lossyScale.z / nodeSize);

        worldXCorner = terrainTransform.position.x - terrainTransform.lossyScale.x / 2;
        worldY = terrainTransform.lossyScale.y + terrainTransform.position.y;
        worldZCorner = terrainTransform.position.z - terrainTransform.lossyScale.z / 2;

        DrawGrid();
    }

    public void DrawGrid()
    {
        float halfNodeSize = nodeSize / 2;

        nodes = new Node[sizeX, sizeY];

        for(int x = 0; x < sizeX; x++)
        {
            for(int y = 0; y < sizeY; y++)
            {

                nodes[x, y] = new Node(x, y, CalcNodePosition(x, y));
            }
        }

        gridDrawn = true;
    }

    public void OnDrawGizmos()
    {
        if(false)
            if(gridDrawn)
            {
                foreach(var node in nodes)
                {
                    if (node.Walkable)
                        Gizmos.color = Color.white;
                    else
                        Gizmos.color = Color.red;
                    Gizmos.DrawCube(node.WorldPosition, node.VectorSize);
                }
            }
    }

    public Node GetNodeFromWorldPosition(Vector3 position)
    {
        Vector3 localPosition = position - worldGridCorner;

        int x = Mathf.Clamp((int)localPosition.x, 0, sizeX-1);
        int y = Mathf.Clamp((int)localPosition.z, 0, sizeY-1);

        return nodes[x, y];
    }

    private Vector3 CalcNodePosition(int gridX, int gridY)
    {
        Vector3 position = new Vector3(gridX * nodeSize, 0, gridY * nodeSize);

        position += worldGridCorner;

        position += new Vector3(HalfNodeSize, 0, HalfNodeSize);

        return position;
    }

    public Node[] GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for(int x = node.GridX - 1; x <= node.GridX + 1; x++)
        {
            for(int y = node.GridY - 1; y <= node.GridY + 1; y++)
            {
                if (x >= 0 && x < sizeX &&
                    y >= 0 && y < sizeY &&
                    x + y != 0)
                    neighbours.Add(nodes[x, y]);
            }
        }
        return neighbours.ToArray();
    }
}
