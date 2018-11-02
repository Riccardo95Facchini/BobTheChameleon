using UnityEngine;

public class DrawLines : MonoBehaviour
{
    [SerializeField]
    private GameObject lineGeneratorPrefab;
    [SerializeField]
    private GameObject player;

    private GameObject newLineGenerator;



    private void SpawnLineGenerator()
    {
        Vector3 start = player.transform.position;
        Vector3 end = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        newLineGenerator = Instantiate(lineGeneratorPrefab);
        LineRenderer lineRenderer = newLineGenerator.GetComponent<LineRenderer>();

        start = new Vector3(start.x + 0.45f, start.y + 0.4f, start.z);

        Debug.Log(start.ToString());

        lineRenderer.SetPosition(0, new Vector3(start.x, start.y, start.z));
        lineRenderer.SetPosition(1, new Vector3(end.x, end.y, start.z));
    }

    #region EventManager
    private void OnEnable()
    {
        EventManager.StartListening("TongueOut", TongueOut);
    }

    private void TongueOut()
    {
        EventManager.StartListening("TongueIn", TongueIn);
        EventManager.StopListening("TongueOut", TongueOut);
        SpawnLineGenerator();

    }
    private void TongueIn()
    {
        EventManager.StartListening("TongueOut", TongueOut);
        EventManager.StopListening("TongueIn", TongueIn);
        Destroy(newLineGenerator);
    }

    private void OnDisable()
    {
        EventManager.StopListening("TongueOut", TongueOut);
    }
    #endregion
}
