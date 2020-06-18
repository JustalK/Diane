using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleportation : MonoBehaviour
{
    [SerializeField] private Transform teleportation;
    [SerializeField] private float liliputian;
    [SerializeField] private float layer;
    
    public float getLiliputian() {
        return liliputian;
    }
    
    public Vector2 getPosition() {
        return new Vector2(teleportation.position.x,teleportation.position.y);
    }
    
    public float getLayer() {
        return layer;
    }
}
