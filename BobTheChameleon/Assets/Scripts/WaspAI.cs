using Pathfinding;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]
public class WaspAI : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float updateRate = 2f; //How many times we'll update the path

    //Calculated path
    [SerializeField]
    private Path path;

    //Ai's speed per second
    [SerializeField]
    private float speed = 300f;
    [SerializeField]
    private ForceMode2D forceMode;

    private Seeker seeker;
    private Rigidbody2D rb;

    [HideInInspector]
    public bool pathIsEnded = false;

    //Max distance from the AI to a waypoint for it to continue to the next waypoint
    private float nextWaypointDistance = 3f;

    private int currentWaypoint = 0; //Index of the waypoint currently moving towards to

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        if(target == null)
        {
            return;
        }

        //Start a new path to te target position, return the result to the OnPathComplete method
        seeker.StartPath(transform.position, target.position, OnPathComplete);

        StartCoroutine(UpdatePath());
    }

    void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    IEnumerator UpdatePath()
    {
        if(target == null)
        {
            //TODO: Insert player search here
        }

        seeker.StartPath(transform.position, target.position, OnPathComplete);

        yield return new WaitForSeconds(1f / updateRate);
        StartCoroutine(UpdatePath());

    }

    private void FixedUpdate()
    {
        if(target == null)
        {
            //TODO: Insert player search here
        }

        if(path == null)
            return;

        //If the last waypoint in the path is reached
        if(currentWaypoint >= path.vectorPath.Count)
        {
            if(pathIsEnded)
                return;

            pathIsEnded = true;
            return;
        }

        pathIsEnded = false;

        //Direction to next waypoint
        Vector3 direction = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        direction *= speed * Time.fixedDeltaTime;

        //Move the AI
        rb.AddForce(direction, forceMode);

        var dist = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
        if(dist < nextWaypointDistance)
        {
            currentWaypoint++;
            return;
        }
    }
}
