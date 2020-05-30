using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTraps : MonoBehaviour
{
    
    [SerializeField] private float speedFall;
    [SerializeField] private float speedReset;

    private bool isDown = false;
    private float yPositionStart;
    private Rigidbody2D rb2D;
    
    void Awake() {
        rb2D = GetComponent<Rigidbody2D>();
        yPositionStart = transform.position.y;
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if(isDown) {
            transform.position = new Vector2(transform.position.x,transform.position.y+speedReset*Time.fixedDeltaTime);
            if(transform.position.y>=yPositionStart) isDown=false;
        } else {
            rb2D.AddForce(new Vector2(0f, -speedFall*Time.fixedDeltaTime));            
        }
    }
    
    void OnCollisionEnter2D(Collision2D col)
    {
        isDown=true;
    }
}
