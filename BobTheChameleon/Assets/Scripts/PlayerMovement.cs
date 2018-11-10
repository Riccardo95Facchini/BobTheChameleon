using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController2D controller;

    public float runSpeed = 40f;

    float horizontalMove = 0f;
    float verticalMove = 0f;
    bool jump = false;

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        //Climbs at half speed.
        verticalMove = Input.GetAxisRaw("Vertical") * runSpeed * 0.5f;

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
        controller.Move(horizontalMove * Time.fixedDeltaTime, verticalMove * Time.fixedDeltaTime, jump);
        jump = false;
    }
}
