using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementsScript : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400f;
    [Range(0, 0.3f)] [SerializeField] private float m_MovementSmoothing = .05f;
    [Range(0, 1000f)] [SerializeField] private float m_MovementSpeed = 500f;
    [Range(0, 1000f)] [SerializeField] private float m_MaxSpeed = 1000f;
    
    private Rigidbody2D m_Rigidbody2D;
    private Vector2 m_Velocity = Vector2.zero;
    float direction = 0f;
    bool horizontalMove = false;
    bool jumpMove = false;
    bool jumpActive = true;
    bool playerOnTheGround = true;
    bool keyPressed = false;
    int nbrJump = 0;
    
    void Awake() {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }
    
    // Update is called once per frame
    void Update()
    {
        keyPressed = Input.anyKey;
        direction = Input.GetAxisRaw("Horizontal");
        
        horizontalMove = Input.GetButtonDown("Horizontal") ? true : horizontalMove;
        jumpMove = Input.GetButtonDown("Jump") ? true : jumpMove;
        playerOnTheGround = m_Rigidbody2D.velocity.y==0f;
        jumpActive = playerOnTheGround || nbrJump<2;
        if(playerOnTheGround) nbrJump=0;
        if(!playerOnTheGround && jumpMove) {
            nbrJump++;
        }
    }
    
    void FixedUpdate() {
        Vector2 targetVelocity = new Vector2(direction * m_MovementSpeed * Time.fixedDeltaTime, m_Rigidbody2D.velocity.y);
        
        if(jumpMove && jumpActive) m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        
        m_Rigidbody2D.velocity = Vector2.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing, m_MaxSpeed);
        
        horizontalMove = false;
        jumpMove = false;
    }
}
