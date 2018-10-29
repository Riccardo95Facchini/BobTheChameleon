using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    [Tooltip("Sets the modifier for the gravity, default is 1, half gravity is 0.5, ecc..")]
    public float gravityModifier = 1f;

    public float minGroundNormalY = 0.65f;

    protected Vector2 targetVelocity;
    protected bool grounded;
    protected Vector2 groundNormal;
    protected Vector2 velocity;
    protected Rigidbody2D rigidBody2D;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

    //If the distance moved from the last frame is less than this value, don't move.
    protected const float minMoveDistance = 0.001f;
    //Distance padding for collider casting.
    protected const float shellRadius = 0.01f;

    private void OnEnable()
    {
        //Caching
        rigidBody2D = GetComponent<Rigidbody2D>();
    }

    //Use this for initialization
    void Start()
    {
        contactFilter.useTriggers = false;
        /* Sets the layer mask as the one of this object, it can be seen in Edit -> Project Settings -> Physics 2D.
         * If the layer isn't modified it uses the default one. */
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
    }

    private void Update()
    {
        targetVelocity = Vector2.zero;
        ComputeVelocity();
    }

    protected virtual void ComputeVelocity() { }

    void FixedUpdate()
    {
        velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
        velocity.x = targetVelocity.x;

        //Until a collision is detected we always consider the object not touching the ground
        grounded = false;

        Vector2 deltaPosition = velocity * Time.deltaTime;

        /* The ground normal points away from the ground, using the two components swapped 
         * with and the x negative results in a new vector perpendicular to the original one.
         */
        Vector2 moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);

        //Calculate horizontal movement first
        Vector2 move = moveAlongGround * deltaPosition.x;

        Movement(move, false);

        //Calculate vertical movement
        move = Vector2.up * deltaPosition.y;

        Movement(move, true);
    }

    /* Movement function, it separates the x and y axis components
     * and handles them separately in order to better deal with slopes
     * (even if we probably won't have them)
     */
    void Movement(Vector2 move, bool yMovement)
    {
        //How much we want to move
        float distance = move.magnitude;

        if(distance > minMoveDistance)
        {
            /* Casts the shape of the collider to check for collisions.
             * direction: Direction of the casting.
             * hitBuffer: Array storing the results of the casting.
             * distance: Maximum distance of the casting.
             * contactFilter: Used to filter the results.
             * Return: int -> Number of contacts made
             */
            int count = rigidBody2D.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
            hitBufferList.Clear();

            for(int i = 0; i < count; i++)
                hitBufferList.Add(hitBuffer[i]);

            for(int i = 0; i < hitBufferList.Count; i++)
            {
                Vector2 currentNormal = hitBufferList[i].normal;

                //Checks if the object is standing on the ground.
                if(currentNormal.y > minGroundNormalY)
                {
                    grounded = true;
                    if(yMovement)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot(velocity, currentNormal);

                if(projection < 0)
                {
                    //Cancels out the part of the velocity blocked by the collision.
                    velocity = velocity - projection * currentNormal;
                }

                float modifiedDistance = hitBufferList[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }
        }

        rigidBody2D.position = rigidBody2D.position + move.normalized * distance;
    }
}
