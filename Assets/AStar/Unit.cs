using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {


	public Transform target;
	public float speed = 1;
	
    private Vector2[] path;
	private int targetIndex;

	void Start() 
    {
		PathRequestHandler.RequestPath(transform.position,target.position, OnPathFound);
	}

	public void OnPathFound(Vector2[] newPath, bool pathSuccessful) 
    {
		if (pathSuccessful) {
			path = newPath;
			targetIndex = 0;

            StopCoroutine(FollowPath());
            StartCoroutine(FollowPath());
		}
	}

	IEnumerator FollowPath() 
    {
		Vector2 currentWaypoint = path[0];
		while (true) 
        {
            if ((Vector2)transform.position == currentWaypoint) 
            {
                targetIndex ++;
                if (targetIndex >= path.Length) {
                    yield break;
            }
            currentWaypoint = path[targetIndex];
        }

			transform.position = Vector2.MoveTowards(transform.position,currentWaypoint,speed * Time.deltaTime);
			yield return null;

		}
	}

	public void OnDrawGizmos() 
    {
		if (path != null) 
        {
			for (int i = targetIndex; i < path.Length; i ++) 
            {
				Gizmos.color = Color.black;
				Gizmos.DrawSphere(path[i], .1f);

				if (i == targetIndex) 
                {
					Gizmos.DrawLine(transform.position, path[i]);
				}
				else 
                {
					Gizmos.DrawLine(path[i-1],path[i]);
				}
			}
		}
	}
}