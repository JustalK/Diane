using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Benediction : MonoBehaviour
{
    private enum TypeTeleportation{ Teleportation, NewLevel }
    [SerializeField] private Transform teleportation;
    [SerializeField] private float liliputian;
    [SerializeField] private float layer;
    [SerializeField] private int level;
    [SerializeField] private TypeTeleportation type;
    
    public float getLiliputian() {
        return liliputian;
    }
    
    public Vector2 getPosition() {
        return new Vector2(teleportation.position.x,teleportation.position.y);
    }
    
    public float getLayer() {
        return layer;
    }

    public string getType() {
        return type.ToString();
    }
    
    public int getLevel() {
        return level;
    }
}
