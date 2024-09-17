using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagPointToPath : MonoBehaviour
{
    public PathCreator pathCreator;
    public Transform worldPoint;
    public Transform findPoint;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            findPoint.position = pathCreator.path.GetClosestPointOnPath(worldPoint.position);
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            float distance = pathCreator.path.GetClosestDistanceAlongPath(worldPoint.position);
            Debug.Log("Found Distance = " + distance);
            findPoint.position = pathCreator.path.GetPointAtDistance(distance);
        }
    }
}
