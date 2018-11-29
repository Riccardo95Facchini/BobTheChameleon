using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrayScript : MonoBehaviour {

    bool goUp;
    public float speed = 1;
    public int gain = 10;

    // Use this for initialization
    void Start()
    {

        StartCoroutine(switchDirection());

    }


    IEnumerator switchDirection()
    {

        while (gameObject.activeSelf)
        {
            yield return new WaitForSeconds(0.5f);
            goUp = !goUp;
        }


    }

    // Update is called once per frame
    void Update()
    {

        if (goUp)
        {
            transform.position = transform.position + (new Vector3(0, speed * Time.deltaTime, 0));
        }
        else
        {
            transform.position = transform.position - (new Vector3(0, speed * Time.deltaTime, 0));
        }

    }






    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnPicked(collision);


        }
    }







    protected virtual void OnPicked(Collider2D collision)
    {
        Debug.Log("you've eaten a bug");

        BonusManager bonusManager = collision.GetComponent<BonusManager>();

        if (!bonusManager) return;
        else
        {
            
            bonusManager.Gain(gain);
            Destroy(gameObject);
        }
    }
}
