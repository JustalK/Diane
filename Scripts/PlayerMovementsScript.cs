using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementsScript : MonoBehaviour
{
    [SerializeField] private Transform feetPos;
    [SerializeField] private float m_JumpForce = 15f;
    [SerializeField] private float fallMultiplier = 15f;
    [SerializeField] private float lowJumpMultiplier = 4f;
    [SerializeField] private int m_NbrOfJumpMax = 2;
    [Range(0, 0.3f)] [SerializeField] private float m_MovementSmoothing = .05f;
    [Range(0, 1000f)] [SerializeField] private float m_MovementSpeed = 500f;
    [Range(0, 1000f)] [SerializeField] private float m_MaxSpeed = 1000f;
    
    private Rigidbody2D m_Rigidbody2D;
    private Vector2 m_Velocity = Vector2.zero;
    private float direction = 0f;
    private bool horizontalMove = false;
    private bool jumpMove = false;
    private int nbrJump = 0;
    private bool playerOnTheGround = true;
    private bool keyPressed = false;
    public LayerMask whatIsGrounded;
    
    void Awake() {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }
    
    // Update is called once per frame
    void Update()
    {
        keyPressed = Input.anyKey;
        direction = Input.GetAxisRaw("Horizontal");
        
        horizontalMove = Input.GetButtonDown("Horizontal") ? true : horizontalMove;
        
        if(Input.GetButtonDown("Jump")) {
            jumpMove = true;
            nbrJump++;
            Debug.Log("JUMP : "+nbrJump);
        }
        
    }
    
    void FixedUpdate() {
        Vector2 targetVelocity = new Vector2(direction * m_MovementSpeed * Time.fixedDeltaTime, m_Rigidbody2D.velocity.y);
        
        if(jumpMove && playerOnTheGround) targetVelocity.y = m_JumpForce;
        if(jumpMove && !playerOnTheGround && nbrJump<=m_NbrOfJumpMax) {
            targetVelocity.y = Mathf.Abs(m_Rigidbody2D.velocity.y)+m_JumpForce;
            m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x,0);
        }
        if(m_Rigidbody2D.velocity.y>0) targetVelocity.y += Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        if(m_Rigidbody2D.velocity.y<0) targetVelocity.y += Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        
        m_Rigidbody2D.velocity = Vector2.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing, m_MaxSpeed);
        
        horizontalMove = false;
        jumpMove = false;
    }
    
    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.layer == LayerMask.NameToLayer("ground")) {
            playerOnTheGround = true;
            Debug.Log("RESET");
            nbrJump=0;
        }
    }
    
    void OnCollisionExit2D(Collision2D col)
    {
        playerOnTheGround = false;
    }
}
