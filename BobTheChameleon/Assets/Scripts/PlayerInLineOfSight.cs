using UnityEngine;

public class PlayerInLineOfSight : MonoBehaviour
{
    [SerializeField]
    private LayerMask WhatIsObstacleAndPlayer;

    private Camouflage playerState;

    private bool spottedBeforeCamouflage;

    private void Awake()
    {
        playerState = null;
        spottedBeforeCamouflage = false;
    }

    public bool IsPlayerInSight(Vector3 playerPosition, float distance)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position,
                    (playerPosition - transform.position).normalized,
                    distance, WhatIsObstacleAndPlayer);

        if(hit.collider != null && hit.collider.tag == Names.Tags.Player.ToString())
        {
            if(playerState == null)
                playerState = hit.collider.GetComponent<Camouflage>();

            if(!playerState.IsCamouflaged())
                spottedBeforeCamouflage = true;

            return spottedBeforeCamouflage;
        }
        else
        {
            spottedBeforeCamouflage = false;
            return false;
        }
    }
}
