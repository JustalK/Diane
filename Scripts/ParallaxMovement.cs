﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxMovement : MonoBehaviour
{

    [SerializeField] private Vector2 parallaxEffectMultiplier;
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    private float textureUnitSizeX;
    
    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.instance.transform;
        lastCameraPosition = cameraTransform.position;
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        textureUnitSizeX = texture.width/sprite.pixelsPerUnit;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * parallaxEffectMultiplier.x,deltaMovement.y * parallaxEffectMultiplier.y,transform.position.z);
        lastCameraPosition = cameraTransform.position;
    
        if(Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureUnitSizeX) {
            Debug.Log(textureUnitSizeX);
            float offsetPositionX = (cameraTransform.position.x - transform.position.x) % textureUnitSizeX;
            transform.position = new Vector3(cameraTransform.position.x + offsetPositionX,transform.position.y);
        }
    }
}
