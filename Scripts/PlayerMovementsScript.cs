using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementsScript : MonoBehaviour
{
    [SerializeField] private Transform feetPos;
    [SerializeField] private float m_JumpForce = 75f;
    [SerializeField] private float m_DashForce = 25f;
    [SerializeField] private float m_FallMultiplier = 20f;
    [SerializeField] private float m_JumpMultiplier = 20f;
    [SerializeField] private int m_NbrOfJumpMax = 2;
    [Range(0, 0.3f)] [SerializeField] private float m_MovementSmoothing = .05f;
    [Range(0, 1000f)] [SerializeField] private float m_MovementSpeed = 500f;
    
    private Rigidbody2D m_Rigidbody2D;
    private Vector2 m_Velocity = Vector2.zero;
    private float direction = 0f;
    private string gameDirection = "NO_MOVE";
    private bool horizontalMove = false;
    private bool dashMove = false;
    private float lastDash = 0f;
    private int nbrDash = 0;
    private bool jumpMove = false;
    private bool longJumpMove = false;
    private float longJumpTime = 1f;
    private int nbrJump = 0;
    private bool playerOnTheGround = true;
    private bool keyPressed = false;
    public LayerMask whatIsGrounded;
    
    void Awake() {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        keyPressed = Input.anyKey;
        direction = Input.GetAxisRaw("Horizontal");
        gameDirection="NO_MOVE";
        
        if(Input.GetButton("Horizontal")) {
            gameDirection = direction==-1 ? "LEFT":"RIGHT";
            horizontalMove = true;
        }
        
        if(Input.GetButtonDown("Jump")) {
            jumpMove = true;
            nbrJump++;
        }
        
        // Long Jump
        if(Input.GetButton("Jump")) {
            longJumpMove=true;
        }

        if(Input.GetButton("Dash") && gameDirection!="NO_MOVE" && Time.time-lastDash>0.4f) {
            dashMove=true;
            lastDash=Time.time;
        }
    }
    
    void FixedUpdate() {
        Vector2 targetVelocity = new Vector2(direction * m_MovementSpeed * Time.fixedDeltaTime, m_Rigidbody2D.velocity.y);
        
        if(dashMove && nbrDash<2) {
            if(!playerOnTheGround) nbrDash++;
            targetVelocity.x = direction*m_MovementSpeed*m_DashForce*Time.fixedDeltaTime;
        }
        
        if(jumpMove && playerOnTheGround) targetVelocity.y = m_JumpForce;
        if(jumpMove && !playerOnTheGround && nbrJump<=m_NbrOfJumpMax) {
            targetVelocity.y = Mathf.Abs(m_Rigidbody2D.velocity.y)+m_JumpForce;
            m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x,0);
        }
        if(longJumpMove && !playerOnTheGround && longJumpTime>0) {
            targetVelocity.y += 10f;
            longJumpTime -= Time.fixedDeltaTime;
        }
        targetVelocity.y += GravityMultiplier(Time.fixedDeltaTime);
        
        m_Rigidbody2D.velocity = Vector2.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
        
        horizontalMove = false;
        dashMove = false;
        jumpMove = false;
        longJumpMove = false;
    }
    
    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.layer == LayerMask.NameToLayer("ground")) {
            bool feetCollision = false;
            // Search where is the collision on y
            foreach (ContactPoint2D missileHit in col.contacts)
            {
                feetCollision = col.otherCollider.transform.position.y-missileHit.point.y>0.4;
            }
            // If the collition is on the feet
            if(feetCollision) {                
                playerOnTheGround = true;
                nbrJump=0;
                nbrDash=0;
                longJumpTime=1f;
            }
            
        }
    }
    
    void OnCollisionExit2D(Collision2D col)
    {
        playerOnTheGround = false;
    }
    
    /**
     * The multiplier that will affect the falling and the jumping by creating an accelation and deceleration coefficient
     * @param float time The update time of the function
     */
    float GravityMultiplier(float time) {
        if(m_Rigidbody2D.velocity.y>0) return Physics2D.gravity.y * (m_JumpMultiplier - 1) * time;
        if(m_Rigidbody2D.velocity.y<0) return Physics2D.gravity.y * (m_FallMultiplier - 1) * time;
        return 0;
    }
}
