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
        }
    }
    
    IEnumerator Teleport() {
        yield return new WaitForSeconds(0.2f);
        player.transform.position = new Vector2(teleportation.transform.position.x,teleportation.transform.position.y);
        PlayerMovementsScript playerScript = player.GetComponent<PlayerMovementsScript>();
        playerScript.stop();
    }
}