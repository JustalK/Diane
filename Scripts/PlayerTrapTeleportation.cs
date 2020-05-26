using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrapTeleportation : MonoBehaviour
{
    public Transform teleportation;
    public GameObject player;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag=="Player") {
            StartCoroutine(Teleport());
            StartCoroutine(AllowedPlayerToMove());
        }
    }
    
    IEnumerator Teleport() {
        PlayerMovementsScript playerScript = player.GetComponent<PlayerMovementsScript>();
        playerScript.Stop();
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector2(teleportation.transform.position.x,teleportation.transform.position.y);
    }
    
    IEnumerator AllowedPlayerToMove() {
        yield return new WaitForSeconds(1f);
        PlayerMovementsScript playerScript = player.GetComponent<PlayerMovementsScript>();
        playerScript.AllowedToMove();
    }
}