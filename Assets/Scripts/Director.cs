using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Director : MonoBehaviour
{
    public GameObject Target;
    public GameObject Pursuer;
    Node[] path;

    void Start()
    {
        path = Pathfinding.FindPath(Target.transform.position, Pursuer.transform.position);
        Debug.Log("Done");
        string message = "";
        foreach(var node in path)
            message += $"{node.WorldPosition} => ";
        Debug.Log(message);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        if (path != null)
        {
            foreach(var node in path)
            {
                Gizmos.DrawSphere(node.WorldPosition + Vector3.up, 1);
            }
        }
    }
}
