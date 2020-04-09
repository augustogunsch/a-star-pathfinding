using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Agent : MonoBehaviour
{
	public PathFinder MyPathFinder;
	public float Speed;
	public Transform TargetTransform;

	public void Start()
	{
		PathRequester.RequestPath(transform.position, TargetTransform.position,
			OnPathFound, MyPathFinder);
	}

	private void OnPathFound(Vector3[] path, bool succesful)
	{
		if (!succesful)
			return;
		StartCoroutine(FollowPath(path));
	}

	private IEnumerator FollowPath(Vector3[] path)
	{
		for (int i = 0; i < path.Length; i++)
		{
			while (true)
			{
				if (MoveToPoint(path[i]))
					break;
				yield return null;
			}
		}
		yield break;
	}

	private bool MoveToPoint(Vector3 point)
	{
		transform.position = Vector3.MoveTowards(transform.position, point, Speed * Time.deltaTime);
		if (transform.position == point)
			return true;
		return false;
	}
}
