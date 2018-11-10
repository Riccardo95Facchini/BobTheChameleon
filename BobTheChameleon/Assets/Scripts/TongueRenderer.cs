using UnityEngine;

public class TongueRenderer : MonoBehaviour
{
    [SerializeField]
    private GameObject lineGeneratorPrefab;

    //Booleans
    private bool drawn;
    private bool attached;

    private GameObject newLineGenerator;
    private LineRenderer lineRenderer;
    private DistanceJoint2D tongueJoint;

    private Vector3 startPoint;
    private Vector3 endPoint;

    private CharacterController2D controller;

    //Take 1, shift it of 8 places to the left, reverse the values of the bits.
    private int ignoreLayerMask = ~(1 << 8);

    private void FixedUpdate()
    {
        if(drawn)
        {
            SetStartPosition();
            lineRenderer.SetPosition(0, new Vector3(startPoint.x, startPoint.y, 0f));
        }
    }

    /// <summary>
    /// When awake cache the needed components and initiate the renderer
    /// </summary>
    private void Awake()
    {
        tongueJoint = GetComponent<DistanceJoint2D>();
        newLineGenerator = Instantiate(lineGeneratorPrefab);
        lineRenderer = newLineGenerator.GetComponent<LineRenderer>();
        controller = GetComponent<CharacterController2D>();
    }

    /// <summary>
    /// Actually draws the tongue by setting the positions
    /// </summary>
    private void DrawTongue()
    {
        drawn = true;
        lineRenderer.SetPosition(0, new Vector3(startPoint.x, startPoint.y, 0f));
        lineRenderer.SetPosition(1, new Vector3(endPoint.x, endPoint.y, 0f));
    }

    /// <summary>
    /// Changes the startPoint without drawing the tongue
    /// </summary>
    private void SetStartPosition()
    {
        startPoint = this.transform.position;
        if(controller.getFacingRight())
            startPoint = new Vector3(startPoint.x + 0.45f, startPoint.y + 0.4f, 0f);
        else
            startPoint = new Vector3(startPoint.x - 0.45f, startPoint.y + 0.4f, 0f);
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
        RaycastHit2D[] hits = Physics2D.RaycastAll(startPoint, (endPoint - startPoint), Vector2.Distance(startPoint, endPoint), ignoreLayerMask);
        Debug.DrawLine(startPoint, endPoint, Color.green, 5);

        if(hits.Length > 0)
        {
            if(hits[0].collider.tag == Names.Layers.Anchor.ToString())
            {
                endPoint = hits[0].transform.position;
                Attach();
            }
            else
            {
                // Instead of connecting to the center it snaps to the collision point.
                endPoint = hits[0].point;
            }

            return true;
        }
        return false;
    }

    /// <summary>
    /// Attaches the DistanceJoint2D to the hit ancor and enables it
    /// </summary>
    private void Attach()
    {
        attached = true;
        tongueJoint.connectedAnchor = endPoint;
        tongueJoint.enabled = true;
    }

    /// <summary>
    /// Disables the DistanceJoint2D when the tongue is retracted
    /// </summary>
    private void Detach()
    {
        attached = false;
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
        attached = false;
        EventManager.StartListening(Names.Events.TongueOut.ToString(), TongueOut);
    }

    private void TongueOut()
    {
        SetStartPosition();
        SetEndPosition();
        if(CorrectSide())
        {
            EventManager.StopListening(Names.Events.TongueOut.ToString(), TongueOut);
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
    }
    private void TongueIn()
    {
        if(attached)
            Detach();

        EventManager.StartListening(Names.Events.TongueOut.ToString(), TongueOut);
        EventManager.StopListening(Names.Events.TongueIn.ToString(), TongueIn);
        lineRenderer.SetPosition(1, new Vector3(startPoint.x, startPoint.y, 0f));
        drawn = false;
    }

    private void OnDisable()
    {
        EventManager.StopListening(Names.Events.TongueOut.ToString(), TongueOut);
        drawn = false;
    }
    #endregion
}
