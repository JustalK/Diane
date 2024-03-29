﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Message : MonoBehaviour
{
    public static Message instance = null;
    private TextMeshProUGUI title;
    private TextMeshProUGUI description;
    private Animator animator;
    
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
        TextMeshProUGUI[] tmp=GetComponentsInChildren<TextMeshProUGUI>();
        title = tmp[0];
        description = tmp[1];
        animator = GetComponent<Animator>();
    }
    
    public void SetTitleText(string text) {
        title.text=text;
    }

    public void SetDescriptionText(string text) {
        description.text=text;
    }   
    
    public void SetIsShowing(bool value) {
        animator.SetBool("isShowing",value);
    }   
    
    public TextMeshProUGUI GetTitle() {
        return title;
    }
    
    public TextMeshProUGUI GetDescription() {
        return description;
    }
    
    public Animator GetAnimator() {
        return animator;
    }
}
