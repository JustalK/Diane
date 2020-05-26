using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementsScript : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 50f;
    [SerializeField] private float m_LongJumpForce = 2f;
    [SerializeField] private float m_DashForce = 25f;
    [SerializeField] private float m_FallMultiplier = 20f;
    [SerializeField] private float m_JumpMultiplier = 20f;
    [SerializeField] private int m_NbrOfJumpMax = 2;
    [Range(0, 0.3f)] [SerializeField] private float m_MovementSmoothing = .05f;
    [Range(0, 1000f)] [SerializeField] private float m_MovementSpeed = 500f;
    

    private Animator anim;
    private Rigidbody2D m_Rigidbody2D;
    private Vector2 m_Velocity = Vector2.zero;
    private float direction = 0f;
    private string gameDirection = "NO_MOVE";
    private bool horizontalMove = false;
    private float lastHorizontalMove = 1f;
    private bool dashMove = false;
    private float lastDash = 0f;
    private int nbrDash = 0;
    private bool jumpMove = false;
    private bool longJumpMove = false;
    private float longJumpTime = 1f;
    private bool playerOnTheGround = true;
    private bool keyPressed = false;
    public LayerMask whatIsGrounded;
    private bool canMove = true;
    
    void Awake() {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    
    void Update()
    {
        gameDirection="NO_MOVE";
        direction = 0;
        anim.SetBool("isRunning",false);
        
        if(canMove) {    
            keyPressed = Input.anyKey;
            direction = Input.GetAxisRaw("Horizontal");
            
            
            if(Input.GetButton("Horizontal")) {
                gameDirection = direction==-1 ? "LEFT":"RIGHT";
                Flip(direction);
                anim.SetBool("isRunning",true);
                horizontalMove = true;
                lastHorizontalMove = direction;
            }
            
            if(Input.GetButtonDown("Jump")) {
                anim.SetBool("isJumping",true);
                jumpMove = true;
            }
            
            if(Input.GetButton("Jump")) {
                longJumpMove=true;
            }
    
            if(Input.GetButton("Dash") && gameDirection!="NO_MOVE" && Time.time-lastDash>0.4f) {
                dashMove=true;
                lastDash=Time.time;
            }
        }
        if(playerOnTheGround) {
            anim.SetBool("takeoff",false);
        } else {
            anim.SetBool("takeoff",true);
        }
    }
    
    void FixedUpdate() {
        Vector2 targetVelocity = new Vector2(direction * m_MovementSpeed * Time.fixedDeltaTime, m_Rigidbody2D.velocity.y);
        
        if(dashMove && nbrDash<2) {
            if(!playerOnTheGround) nbrDash++;
            targetVelocity.x = direction*m_MovementSpeed*m_DashForce*Time.fixedDeltaTime;
        }
        if(jumpMove && playerOnTheGround) {
            targetVelocity.y = m_JumpForce;
        }
        if(longJumpMove && !playerOnTheGround && longJumpTime>0f) {
            targetVelocity.y = m_JumpForce*(longJumpTime+0.2f);
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
            /**
            bool feetCollision = false;
            // Search where is the collision on y
            foreach (ContactPoint2D missileHit in col.contacts)
            {
                feetCollision = col.otherCollider.transform.position.y-missileHit.point.y>0.4;
            }
            // If the collition is on the feet
            if(feetCollision) { 
            **/               
                playerOnTheGround = true;
                anim.SetBool("isJumping",false);
                anim.SetBool("takeoff",false);
                nbrDash=0;
                longJumpTime=1f;
            //}
            
        }
    }
    
    void OnCollisionExit2D(Collision2D col)
    {
        playerOnTheGround = false;
    }
    
    private void Flip(float d)
    {
        /**
        Vector3 theScale = transform.localScale;
        theScale.x = d*Mathf.Abs(theScale.x);
        transform.localScale = theScale;
        **/
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
    
    public void Stop() {
        m_Rigidbody2D.velocity=Vector2.zero;
        horizontalMove = false;
        dashMove = false;
        jumpMove = false;
        longJumpMove = false;
        canMove = false;
    }
    
    public void AllowedToMove() {
        canMove = true;
    }
}
