using System.Collections;
using UnityEngine;

public class TongueRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer tongueRenderer;
    [SerializeField] private LayerMask tongueLayerMask;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private CircleCollider2D tailCollider;

    [SerializeField] private Transform mouth;

    [SerializeField] [Min(0.5f)] private float tongueMinDistance;
    [SerializeField] private float tongueMaxDistance = 10f;
    [SerializeField] private float climbSpeed = 3f;

    private bool drawn;
    private bool tongueAttached;

    private Vector3 startPoint;
    private Vector3 endPoint;

    private CharacterController2D controller;
    private DistanceJoint2D tongueJoint;
    private GameObject caughtPrey = null;
    private PolygonCollider2D headCollider;

    /// <summary>
    /// When awake cache the needed components and initiate the renderer
    /// </summary>
    private void Awake()
    {
        drawn = false;
        tongueAttached = false;
        controller = GetComponent<CharacterController2D>();
        tongueJoint = GetComponent<DistanceJoint2D>();
        headCollider = GetComponent<PolygonCollider2D>();
        tongueJoint.enabled = false;
    }

    /// <summary>
    /// Starts when the tongue is drawn and loops until it isn't anymore
    /// </summary>
    /// <returns>null</returns>
    private IEnumerator Drawn()
    {
        while(drawn)
        {
            SetStartPosition();
            tongueRenderer.SetPosition(0, new Vector3(startPoint.x, startPoint.y, 1f));

            if(tongueAttached)
            {
                if(caughtPrey == null)
                    HandleAnchor();
                else
                    HandlePrey();
            }
            yield return null;
        }
    }

    /// <summary>
    /// If a prey is caught, set the tip of the tongue to the position of the prey until it is eaten
    /// </summary>
    private void HandlePrey()
    {
        if(caughtPrey.activeSelf)
            tongueRenderer.SetPosition(1, caughtPrey.transform.position);
        else
        {
            TongueIn();
            caughtPrey = null;
        }

    }

    /// <summary>
    /// Handles when Bob is attached to an anchor point
    /// </summary>
    private void HandleAnchor()
    {
        tongueJoint.anchor = mouth.localPosition;
        tongueJoint.enabled = !controller.getGrounded();

        if(!tongueJoint.enabled)
        {
            tongueJoint.distance = Vector2.Distance(startPoint, endPoint);
        }

        if(tongueJoint.distance > tongueMaxDistance)
            TongueIn();
        else
            HandleTongueLength();
    }

    /// <summary>
    /// Retracts or extends the 'tongue'
    /// </summary>
    private void HandleTongueLength()
    {
        if(Input.GetAxis("Vertical") > 0f && tongueAttached && tongueJoint.distance > tongueMinDistance)
        {
            tongueJoint.distance -= Time.fixedDeltaTime * climbSpeed;
        }
        else if(Input.GetAxis("Vertical") < 0f && tongueAttached && tongueJoint.distance < tongueMaxDistance)
        {
            tongueJoint.distance += Time.fixedDeltaTime * climbSpeed;
        }
    }

    /// <summary>
    /// Actually draws the tongue by setting the positions
    /// </summary>
    private void DrawTongue()
    {
        drawn = true;
        tongueRenderer.SetPosition(0, new Vector3(startPoint.x, startPoint.y, 1f));
        tongueRenderer.SetPosition(1, new Vector3(endPoint.x, endPoint.y, 1f));
        StartCoroutine(Drawn());
    }

    /// <summary>
    /// Changes the startPoint without drawing the tongue
    /// </summary>
    private void SetStartPosition()
    {
        startPoint = mouth.position;
    }

    /// <summary>
    /// Changes the endPoint based on the cursor's position 
    /// relative to the screen without drawing the tongue
    /// </summary>
    private void SetEndPosition()
    {
        endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    /// <summary>
    /// Checks for collisions in the path from the mouth to the aimed point, if one or more
    /// is found, it uses the collision position as endPoint.
    /// </summary>
    /// <returns>
    /// true if at least one collision is found.
    /// false if no collisions are found.
    /// </returns>
    private bool CheckRaycast()
    {
        RaycastHit2D hit = Physics2D.Raycast(startPoint, (endPoint - startPoint), tongueMaxDistance, tongueLayerMask);
        if(hit.collider != null)
        {
            if(hit.collider.tag == Names.Tags.Anchor.ToString() && CheckMaximumDistance(hit.transform.position))
            {
                endPoint = hit.transform.position;
                Attach(hit.rigidbody);
                return true;
            }
            else if(hit.collider.tag == Names.Tags.Prey.ToString() && CheckMaximumDistance(hit.transform.position))
            {
                caughtPrey = hit.collider.gameObject;
                hit.collider.GetComponent<PreyPatrol>().Caught(mouth);
                tongueAttached = true;
                return true;
            }
            else
            {
                // Instead of connecting to the center it snaps to the collision point.
                endPoint = hit.point;
            }
        }
        else if(Vector2.Distance(startPoint, endPoint) > tongueMaxDistance)
        {
            endPoint = new Ray2D(startPoint, (endPoint - startPoint)).GetPoint(tongueMaxDistance);
        }

        return false;
    }

    /// <summary>
    /// Checks if the point is too far to be reached, if too far sets the end position to the furthest point on the line connecting start and end point.
    /// </summary>
    /// <param name="end">Point where the toungue tip should be drawn</param>
    /// <returns>True if the distance is less then the maximum one, false otherwise</returns>
    private bool CheckMaximumDistance(Vector2 end)
    {
        if(Vector2.Distance(startPoint, end) > tongueMaxDistance)
        {
            endPoint = new Ray2D(startPoint, (endPoint - startPoint)).GetPoint(tongueMaxDistance);
            return false;
        }
        else
            return true;
    }

    /// <summary>
    /// Attaches the DistanceJoint2D to the hit ancor and enables it
    /// </summary>
    private void Attach(Rigidbody2D anchorRB)
    {
        tongueAttached = true;
        tongueJoint.anchor = startPoint;
        //tongueJoint.connectedAnchor = endPoint;
        tongueJoint.distance = Vector2.Distance(startPoint, endPoint);
        tongueJoint.connectedBody = anchorRB;
    }

    /// <summary>
    /// Disables the DistanceJoint2D when the tongue is retracted
    /// </summary>
    private void Detach()
    {
        tongueAttached = false;
        tongueJoint.enabled = false;
        tongueJoint.connectedBody = null;
    }

    /// <summary>
    /// Checks if the sprite is facing the right side when launching the tongue.
    /// </summary>
    /// <returns>
    /// true if the sprite is facing right(left) and the tongue is launched to the right(left)
    /// false if the tongue is launched in the opposite direction
    /// </returns>
    private bool CorrectSide()
    {
        if(endPoint.x >= startPoint.x && controller.getFacingRight())
            return true;
        else if(endPoint.x <= startPoint.x && !controller.getFacingRight())
            return true;

        return false;
    }

    #region EventManager
    private void OnEnable()
    {
        EventManager.StartListening(Names.Events.TongueOut, TongueOut);
    }

    private void TongueOut()
    {
        CancelInvoke();
        SetStartPosition();
        SetEndPosition();
        headCollider.enabled = false;
        tailCollider.enabled = false;
        EventManager.StopListening(Names.Events.TongueOut, TongueOut);

        if(!CorrectSide())
        {
            controller.Flip();
        }

        if(CheckRaycast())
            EventManager.StartListening(Names.Events.TongueIn, TongueIn);
        else
            Invoke(Names.Events.TongueIn.ToString(), 0.1f);

        DrawTongue();
    }
    private void TongueIn()
    {
        if(tongueAttached)
            Detach();

        headCollider.enabled = true;
        tailCollider.enabled = true;
        EventManager.StartListening(Names.Events.TongueOut, TongueOut);
        EventManager.StopListening(Names.Events.TongueIn, TongueIn);
        tongueRenderer.SetPosition(1, tongueRenderer.GetPosition(0));
        drawn = false;
    }

    private void OnDisable()
    {
        EventManager.StopListening(Names.Events.TongueOut, TongueOut);
        drawn = false;
    }
    #endregion
}
