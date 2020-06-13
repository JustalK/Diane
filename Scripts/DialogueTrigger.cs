using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private float numberBeforeTrigger;
    [SerializeField] private float numberTrigger;
    
    void OnTriggerEnter2D(Collider2D col) {
        if(numberBeforeTrigger>0) {
            numberBeforeTrigger--;
            return;
        }
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        numberTrigger--;
        if(numberTrigger==0) Destroy(this);
    }
}
