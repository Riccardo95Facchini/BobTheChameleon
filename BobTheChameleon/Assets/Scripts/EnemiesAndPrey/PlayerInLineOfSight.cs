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

    /// <summary>
    /// Checks if the player was spotted 
    /// </summary>
    /// <param name="playerPosition">Position of the player in the space</param>
    /// <param name="distance">Maximum distance for the raycast</param>
    /// <returns>True if spotted, false otherwise</returns>
    public bool IsPlayerInSight(Vector3 playerPosition, float distance)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position,
                    (playerPosition - transform.position).normalized,
                    distance, WhatIsObstacleAndPlayer);

        if(hit.collider != null && hit.collider.tag == Names.Tags.Player.ToString())
            SpottedCheck(hit.collider);
        else
            spottedBeforeCamouflage = false;

        return spottedBeforeCamouflage;
    }

    /// <summary>
    /// Checks if the player was spotted before camouflaging
    /// </summary>
    /// <param name="hit">Result of the raycast</param>
    /// <returns>true if spotted, false otherwise</returns>
    public bool SpottedCheck(Collider2D hit)
    {
        if(playerState == null)
            playerState = hit.GetComponent<Camouflage>();
        else
            return spottedBeforeCamouflage;

        spottedBeforeCamouflage = !playerState.IsCamouflaged();

        return spottedBeforeCamouflage;
    }

    public bool WasSpottedBeforeCamouflage()
    {
        return spottedBeforeCamouflage;
    }

    public void Reset()
    {
        playerState = null;
        spottedBeforeCamouflage = false;
    }
}
