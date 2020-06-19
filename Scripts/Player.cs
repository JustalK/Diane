using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
﻿using System;
using TMPro;

// Inverse gravity ?
// Ghost (Pass through wall)
// Layer like Abe

public class Player : MonoBehaviour
{
    public static Player instance = null;
    private GameManager gameManager;
    [SerializeField] private float m_DashForce = 25f;
    [SerializeField] private GameObject m_feetPosition;
    [SerializeField] private float m_MovementSmoothing = 0.05f;
    [SerializeField] private float m_MovementSpeed = 800f;
    private CinemachineVirtualCamera vcam;
    private Message message;
    private float m_jumpingForce = 20f;
    private float m_fallingForce = 4f;
    private float m_accelerationFallingForce = 10f;
    
    private float m_timeJump = 0.25f;
    private float m_timePower = 0.5f;
    private float m_readingDialogue = 0.5f;
    
    private float m_smallSizeLayer0 = 0.5f;
    private Rigidbody2D m_Rigidbody2D;
    
    private Vector2 m_Velocity = Vector2.zero;

    private Animator anim;
    private Animator animCamera;
    private float direction = 0f;
    private bool dashMove = false;
    private float lastDash = 0f;
    private int nbrDash = 0;
    private SpriteRenderer sprite;
    
    private bool isReadingDialogue=false;
    private bool isTooHigh=false;
    private bool isKeyReleaseInAction=false;
    private bool isJumping=false;
    private bool isTakingOff=false;
    private bool isFalling=false;
    private bool isDoubleJumping=false;
    private bool isOnTheGround = false;
    private bool isLiliputian = false;
    private bool isAllowedToMove = true;
    private bool isAllowedToBenediction = false;
    private bool isMessageWaitingForJump = false;
    private bool isMessageWaitingForBenediction = false;
    
    private bool hasLeft;
    private bool hasRight;
    private bool hasJump;
    private bool hasLiliputian;
    private bool hasBenediction;
    private bool hasMadeOneMove;
    
    private bool keyJump=false;
    private bool keyFall=false;
    private bool keyLeft=false;
    private bool keyRight=false;
    private bool keyDash=false;
    private bool keyNextDialogue=false;
    private bool keyPower=false;
    private bool keyBenediction=false;
    private bool keyUpdate=false;
    
    private float timeInAir;
    private float timeLastPower=0f;
    private float timeLastReadingDialogue=0f;

    private float inLayer;
    private GameObject inCurrentBenediction;
    private float normalSize;
    
