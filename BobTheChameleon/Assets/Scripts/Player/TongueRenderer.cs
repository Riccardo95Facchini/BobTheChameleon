using System.Collections;
using UnityEngine;

public class TongueRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer tongueRenderer;
    [SerializeField] private LayerMask tongueLayerMask;
    [SerializeField] private PlayerMovement playerMovement;

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

    /// <summary>
    /// When awake cache the needed components and initiate the renderer
    /// </summary>
    private void Awake()
    {
        drawn = false;
        tongueAttached = false;
        controller = GetComponent<CharacterController2D>();
        tongueJoint = GetComponent<DistanceJoint2D>();
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
            if(hit.collider.tag == Names.Tags.Anchor.ToString())
            {
                endPoint = hit.transform.position;
                Attach(hit.rigidbody);
                return true;
            }
            else if(hit.collider.tag == Names.Tags.Prey.ToString())
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
            //var x2 = startPoint.x + (endPoint.x - startPoint.x) * tongueMaxDistance / Vector2.Distance(startPoint, endPoint);
            //var y2 = startPoint.y + (endPoint.y - startPoint.y) * tongueMaxDistance / Vector2.Distance(startPoint, endPoint);
            //endPoint = new Vector3(x2, y2, 0);
            endPoint = new Ray2D(startPoint, (endPoint - startPoint)).GetPoint(tongueMaxDistance);
        }

        return false;
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
        SetStartPosition();
        SetEndPosition();
        EventManager.StopListening(Names.Events.TongueOut, TongueOut);

        if(!CorrectSide())
        {
            controller.Flip();
        }

        if(CheckRaycast())
        {
            DrawTongue();
            EventManager.StartListening(Names.Events.TongueIn, TongueIn);
        }
        else
        {
            DrawTongue();
            Invoke(Names.Events.TongueIn.ToString(), 0.1f);
        }
    }
    private void TongueIn()
    {
        if(tongueAttached)
            Detach();

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
