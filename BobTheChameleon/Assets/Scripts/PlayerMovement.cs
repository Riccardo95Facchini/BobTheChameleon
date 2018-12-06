using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterController2D controller;
    [SerializeField]
    private float runSpeed = 40f;
    [SerializeField]
    private float sprintModifier = 1.5f;

    float horizontalMove = 0f;
    bool jump = false;

    private bool isOnLadder;

    /* Not actually used at the moment
    private bool isSwinging;

    public bool IsSwinging
    {
        get { return isSwinging; }
        set { isSwinging = value; }
    }
    */

    public void SetIsOnLadder(bool v)
    {
        isOnLadder = v;
    }

    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;


        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
        else if (Input.GetButtonDown("Sprint"))
        {
            runSpeed *= sprintModifier;
            
            
        }
        else if (Input.GetButtonUp("Sprint"))
            runSpeed /= sprintModifier;
    }

    void FixedUpdate()
    {

        
        // Move our character
        controller.Move(horizontalMove * Time.fixedDeltaTime, jump, isOnLadder);
        
       
        jump = false;
    }
}