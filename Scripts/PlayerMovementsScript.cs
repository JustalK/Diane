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
    private float lastHorizontalMove = 1f;
    private bool dashMove = false;
    private float lastDash = 0f;
    private int nbrDash = 0;
    private bool fallMove = false;
    private bool longJumpMove = false;
    private float longJumpTime = 1f;
    private bool playerOnTheGround = true;
    public LayerMask whatIsGrounded;
    private bool canMove = true;
    private SpriteRenderer sprite;
    
    private bool isTooHigh=false;
    private bool isJumping=false;
    
    private bool keyJump=false;
    private bool keyFall=false;
    private bool keyLeft=false;
    private bool keyRight=false;
    private bool keyDash=false;
    
    private float timeInAir=1f;
    
    void Awake() {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        animCamera = vcam.GetComponent<Animator>();
    }
    
    void Update()
    {
        direction = 0;
        
        if(canMove) {    
            direction = Input.GetAxisRaw("Horizontal");
            
            // If the user press horizontal and only one key at the time
            if(Input.GetButton("Horizontal")) {
                if(direction==1) keyRight=true;
                if(direction==-1) keyLeft=true;
                //if(playerOnTheGround) Instantiate(dust,m_feetPosition.transform.position,Quaternion.identity);
            }
            
            if(Input.GetButton("Jump")) {
                keyJump = true;
            }
            
            if(Input.GetButton("Down")) {
                keyFall = true;
            }

            if(Input.GetButton("Dash")) {
                keyDash=true;
            }
        }
        
        if(isPlayerIdle()) playerIdle();
    }
    
    void FixedUpdate() {
        Vector2 targetVelocity = new Vector2(0, m_Rigidbody2D.velocity.y);

        if(canPlayerMovingHorizontal()) targetVelocity = playerMovingHorizontal(targetVelocity);
        if(canPlayerDashing()) targetVelocity = playerDashing(targetVelocity);
        if(canPlayerTakingOff()) targetVelocity = playerTakingOff(targetVelocity);
        //if(canPlayerJumping()) targetVelocity = playerJumping(targetVelocity);
        //if(canPlayerFalling()) targetVelocity = playerFalling(targetVelocity);
        
        m_Rigidbody2D.velocity = Vector2.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
        
        frameResetControl();
    }
    
    private void frameResetControl() {
        keyRight = false;
        keyLeft = false;
        keyFall = false;
        keyJump = false;
        keyDash = false;
    }

    private void playerIdle() {
        anim.SetBool("takeoff",false);
        anim.SetBool("newJump",false);
        anim.SetBool("falloff",false);
        anim.SetBool("isRunning",false);
    }
    
    private Vector2 playerMovingHorizontal(Vector2 targetVelocity) {
        if(playerOnTheGround) anim.SetBool("isRunning",true);
        float d = keyRight ? 1 : -1;
        Flip(d);
        
        targetVelocity.x = d * m_MovementSpeed * Time.fixedDeltaTime;
        
        return targetVelocity;
    }
    
    private Vector2 playerDashing(Vector2 targetVelocity) {
        anim.SetTrigger("isDashing");
        if(!playerOnTheGround) nbrDash++;
        targetVelocity.x = direction*m_MovementSpeed*m_DashForce*Time.fixedDeltaTime;
        lastDash=Time.time;
        
        return targetVelocity;
    }
    
    // The player is taking off
    private Vector2 playerTakingOff(Vector2 targetVelocity) {
        targetVelocity.y = m_JumpForce;
        isJumping=true;
        Debug.Log(targetVelocity.y);
        
        anim.SetBool("takeoff",true);
        return targetVelocity;
    }
    
    // The player is jumping
    private Vector2 playerJumping(Vector2 targetVelocity) {
        targetVelocity.y = targetVelocity.y-Physics2D.gravity.y*Time.fixedDeltaTime;
        timeInAir-=Time.fixedDeltaTime;
        anim.SetBool("isJumping",true);
        
        return targetVelocity;
    }
    
    // The player is falling 
    private Vector2 playerFalling(Vector2 targetVelocity) {
        if(keyFall) targetVelocity.y *= 1.2f;
        if(m_Rigidbody2D.velocity.y<-20f) isTooHigh=true;
        
        anim.SetBool("newJump",false);
        anim.SetBool("falloff",true);
        
        return targetVelocity;
    }
    
    private bool isPlayerIdle() {
        return !keyRight && !keyLeft && !keyJump && !keyFall && m_Rigidbody2D.velocity.y==0; 
    }
    
    // Is the player moving left or right ?
    private bool canPlayerMovingHorizontal() {
        return (keyRight || keyLeft) && !(keyRight && keyLeft);
    }
    
    private bool canPlayerDashing() {
        return ((keyDash && keyRight) || (keyDash && keyLeft)) && nbrDash<2 && Time.time-lastDash>0.4f;
    }
    
    // Is the player taking off ?
    private bool canPlayerTakingOff() {
        return playerOnTheGround && keyJump && !isJumping;
    }
    
    // Is the player jumping ?
    private bool canPlayerJumping() {
        return !playerOnTheGround && keyJump && timeInAir<=1f;
    }
    
    // Is the player falling ?
    private bool canPlayerFalling() {
        return m_Rigidbody2D.velocity.y<0 || (keyFall && !playerOnTheGround);
    }
    
    private void Flip(float d)
    {
        sprite.flipX=d==-1;
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
        dashMove = false;
        longJumpMove = false;
    }
    
    public void AllowedToMove() {
        canMove = true;
    }
    
    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.layer != LayerMask.NameToLayer("ground")) return;
    
        if(isTooHigh) animCamera.SetTrigger("shake");
        anim.SetBool("isJumping",false);
        anim.SetBool("falloff",false);
        anim.SetBool("takeoff",false);
        anim.SetBool("newJump",false);
        playerOnTheGround = true;
        nbrDash=0;
        longJumpTime=1f;
        
        isTooHigh = false;
        isJumping = false;
        timeInAir = 1f;
        frameResetControl();
    }
    
    void OnCollisionExit2D(Collision2D col)
    {
        if(col.gameObject.layer != LayerMask.NameToLayer("ground")) return;
        
        anim.SetBool("takeoff",true);
        playerOnTheGround = false;
    }
}
