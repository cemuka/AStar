using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class PathRequestHandler
{
	static Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
	static PathRequest currentPathRequest;
	static Pathfinding pathfinding;
	static bool isProcessingPath;

	public static void Initialize(Pathfinding path) 
    {
		pathfinding = path;
	}

	public static void RequestPath(Vector2 pathStart, Vector2 pathEnd, Action<Vector2[], bool> callback) 
    {
		var newRequest = new PathRequest(pathStart,pathEnd,callback);
		pathRequestQueue.Enqueue(newRequest);
		TryProcessNext();
	}

	private static void TryProcessNext() 
    {
		if (!isProcessingPath && pathRequestQueue.Count > 0) {
			currentPathRequest = pathRequestQueue.Dequeue();
			isProcessingPath = true;
			pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
		}
	}

	public static void FinishedProcessingPath(Vector2[] path, bool success) 
    {
		currentPathRequest.callback(path,success);
		isProcessingPath = false;
		TryProcessNext();
	}

	struct PathRequest 
    {
		public Vector2 pathStart;
		public Vector2 pathEnd;
		public Action<Vector2[], bool> callback;

		public PathRequest(Vector2 _start, Vector2 _end, Action<Vector2[], bool> _callback) 
        {
			pathStart = _start;
			pathEnd = _end;
			callback = _callback;
		}
	}
}