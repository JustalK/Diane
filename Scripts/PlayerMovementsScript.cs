using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerMovementsScript : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 50f;
    [SerializeField] private float m_LongJumpForce = 2f;
    [SerializeField] private float m_DashForce = 25f;
    [SerializeField] private float m_FallMultiplier = 20f;
    [SerializeField] private float m_JumpMultiplier = 20f;
    [SerializeField] private int m_NbrOfJumpMax = 2;
    [SerializeField] private GameObject m_feetPosition;
    [SerializeField] private GameObject dust;
    [SerializeField] private CinemachineVirtualCamera vcam = null;
    [Range(0, 0.3f)] [SerializeField] private float m_MovementSmoothing = .05f;
    [Range(0, 1000f)] [SerializeField] private float m_MovementSpeed = 500f;
    

    private Animator anim;
    private Animator animCamera;
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
    private bool fallMove = false;
    private bool longJumpMove = false;
    private float longJumpTime = 1f;
    private bool playerOnTheGround = true;
    private bool keyPressed = false;
    public LayerMask whatIsGrounded;
    private bool canMove = true;
    private SpriteRenderer sprite;
    
    private CapsuleCollider2D capsule;
    
    void Awake() {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        animCamera = vcam.GetComponent<Animator>();
        capsule = GetComponent<CapsuleCollider2D>(); 
    }
    
    void Update()
    {
        gameDirection="NO_MOVE";
        direction = 0;
        
        if(canMove) {    
            keyPressed = Input.anyKey;
            direction = Input.GetAxisRaw("Horizontal");
            
            
            if(Input.GetButton("Horizontal")) {
                gameDirection = direction==-1 ? "LEFT":"RIGHT";
                Flip(direction);
                if(playerOnTheGround) anim.SetBool("isRunning",true);
                //if(playerOnTheGround) Instantiate(dust,m_feetPosition.transform.position,Quaternion.identity);
                horizontalMove = true;
                lastHorizontalMove = direction;
            } else {
                anim.SetBool("isRunning",false);
            }
            
            if(Input.GetButtonDown("Jump")) {
                jumpMove = true;
            }
            
            if(Input.GetButton("Jump")) {
                anim.SetBool("isJumping",true);
                longJumpMove=true;
                if(!jumpMove) jumpMove = true;
            }
            
            if(Input.GetButton("Down")) {
                fallMove = true;
            }

            if(Input.GetButton("Dash") && gameDirection!="NO_MOVE" && Time.time-lastDash>0.4f) {
                dashMove=true;
                lastDash=Time.time;
            }
        }
    }
    
    void FixedUpdate() {
        Vector2 targetVelocity = new Vector2(direction * m_MovementSpeed * Time.fixedDeltaTime, m_Rigidbody2D.velocity.y);
        
        if(dashMove && nbrDash<2) {
            anim.SetTrigger("isDashing");
            if(!playerOnTheGround) nbrDash++;
            targetVelocity.x = direction*m_MovementSpeed*m_DashForce*Time.fixedDeltaTime;
        }
        if(jumpMove && playerOnTheGround) {
            targetVelocity.y = m_JumpForce;
            anim.SetBool("takeoff",true);
        }
        if(fallMove && !playerOnTheGround) {
            targetVelocity.y = -m_JumpForce;
        }
        if(longJumpMove && !playerOnTheGround && longJumpTime>0f) {
            targetVelocity.y = m_JumpForce*(longJumpTime+0.2f);
            longJumpTime -= Time.fixedDeltaTime;
            anim.SetBool("newJump",true);
            anim.SetBool("falloff",false);
        }
        targetVelocity.y += GravityMultiplier(Time.fixedDeltaTime);
        if(m_Rigidbody2D.velocity.y<0) {
            anim.SetBool("newJump",false);
            anim.SetBool("falloff",true);
        }
        m_Rigidbody2D.velocity = Vector2.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
        
        horizontalMove = false;
        dashMove = false;
        jumpMove = false;
        longJumpMove = false;
        fallMove = false;
    }
    
    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.layer != LayerMask.NameToLayer("ground")) return;
    
        animCamera.SetTrigger("shake");
        anim.SetBool("isJumping",false);
        anim.SetBool("falloff",false);
        anim.SetBool("takeoff",false);
        anim.SetBool("newJump",false);
        playerOnTheGround = true;
        nbrDash=0;
        longJumpTime=1f;
        fallMove = false;
    }
    
    void OnCollisionExit2D(Collision2D col)
    {
        if(col.gameObject.layer != LayerMask.NameToLayer("ground")) return;
        
        anim.SetBool("takeoff",true);
        playerOnTheGround = false;
    }
    
    private void Flip(float d)
    {
        sprite.flipX=d==1;
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
        canMove = false;
        anim.SetBool("isJumping",false);
        anim.SetBool("falloff",false);
        anim.SetBool("takeoff",false);
        anim.SetBool("isRunning",false);
        m_Rigidbody2D.velocity=Vector2.zero;
        horizontalMove = false;
        dashMove = false;
        jumpMove = false;
        longJumpMove = false;
    }
    
    public void AllowedToMove() {
        canMove = true;
    }
}
