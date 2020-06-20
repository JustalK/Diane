using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;
using UnityEngine.SceneManagement;

public class Transitions : MonoBehaviour
{
    public static Transitions instance = null;
    [SerializeField] private Transition startTransition;
    [SerializeField] private Transition endTransition;
    [SerializeField] private int nextLevel;
    private enum Transition{ Default, Glowing };
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

    public void StartAnimations() {
        switch(startTransition) 
        {
            case Transition.Glowing:
                StartAnimation(1f,"isDeglowing");
                break;
            default:
                break;
        }
    }

    private void StartAnimation(float seconds,string animation) {
        Debug.Log(seconds);
        Debug.Log(animator);
        animator.SetBool(animation,true);
        StartCoroutine(BackToIdle(seconds,animation));
    }

    IEnumerator BackToIdle(float seconds,string animation) {
        yield return new WaitForSeconds(seconds);
        animator.SetBool(animation,false);
    }
    
    public void EndAnimations() {
        switch(endTransition) 
        {
            case Transition.Glowing:
                EndAnimation(3,"isGlowing");
                break;
            default:
                break;
        }
    }
    
    private void EndAnimation(float seconds,string animation) {
        animator.SetBool(animation,true);
        StartCoroutine(WaitBeforeLoadingNextLevel(seconds));
    }
    
    IEnumerator WaitBeforeLoadingNextLevel(float seconds) {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene(nextLevel);
    }
}
