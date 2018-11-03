using UnityEngine;

public class DrawLines : MonoBehaviour
{
    [SerializeField]
    private GameObject lineGeneratorPrefab;
    [SerializeField]
    private GameObject player;

    private GameObject newLineGenerator;
    private bool drawn;
    private LineRenderer lineRenderer;
    private BoxCollider lineCollider;

    private Vector3 startPoint;
    private Vector3 endPoint;


    private int layerMask = ~(1 << 8);


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
        RaycastHit2D[] hits = Physics2D.RaycastAll(startPoint, (endPoint - startPoint), Vector2.Distance(startPoint, endPoint), layerMask);
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
            return true;
        }
        return false;
    }


    #region EventManager
    private void OnEnable()
    {
        EventManager.StartListening("TongueOut", TongueOut);
        drawn = false;
    }

    private void TongueOut()
    {
        EventManager.StartListening("TongueIn", TongueIn);
        EventManager.StopListening("TongueOut", TongueOut);
        SetStartPosition();
        SetEndPosition();
        if(CheckRaycast())
            SpawnLineGenerator();
        else
        {
            SpawnLineGenerator();
            Invoke("TongueIn", 0.25f);

        }

    }
    private void TongueIn()
    {
        EventManager.StartListening("TongueOut", TongueOut);
        EventManager.StopListening("TongueIn", TongueIn);
        Destroy(newLineGenerator);
        drawn = false;
    }

    private void OnDisable()
    {
        EventManager.StopListening("TongueOut", TongueOut);
        drawn = false;
    }
    #endregion
}
