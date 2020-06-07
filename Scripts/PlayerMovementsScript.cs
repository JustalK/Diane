using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerMovementsScript : MonoBehaviour
{
    [SerializeField] private float m_DashForce = 25f;
    [SerializeField] private GameObject m_feetPosition;
    [SerializeField] private GameObject dust;
    [SerializeField] private CinemachineVirtualCamera vcam = null;
    [SerializeField] private float m_MovementSmoothing = 0.05f;
    [SerializeField] private float m_MovementSpeed = 800f;
    private float m_jumpingForce = 20f;
    private float m_fallingForce = 4f;
    private float m_accelerationFallingForce = 10f;
    private float m_timeJump = 0.25f;
    private float m_timePower = 0.5f;
    private Rigidbody2D m_Rigidbody2D;
    
    private Vector2 m_Velocity = Vector2.zero;

    private Animator anim;
    private Animator animCamera;
    private float direction = 0f;
    private bool dashMove = false;
    private float lastDash = 0f;
    private int nbrDash = 0;
    private SpriteRenderer sprite;
    
    private bool isTooHigh=false;
    private bool isKeyReleaseInAction=false;
    private bool isJumping=false;
    private bool isTakingOff=false;
    private bool isFalling=false;
    private bool isDoubleJumping=false;
    private bool isOnTheGround = true;
    private bool isLiliputian = false;
    private bool isAllowedToMove = true;
    
    private bool hasLeft=true;
    private bool hasRight=true;
    private bool hasJump=false;
    private bool hasLiliputian=true;
    
    private bool keyJump=false;
    private bool keyFall=false;
    private bool keyLeft=false;
    private bool keyRight=false;
    private bool keyDash=false;
    private bool keyPower=false;
    private bool keyUpdate=false;
    
    private float timeInAir;
    private float timeLastPower=0f;
    
    void Awake() {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        animCamera = vcam.GetComponent<Animator>();
        
        timeInAir=m_timeJump;
    }
    
    void Update()
    {
        resetControl();
        direction = 0;
        
        if(isAllowedToMove) {    
            direction = Input.GetAxisRaw("Horizontal");
            
            // If the user press horizontal and only one key at the time
            if(Input.GetButton("Horizontal")) {
                if(direction==1) keyRight=true;
                if(direction==-1) keyLeft=true;
                //if(isOnTheGround) Instantiate(dust,m_feetPosition.transform.position,Quaternion.identity);
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
            
            if(Input.GetButton("Power")) {
                keyPower=true;
            }
        }
        
        if(isPlayerIdle()) playerIdle();
        if(isPlayerIdleInAction()) isKeyReleaseInAction=true;
        keyUpdate = true;
    }
    
    private void resetControl() {
        keyRight = false;
        keyLeft = false;
        keyFall = false;
        keyJump = false;
        keyDash = false;
        keyPower = false;
    }
    
    void FixedUpdate() {
        if(keyUpdate) {
            Vector2 targetVelocity = new Vector2(0, m_Rigidbody2D.velocity.y);

            if(canPlayerMovingHorizontal()) targetVelocity = playerMovingHorizontal(targetVelocity);
            //if(canPlayerDashing()) targetVelocity = playerDashing(targetVelocity);
            if(hasJump && canPlayerJumping()) playerJumping();
            if(hasJump && canPlayerTakingOff()) targetVelocity = playerTakingOff(targetVelocity);
            if(canPlayerFalling()) targetVelocity = playerFalling(targetVelocity);
            if(hasJump && canPlayerDoubleJumping()) targetVelocity = playerDoubleJumping(targetVelocity);
            if(hasLiliputian && canPlayerLiliputian()) playerLiliputian();
        
            m_Rigidbody2D.velocity = Vector2.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
        }
        keyUpdate=false;
    }

    private void playerIdle() {
        anim.SetBool("takeoff",false);
        anim.SetBool("newJump",false);
        anim.SetBool("falloff",false);
        anim.SetBool("isRunning",false);
        anim.SetBool("isJumping",false);
        m_Rigidbody2D.velocity = new Vector2(0,0);
    }
    
    private Vector2 playerMovingHorizontal(Vector2 targetVelocity) {
        if(isOnTheGround) anim.SetBool("isRunning",true);
        float d = keyRight ? 1 : -1;
        Flip(d);
        
        targetVelocity.x = d * m_MovementSpeed * Time.fixedDeltaTime * this.transform.localScale.x;
        
        return targetVelocity;
    }
    
    private Vector2 playerDashing(Vector2 targetVelocity) {
        anim.SetTrigger("isDashing");
        if(!isOnTheGround) nbrDash++;
        targetVelocity.x = direction*m_MovementSpeed*m_DashForce*Time.fixedDeltaTime*this.transform.localScale.x;
        lastDash=Time.time;
        
        return targetVelocity;
    }
    
    // The player is jumping
    private Vector2 playerTakingOff(Vector2 targetVelocity) {
        m_Rigidbody2D.AddForce(new Vector2(0,m_jumpingForce*this.transform.localScale.y),ForceMode2D.Impulse);
        isJumping=true;
        isTakingOff=true;
        
        anim.SetBool("takeoff",false);
        anim.SetBool("isJumping",true);
        return targetVelocity; 
    }

    private Vector2 playerDoubleJumping(Vector2 targetVelocity) {
        float force=0f;
        if(m_Rigidbody2D.velocity.y<m_jumpingForce) force=m_jumpingForce-m_Rigidbody2D.velocity.y; 
        m_Rigidbody2D.AddForce(new Vector2(0,force*this.transform.localScale.y),ForceMode2D.Impulse);
        isTakingOff=true;
        isFalling = false;
        isDoubleJumping = true;
        timeInAir = 0.25f;
        
        anim.SetBool("takeoff",false);
        anim.SetBool("isJumping",true);
        return targetVelocity;
    }    
    
    private void playerJumping() {
        isJumping=true;
        isTakingOff=false;
        timeInAir-=Time.fixedDeltaTime;
        
        Debug.Log("JUMP");
        
        anim.SetBool("takeoff",false);
        anim.SetBool("isJumping",true);
    }
    
    // The player is falling 
    private Vector2 playerFalling(Vector2 targetVelocity) {
        if(m_Rigidbody2D.velocity.y<-20f) isTooHigh=true;
        isFalling = true;
        
        targetVelocity.y = targetVelocity.y - m_fallingForce*this.transform.localScale.y;
        if(keyFall) {
            targetVelocity.y -= m_accelerationFallingForce*this.transform.localScale.y;
        }
        
        anim.SetBool("takeoff",false);
        anim.SetBool("isJumping",false);
        anim.SetBool("newJump",false);
        anim.SetBool("falloff",true);
        
        return targetVelocity;
    }

    private void playerLiliputian() {
        timeLastPower=Time.time;
        isLiliputian=!isLiliputian;
        if(isLiliputian) playerSmallSize();
        else playerNormalSize();
    }
    
    private void playerSmallSize() {
        this.transform.position=new Vector2(this.transform.position.x,this.transform.position.y+0.5f);
        this.transform.localScale=new Vector2(0.5f,0.5f);
    }

    private void playerNormalSize() {
        this.transform.position=new Vector2(this.transform.position.x,this.transform.position.y-0.5f);
        this.transform.localScale=new Vector2(1f,1f);
    }    
    
    private bool isPlayerIdle() {
        return !keyRight && !keyLeft && !keyJump && !keyFall && !isJumping && !isFalling; 
    }

    private bool isPlayerIdleInAction() {
        return !keyJump && !keyFall && (isJumping || isFalling); 
    }
    
    // Is the player moving left or right ?
    private bool canPlayerMovingHorizontal() {
        return (keyRight || keyLeft) && !(keyRight && keyLeft);
    }
    
    private bool canPlayerDashing() {
        return ((keyDash && keyRight) || (keyDash && keyLeft)) && nbrDash<2 && Time.time-lastDash>0.4f;
    }
    
    // Is the player jumping ?
    private bool canPlayerTakingOff() {
        return isOnTheGround && keyJump && !isJumping;
    }
    
    // Is the player jumping ?
    private bool canPlayerJumping() {
        return !isFalling && isJumping && keyJump && timeInAir>0;
    }
    
    // Is the player falling ?
    private bool canPlayerFalling() {
        return timeInAir<=0 || isFalling || (!keyJump && isJumping) || (!isJumping && !isOnTheGround);
    }

    private bool canPlayerDoubleJumping() {
        return !isDoubleJumping && keyJump && isFalling && isKeyReleaseInAction;
    }  
    
    private bool canPlayerLiliputian() {
        return keyPower && Time.time-timeLastPower>=m_timePower;
    }  
    
    private void Flip(float d)
    {
        sprite.flipX=d==-1;
    }
    
    public void Stop() {
        isAllowedToMove = false;
        anim.SetBool("isJumping",false);
        anim.SetBool("falloff",false);
        anim.SetBool("takeoff",false);
        anim.SetBool("isRunning",false);
        m_Rigidbody2D.velocity=Vector2.zero;
        dashMove = false;
    }
    
    public void AllowedToMove() {
        isAllowedToMove = true;
    }
 
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.layer == LayerMask.NameToLayer("Skill")) {
            Skills skill = col.gameObject.GetComponent<Skills>();
            if(skill.getSkill()=="Jump") hasJump=true;
            Destroy(col.gameObject);
            return;
        };   
    }
    
    void OnCollisionEnter2D(Collision2D col)
    {
        
        if(col.gameObject.layer != LayerMask.NameToLayer("ground")) return;
    
        bool isContactVertical = true;
        foreach (ContactPoint2D contact in col.contacts)
        {
            if(contact.normal.x!=0 && contact.normal.y!=1) isContactVertical=false;
        }
        
        // if the contact is not vertical to the floor
        if(!isContactVertical) return;
        
        if(isTooHigh) animCamera.SetTrigger("shake");
        anim.SetBool("isJumping",false);
        anim.SetBool("falloff",false);
        anim.SetBool("takeoff",false);
        anim.SetBool("newJump",false);
        isOnTheGround = true;
        nbrDash=0;
        
        isTooHigh = false;
        isJumping = false;
        isDoubleJumping=false;
        isKeyReleaseInAction=false;
        isFalling = false;
        timeInAir = m_timeJump;
        resetControl();
    }
    
    void OnCollisionExit2D(Collision2D col)
    {
        if(col.gameObject.layer != LayerMask.NameToLayer("ground")) return;
        
        anim.SetBool("takeoff",true);
        isOnTheGround = false;
    }
}
