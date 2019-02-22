using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterController2D controller;
    [SerializeField]
    private float runSpeed = 40f;
    [SerializeField]
    private float sprintModifier = 1.5f;
    [SerializeField] private CapsuleCollider2D bodyCollider;
    [SerializeField] private CapsuleCollider2D bodyOnLadderCollider;
    [SerializeField] private PolygonCollider2D headCollider;

    bool jump = false;
    private bool isPlayerDead;
    private bool isSprinting;
    private bool isOnLadder;

    private void Awake()
    {
        jump = false;
        isPlayerDead = false;
        isOnLadder = false;
        isSprinting = false;
        EventManager.StartListening(Names.Events.PlayerDead, PlayerDead);
    }

    private void OnEnable()
    {
        isOnLadder = false;
    }

    public void SetIsOnLadder(bool v)
    {
        isOnLadder = v;  //Avoids problem with respawn on checkpoint if dead on ladder
        if(v)
        {
            bodyCollider.enabled = false;
            bodyOnLadderCollider.enabled = true;
            headCollider.enabled = false;
        }
        else
        {
            bodyOnLadderCollider.enabled = false;
            bodyCollider.enabled = true;
            headCollider.enabled = true;
        }
    }

    void Update()
    {
        if(Input.GetButtonDown("Jump"))
            jump = true;
        else if(Input.GetButtonDown("Sprint"))
            isSprinting = true;
        else if(Input.GetButtonUp("Sprint"))
            isSprinting = false;
    }

    void FixedUpdate()
    {
        if(!isPlayerDead)
        {
            var horizontalMove = isSprinting ? Input.GetAxisRaw("Horizontal") * runSpeed * sprintModifier : Input.GetAxisRaw("Horizontal") * runSpeed;

            // Move our character
            controller.Move(horizontalMove * Time.fixedDeltaTime, jump, isOnLadder);

            jump = false;
        }
    }

    public void SetRespawn(Vector3 resp)
    {
        this.transform.position = resp; // TODO: only for prototype
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