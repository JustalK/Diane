using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private float numberTrigger;
    
    void OnTriggerEnter2D(Collider2D col) {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        numberTrigger--;
        if(numberTrigger==0) Destroy(this);
    }
}
