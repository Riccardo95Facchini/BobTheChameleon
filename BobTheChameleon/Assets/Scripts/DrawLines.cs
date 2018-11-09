using UnityEngine;

public class DrawLines : MonoBehaviour
{
    [SerializeField]
    private GameObject lineGeneratorPrefab;
    [SerializeField]
    private GameObject player;

    //Booleans
    private bool drawn;
    private bool attached;

    private GameObject newLineGenerator;
    private LineRenderer lineRenderer;
    private DistanceJoint2D tongueJoint;

    private Vector3 startPoint;
    private Vector3 endPoint;

    //Take 1, shift it of 8 places to the left, reverse the values of the bits.
    private int ignoreLayerMask = ~(1 << 8);

    private void FixedUpdate()
    {
        if(drawn)
        {
            SetStartPosition();
            lineRenderer.SetPosition(0, new Vector3(startPoint.x, startPoint.y, 0f));

            if(attached)
            {
                
                //Debug.Log("\n"+ startPoint.ToString() +" "+ tongueJoint.anchor.ToString());
                //Debug.Log(Vector2.Distance(startPoint, endPoint).ToString());
            }
        }
    }

    private void Awake()
    {
        tongueJoint = player.GetComponent<DistanceJoint2D>();
    }

    private void SpawnLineGenerator()
    {
        drawn = true;
        newLineGenerator = Instantiate(lineGeneratorPrefab);
        lineRenderer = newLineGenerator.GetComponent<LineRenderer>();

        lineRenderer.SetPosition(0, new Vector3(startPoint.x, startPoint.y, 0f));
        lineRenderer.SetPosition(1, new Vector3(endPoint.x, endPoint.y, 0f));
    }
    private void SetStartPosition()
    {
        startPoint = player.transform.position;
        startPoint = new Vector3(startPoint.x + 0.45f, startPoint.y + 0.4f, 0f);
    }
    private void SetEndPosition()
    {
        endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private bool CheckRaycast()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(startPoint, (endPoint - startPoint), Vector2.Distance(startPoint, endPoint), ignoreLayerMask);
        //Debug.DrawRay(startPoint, (endPoint - startPoint).normalized, Color.red, 5);
        Debug.DrawLine(startPoint, endPoint, Color.green, 5);

        //Debug.Log(hits.Length);
        if(hits.Length > 0)
        {
            //for(int i = 0; i < hits.Length; i++)
            //{
            //    Debug.Log(hits[i].collider.gameObject.name);
            //}

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

    private void Attach()
    {
        attached = true;
        tongueJoint.connectedAnchor = endPoint;
        tongueJoint.enabled = true;
    }

    private void Detach()
    {
        attached = false;
        tongueJoint.enabled = false;
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
        EventManager.StopListening(Names.Events.TongueOut.ToString(), TongueOut);
        SetStartPosition();
        SetEndPosition();
        if(CheckRaycast())
        {
            SpawnLineGenerator();
            EventManager.StartListening(Names.Events.TongueIn.ToString(), TongueIn);
        }
        else
        {
            SpawnLineGenerator();
            Invoke(Names.Events.TongueIn.ToString(), 0.1f);
        }

    }
    private void TongueIn()
    {
        if(attached)
            Detach();

        EventManager.StartListening(Names.Events.TongueOut.ToString(), TongueOut);
        EventManager.StopListening(Names.Events.TongueIn.ToString(), TongueIn);
        Destroy(newLineGenerator);
        drawn = false;
    }

    private void OnDisable()
    {
        EventManager.StopListening(Names.Events.TongueOut.ToString(), TongueOut);
        drawn = false;
    }
    #endregion
}
