using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller class of the game.
/// </summary>
public class Director : MonoBehaviour
{
	public GameObject Target;
	public GameObject Pursuer;
	Node[] path;
	public NodeGrid Grid;

	//void Update()
	//{
	//	path = Pathfinding.FindPath(Target.transform.position, Pursuer.transform.position, Grid);
	//}

	// DEBUG: draws the path.
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;
		if (path != null)
		{
			foreach (var node in path)
			{
				Gizmos.DrawSphere(node.WorldPosition + Vector3.up, 1);
			}
		}
	}
}
