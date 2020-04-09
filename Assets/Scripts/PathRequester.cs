using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class PathRequester
{
	private static Queue<PathRequest> requestQueue = new Queue<PathRequest>();
	private static PathRequest currentRequest;
	private static bool currentlyRequesting = false;

	private struct PathRequest
	{
		public Vector3 From;
		public Vector3 To;
		public Action<Vector3[], bool> Callback;
		public PathFinder PathFinder;
		public PathRequest(Vector3 from, Vector3 to, Action<Vector3[], bool> callback,
			PathFinder pathFinder)
		{
			this.From = from;
			this.To = to;
			this.Callback = callback;
			this.PathFinder = pathFinder;
		}
	}

	public static void RequestPath(Vector3 from, Vector3 to, Action<Vector3[], bool> callback,
		PathFinder pathFinder)
	{
		requestQueue.Enqueue(new PathRequest(from, to, callback, pathFinder));
		if (!currentlyRequesting)
			NextRequest(pathFinder);
	}

	private static void NextRequest(PathFinder pathFinder)
	{
		currentRequest = requestQueue.Dequeue();
		currentlyRequesting = true;
		pathFinder.RequestPath(currentRequest.From, currentRequest.To, OnPathFound);
	}

	private static void OnPathFound(Vector3[] path, bool successful)
	{
		currentRequest.Callback.Invoke(path, successful);
		if (requestQueue.Count > 0)
			NextRequest(requestQueue.Peek().PathFinder);
		else
			currentlyRequesting = false;
	}
}
