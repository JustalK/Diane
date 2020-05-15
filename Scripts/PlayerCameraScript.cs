using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraScript : MonoBehaviour
{
    public Transform player;
    
    void FixedUpdate() {
        transform.position = new Vector3(player.position.x,player.position.y+1.5f,transform.position.z);
    }
}