    void Awake() {
        if(instance == null) {
            instance = this;
            return;
        }
        if(instance != this) {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start() {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        animCamera = Camera.instance.GetAnimator();
        message = Message.instance;
        gameManager = GameManager.instance;
        
        hasLeft = gameManager.isPlayerHasLeft();
        hasRight = gameManager.isPlayerHasRight();
        hasJump = gameManager.isPlayerHasJump();
        hasLiliputian = gameManager.isPlayerHasLiliputian();
        hasBenediction = gameManager.isPlayerHasBenediction();
        
        timeInAir=m_timeJump;
        normalSize=this.transform.localScale.x;
        inLayer=0;
    }
    
    void Update()
    {
        resetControl();
        direction = 0;
        
        // If the player is reading
        if(isReadingDialogue) { 
            if(Input.GetButton("Dash")) {
                keyNextDialogue=true;
            }
        }
        
        // If the player is playing the game
        if(isAllowedToMove) {    
            direction = Input.GetAxisRaw("Horizontal");
            
            // If the user press horizontal and only one key at the time
            if(Input.GetButton("Horizontal")) {
                if(direction==1) keyRight=true;
                if(direction==-1) keyLeft=true;
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
            
            if(Input.GetButton("Benediction")) {
                keyBenediction=true;
            }
        }
        
        if(isPlayerIdle()) playerIdle();
        if(isPlayerMoving()) hasMadeOneMove=true;
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
        keyBenediction = false;
        keyNextDialogue = false;
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
            if(hasBenediction && canPlayerBenediction()) playerBenediction();
            if(canReadNextDialogue()) readNextDialogue();
            
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
        
        if(isMessageWaitingForJump) playerExecutingAction();
        
        anim.SetBool("takeoff",false);
        anim.SetBool("isJumping",true);
    }
    
    // The player is falling 
    private Vector2 playerFalling(Vector2 targetVelocity) {
        if(m_Rigidbody2D.velocity.y<-20f) isTooHigh=true;
        isFalling = true;
        
        Debug.Log("falling");
        
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

    private void playerBenediction() {

        if(isMessageWaitingForBenediction) playerExecutingAction();
        isAllowedToMove = false;
        anim.SetBool("benediction",true);
        GameManager.instance.ActionBenediction(inCurrentBenediction);
    }
    
    private void playerLiliputian() {
        timeLastPower=Time.time;
        isLiliputian=!isLiliputian;
        if(isLiliputian) playerSmallSize();
        else playerNormalSize();
    }
    
    private void changeTriggerCollisionGround(string name,bool isTrigger) {
        GameObject[] gameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        GameObject grounds = Array.Find(gameObjects,x => x.name==name);
        EdgeCollider2D[] allChildren = grounds.GetComponentsInChildren<EdgeCollider2D>();
        foreach (EdgeCollider2D collider in allChildren) {
            collider.isTrigger=isTrigger;
        }
    }
    
    private void playerSmallSize() {
        this.transform.position=new Vector2(this.transform.position.x,this.transform.position.y+normalSize/2);
        this.transform.localScale=new Vector2(normalSize/2,normalSize/2);
    }

    private void playerNormalSize() {
        this.transform.position=new Vector2(this.transform.position.x,this.transform.position.y-m_smallSizeLayer0);
        this.transform.localScale=new Vector2(normalSize,normalSize);
    }    
    
    private float playerSmallByLayer() {
        if(inLayer==0) return m_smallSizeLayer0;
        return m_smallSizeLayer0;
    }
    
    private bool isPlayerIdle() {
        return !keyRight && !keyLeft && !keyJump && !keyFall && !isJumping && !isFalling; 
    }

    private bool isPlayerMoving() {
        return keyRight || keyLeft || keyJump || keyFall; 
    }
    
    private bool isPlayerIdleInAction() {
        return !keyJump && !keyFall && (isJumping || isFalling); 
    }
    
    public bool IsPlayerMadeOneMove() {
        return hasMadeOneMove;
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
  
    private bool canPlayerBenediction() {
        return keyBenediction && isAllowedToBenediction;
    }      
    
    private bool canReadNextDialogue() {
        return keyNextDialogue && isReadingDialogue && Time.time-timeLastReadingDialogue>m_readingDialogue;
    }
    
    public void willReadDialogue() {
        timeLastReadingDialogue=Time.time;
        isReadingDialogue=true;
        isAllowedToMove=false;   
    }

    public void stopReadDialogue() {
        timeLastReadingDialogue=0f;
        isReadingDialogue=false;
        isAllowedToMove=true;   
    }
    
    private void readNextDialogue() {
        timeLastReadingDialogue=Time.time;
        FindObjectOfType<DialogueManager>().DisplayNextSentance();
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
        if(col.gameObject.layer == LayerMask.NameToLayer("Skill")) collisionSkill(col);
        if(col.gameObject.layer == LayerMask.NameToLayer("Teleportation")) collisionBenediction(col);
        if(col.gameObject.layer == LayerMask.NameToLayer("LoadingNewLevel")) collisionBenediction(col);
    }
    
    private void collisionSkill(Collider2D col) {
        Skills skill = col.gameObject.GetComponent<Skills>();
        if(skill.getInLayer()!=inLayer) return;
        if(skill.getSkill()=="Jump") {
            hasJump = true;
            isMessageWaitingForJump = true;
        }
        if(skill.getSkill()=="Benediction") {
            hasBenediction = true;
            isMessageWaitingForBenediction = true;
        }
        message.SetTitleText(skill.getTitle()); 
        message.SetDescriptionText(skill.getDescription());
        message.SetIsShowing(true);
        Destroy(col.gameObject);
        return;
    }
    
    private void playerExecutingAction() {
        isMessageWaitingForJump=false;
        message.SetIsShowing(false);
    }
    
    private void collisionBenediction(Collider2D col) {
        isAllowedToBenediction=true;
        inCurrentBenediction=col.gameObject;
    }
    
    void OnTriggerExit2D(Collider2D col) {
        if(col.gameObject.layer == LayerMask.NameToLayer("Teleportation")) exitBenediction(col);
        if(col.gameObject.layer == LayerMask.NameToLayer("LoadingNewLevel")) exitBenediction(col);
    }
    
    private void exitBenediction(Collider2D col) {
        isAllowedToBenediction=false;
        inCurrentBenediction=null;
    }
    
    void OnCollisionEnter2D(Collision2D col)
    {
        if((inLayer==0 && col.gameObject.layer != LayerMask.NameToLayer("Ground0")) || 
           (inLayer==1 && col.gameObject.layer != LayerMask.NameToLayer("Ground1"))) {
            return;
        }
    
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
        if((inLayer==0 && col.gameObject.layer != LayerMask.NameToLayer("Ground0")) || 
           (inLayer==1 && col.gameObject.layer != LayerMask.NameToLayer("Ground1"))) {
           return;
        }
        
        anim.SetBool("takeoff",true);
        isOnTheGround = false;
    }
}
