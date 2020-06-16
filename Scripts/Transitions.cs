using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class Transitions : MonoBehaviour
{
    public static Transitions instance = null;
    private Animator animator;
    private UnityEngine.Experimental.Rendering.Universal.Light2D light;
    
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
        light = GetComponentInChildren<UnityEngine.Experimental.Rendering.Universal.Light2D>();
        animator = GetComponent<Animator>();
    }
    
    public void Glowing() {
        animator.SetBool("isGlowing",true);
    }
}
