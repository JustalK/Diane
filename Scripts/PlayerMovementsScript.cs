using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementsScript : MonoBehaviour
{
    [Range(0, 0.3f)] [SerializeField] private float m_MovementSmoothing = .05f;
    [Range(0, 1000f)] [SerializeField] private float m_MovementSpeed = 500f;
    [Range(0, 1000f)] [SerializeField] private float m_MaxSpeed = 1000f;
    
    private Rigidbody2D m_Rigidbody2D;
    private Vector3 m_Velocity = Vector3.zero;
    float horizontalMove = 0f;
    bool keyPressed = false;
    
    void Awake() {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }
    
    // Update is called once per frame
    void Update()
    {
        keyPressed = Input.anyKey;
        horizontalMove = Input.GetAxisRaw("Horizontal");
    }
    
    void FixedUpdate() {
        Vector3 targetVelocity = new Vector2(horizontalMove * m_MovementSpeed * Time.fixedDeltaTime, m_Rigidbody2D.velocity.y);
        m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing, m_MaxSpeed);
    }
}
