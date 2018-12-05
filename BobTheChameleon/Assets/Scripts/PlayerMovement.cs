using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;

    public float runSpeed = 40f;
    //private float climbSpeed = 20f;//climbSpeed=0.5*runSpeed;

    private bool jump;
    private bool isPlayerDead;

    [SerializeField]
    public bool isOnLadder;
    public bool isSwinging;

    private void Awake()
    {
        jump = false;
        isPlayerDead = false;
        isOnLadder = false;
        isSwinging = false;
        EventManager.StartListening(Names.Events.PlayerDead, PlayerDead);
    }

    public void SetIsOnLadder(bool v)
    {
        isOnLadder = v;
    }

    // Update is called once per frame
    void Update()
    {
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
        if(!isPlayerDead)
        {
            var horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

            // Move our character
            controller.Move(horizontalMove * Time.fixedDeltaTime, jump, isOnLadder);
            jump = false;
        }
    }

    #region EventManager
    private void PlayerDead()
    {
        isPlayerDead = true;
        EventManager.StartListening(Names.Events.Respawn, Respawn);
    }
    private void Respawn()
    {
        isPlayerDead = false;
        EventManager.StopListening(Names.Events.Respawn, Respawn);
    }
    #endregion
}