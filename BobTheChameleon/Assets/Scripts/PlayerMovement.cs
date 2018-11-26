using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController2D controller;


    public float runSpeed = 40f;
    private float climbSpeed = 20f;//climbSpeed=0.5*runSpeed;

    float horizontalMove = 0f;
    float verticalMove = 0f;
    bool jump = false;

    public bool isSwinging;
    public bool isOnLadder = false;

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        if(isOnLadder)
        {
            verticalMove = Input.GetAxisRaw("Vertical") * climbSpeed;
        }


        if(Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
        else if(Input.GetButtonDown("Sprint"))
            runSpeed *= 1.5f;
        else if(Input.GetButtonUp("Sprint"))
            runSpeed /= 1.5f;
    }

    void FixedUpdate()
    {
        // Move our character
        controller.Move(horizontalMove * Time.fixedDeltaTime, jump);
        jump = false;

        //Move our character when climbing
        if(isOnLadder)
        {
            controller.Move(verticalMove * Time.fixedDeltaTime, jump);
            jump = false;
        }
    }
}
