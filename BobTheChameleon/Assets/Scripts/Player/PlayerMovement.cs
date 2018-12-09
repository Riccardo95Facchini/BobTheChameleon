using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterController2D controller;
    [SerializeField]
    private float runSpeed = 40f;
    [SerializeField]
    private float sprintModifier = 1.5f;

    bool jump = false;
    private bool isPlayerDead;

    private bool isOnLadder;

    private void Awake()
    {
        jump = false;
        isPlayerDead = false;
        isOnLadder = false;
        EventManager.StartListening(Names.Events.PlayerDead, PlayerDead);
    }

    public void SetIsOnLadder(bool v)
    {
        isOnLadder = v;
    }

    void Update()
    {
        if(Input.GetButtonDown("Jump"))
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