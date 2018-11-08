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
    private BoxCollider lineCollider;

    private Vector3 startPoint;
    private Vector3 endPoint;

    //Take 1, shift it of 8 places to the left, reverse the values of the bits.
    private int playerLayerMask = ~(1 << 8);


    private void SpawnLineGenerator()
    {
        drawn = true;
        newLineGenerator = Instantiate(lineGeneratorPrefab);
        lineRenderer = newLineGenerator.GetComponent<LineRenderer>();

        lineRenderer.SetPosition(0, new Vector3(startPoint.x, startPoint.y, 0f));
        lineRenderer.SetPosition(1, new Vector3(endPoint.x, endPoint.y, 0f));
    }

    private void Update()
    {
        if(drawn)
        {
            SetStartPosition();
            lineRenderer.SetPosition(0, new Vector3(startPoint.x, startPoint.y, 0f));
        }
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
        RaycastHit2D[] hits = Physics2D.RaycastAll(startPoint, (endPoint - startPoint), Vector2.Distance(startPoint, endPoint), playerLayerMask);
        Debug.DrawRay(startPoint, (endPoint - startPoint).normalized, Color.red, 5);
        Debug.DrawLine(startPoint, endPoint, Color.green, 5);

        Debug.Log(hits.Length);
        if(hits.Length > 0)
        {
            for(int i = 0; i < hits.Length; i++)
            {
                Debug.Log(hits[i].collider.gameObject.name);
            }

            endPoint = hits[0].transform.position;
            if(hits[0].collider.tag == "Anchor")
                Attach();
            return true;
        }
        return false;
    }

    private void Attach()
    {
        attached = true;        
    }

    private void Detach()
    {
        attached = false;
    }


    private void OnEnable()
    {
        drawn = false;
        attached = false;
        EventManager.StartListening("TongueOut", TongueOut);
    }

    #region EventManager

    private void TongueOut()
    {
        
        EventManager.StopListening("TongueOut", TongueOut);
        SetStartPosition();
        SetEndPosition();
        if(CheckRaycast())
        {
            SpawnLineGenerator();
            EventManager.StartListening("TongueIn", TongueIn);
        }
        else
        {
            SpawnLineGenerator();
            Invoke("TongueIn", 0.1f);

        }


    }
    private void TongueIn()
    {
        EventManager.StartListening("TongueOut", TongueOut);
        EventManager.StopListening("TongueIn", TongueIn);
        Destroy(newLineGenerator);
        drawn = false;

        if(attached)
            Detach();
    }

    private void OnDisable()
    {
        EventManager.StopListening("TongueOut", TongueOut);
        drawn = false;
    }
    #endregion
}
