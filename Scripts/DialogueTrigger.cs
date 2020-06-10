using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private Dialogue dialogue;
    
    void OnTriggerEnter2D(Collider2D col) {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }
}
