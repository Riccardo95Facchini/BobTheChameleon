using UnityEngine;

public class TongueRenderer : MonoBehaviour
{
    [SerializeField]
    private LineRenderer tongueRenderer;
    [SerializeField]
    private LayerMask tongueLayerMask;
    [SerializeField]
    private const float climbSpeed = 3f;
    [SerializeField]
    private PlayerMovement playerMovement;
    [SerializeField]
    private DistanceJoint2D tongueJoint;
    [SerializeField]
    private GameObject mouth;

    //Booleans
    private bool drawn;
    private bool tongueAttached;

    private Vector3 startPoint;
    private Vector3 endPoint;

    private CharacterController2D controller;

    public const float tongueMaxDistance = 10f;
    public const float tongueMinDistance = 0.5f;

    private void FixedUpdate()
    {
        if(drawn)
        {
            SetStartPosition();
            tongueRenderer.SetPosition(0, new Vector3(startPoint.x, startPoint.y, 1f));

            if(!tongueAttached)
            {
                playerMovement.isSwinging = false;
            }
            else
            {
                playerMovement.isSwinging = true;
                tongueJoint.anchor = mouth.transform.localPosition;
                tongueJoint.enabled = !controller.getGrounded();

                if(tongueJoint.enabled)
                    tongueJoint.distance = Vector2.Distance(startPoint, endPoint);

                HandleTongueLength();
            }
        }
    }

    /// <summary>
    /// Retracts or extends the 'tongue'
    /// </summary>
    private void HandleTongueLength()
    {
        if(Input.GetAxis("Vertical") > 0f && tongueAttached && tongueJoint.distance > tongueMinDistance)
        {
            tongueJoint.distance -= Time.deltaTime * climbSpeed;
        }
        else if(Input.GetAxis("Vertical") < 0f && tongueAttached && tongueJoint.distance < tongueMaxDistance)
        {
            tongueJoint.distance += Time.deltaTime * climbSpeed;
        }
    }

    /// <summary>
    /// When awake cache the needed components and initiate the renderer
    /// </summary>
    private void Awake()
    {
        tongueJoint.enabled = false;
        controller = GetComponent<CharacterController2D>();
    }

    /// <summary>
    /// Actually draws the tongue by setting the positions
    /// </summary>
    private void DrawTongue()
    {
        drawn = true;
        tongueRenderer.SetPosition(0, new Vector3(startPoint.x, startPoint.y, 1f));
        tongueRenderer.SetPosition(1, new Vector3(endPoint.x, endPoint.y, 1f));
    }

    /// <summary>
    /// Changes the startPoint without drawing the tongue
    /// </summary>
    private void SetStartPosition()
    {
        startPoint = mouth.transform.position;
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
        RaycastHit2D[] hits = Physics2D.RaycastAll(startPoint, (endPoint - startPoint), tongueMaxDistance, tongueLayerMask);
        Debug.DrawLine(startPoint, endPoint, Color.green, 5);

        if(hits.Length > 0)
        {
            if(hits[0].collider.tag == Names.Layers.Anchor.ToString())
            {
                endPoint = hits[0].transform.position;
                Attach();
                return true;
            }
            else
            {
                // Instead of connecting to the center it snaps to the collision point.
                endPoint = hits[0].point;
            }
        }
        else
            endPoint = startPoint + ((endPoint - startPoint).normalized * tongueMaxDistance);

        return false;
    }

    /// <summary>
    /// Attaches the DistanceJoint2D to the hit ancor and enables it
    /// </summary>
    private void Attach()
    {
        tongueAttached = true;
        tongueJoint.anchor = startPoint;
        tongueJoint.connectedAnchor = endPoint;
        tongueJoint.distance = Vector2.Distance(startPoint, endPoint);
        //tongueJoint.enabled = true;
    }

    /// <summary>
    /// Disables the DistanceJoint2D when the tongue is retracted
    /// </summary>
    private void Detach()
    {
        tongueAttached = false;
        tongueJoint.enabled = false;
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
        drawn = false;
        tongueAttached = false;
        EventManager.StartListening(Names.Events.TongueOut.ToString(), TongueOut);
    }

    private void TongueOut()
    {
        SetStartPosition();
        SetEndPosition();
        EventManager.StopListening(Names.Events.TongueOut.ToString(), TongueOut);

        if(!CorrectSide())
        {
            controller.Flip();
        }

        if(CheckRaycast())
        {
            DrawTongue();
            EventManager.StartListening(Names.Events.TongueIn.ToString(), TongueIn);
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

        EventManager.StartListening(Names.Events.TongueOut.ToString(), TongueOut);
        EventManager.StopListening(Names.Events.TongueIn.ToString(), TongueIn);
        tongueRenderer.SetPosition(1, tongueRenderer.GetPosition(0));
        drawn = false;
    }

    private void OnDisable()
    {
        EventManager.StopListening(Names.Events.TongueOut.ToString(), TongueOut);
        drawn = false;
    }
    #endregion
}
