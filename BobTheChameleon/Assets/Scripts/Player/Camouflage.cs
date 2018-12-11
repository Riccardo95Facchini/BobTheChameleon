using UnityEngine;

public class Camouflage : MonoBehaviour
{

    [SerializeField]
    private float camouflageTime; //Should last as long as the animation
    //[SerializeField]
    //private Animator animator;

    private bool isCamouflaged;
    private bool startedCamouflage;
    private bool canCamouflage;
    public Animator animator;

    private void OnEnable()
    {
        isCamouflaged = false;
        startedCamouflage = false;
        canCamouflage = false;
    }

    void FixedUpdate()
    {
        //If C is pressed and isn't camouflaged
        if(Input.GetKey(KeyCode.C))
        {
            if(!startedCamouflage && canCamouflage) //If the animation isn't already running
            {
                startedCamouflage = true;
                animator.SetBool("Camo", true);
                Invoke("FinishCamouflage", camouflageTime);
                //animator.SetBool(Names.Animations.CamouflageOn.ToString(), true); //Animation starts immediately, boolean after the invoke
            }
        }
        else if(Input.anyKey)//Any other key breaks the camouflagement
        {
            isCamouflaged = false;
            animator.SetBool("Camo", false);
            startedCamouflage = false;
            //animator.SetBool(Names.Animations.CamouflageOff.ToString(), false);
        }
    }

    /// <summary>
    /// Called after camouflageTime sets the boolean to true
    /// </summary>
    private void FinishCamouflage()
    {
        isCamouflaged = true;
    }

    /// <summary>
    /// Used to enable camouflage only where it's allowed or disable it
    /// </summary>
    /// <param name="value">True if player is in a place where it can camouflage</param>
    public void SetCamouflageFlag(bool value)
    {
        canCamouflage = value;
    }

    public bool IsCamouflaged()
    {
        return isCamouflaged;
    }
}
