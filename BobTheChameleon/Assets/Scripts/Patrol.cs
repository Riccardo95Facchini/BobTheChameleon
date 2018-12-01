using UnityEngine;

public class Patrol : MonoBehaviour
{

    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float chargeSpeed;
    [SerializeField]
    private float lineOfSight;
    [SerializeField]
    private Transform groundDetection;

    private bool movingLeft = true;
    private bool charging;

    void Update()
    {
        float speed;

        if(!charging)
            speed = walkSpeed;
        else
            speed = chargeSpeed;

        transform.Translate(Vector2.left * speed * Time.deltaTime);

        RaycastHit2D verticalCheck = Physics2D.Raycast(groundDetection.position, Vector2.down, 0.5f);

        Vector2 rayDirection;

        if(movingLeft)
            rayDirection = Vector2.left;
        else
            rayDirection = Vector2.right;

        RaycastHit2D horizontalCheck = Physics2D.Raycast(groundDetection.position, rayDirection, 0.2f);

        if(horizontalCheck.collider != false)
        {
            if(horizontalCheck.collider.tag != Names.Tags.Player.ToString())
                FlipSprite();
        }

        horizontalCheck = Physics2D.Raycast(groundDetection.position, rayDirection, lineOfSight);

        if(horizontalCheck.collider != false)
        {
            if(horizontalCheck.collider.tag == Names.Tags.Player.ToString())
                charging = true;
        }

        if(verticalCheck.collider == false)
            FlipSprite();
    }

    private void FlipSprite()
    {
        charging = false;

        if(movingLeft)
        {
            transform.eulerAngles = new Vector3(0, -180, 0);
            movingLeft = false;
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            movingLeft = true;
        }
    }    
}
