using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance = null;
    private Animator animator;
    private Player player;
    private DialogueName dialogueName;
    private DialogueSentance dialogueSentance;
    private DialogueImage dialogueImage;
    private Queue<string> sentances;
    private Sprite[] sprites;
    
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
        player = Player.instance;
        dialogueSentance = DialogueSentance.instance;
        dialogueName = DialogueName.instance;
        dialogueImage = DialogueImage.instance;
        animator = GetComponent<Animator>();
        sentances = new Queue<string>();
    }
    
    public void StartDialogue(Dialogue dialogue) {
        player.willReadDialogue();
        animator.SetBool("isOpen",true);
        dialogueName.SetText(dialogue.name);
        sprites = dialogue.image;
        sentances.Clear();
        
        foreach(string sentance in dialogue.sentances) {
            sentances.Enqueue(sentance);
        }
        
        DisplayNextSentance();
    }
    
    public void DisplayNextSentance() {
        if(sentances.Count == 0) {
            EndDialogue();
            return;
        }

        string sentance = sentances.Dequeue();
        dialogueImage.SetSprite(sprites[sprites.Length - 1 - sentances.Count]);
        
        StopAllCoroutines();
        StartCoroutine(TypeSentance(sentance));
    }
    
    IEnumerator TypeSentance(string sentance) {
        dialogueSentance.SetText("");
        foreach(char letter in sentance.ToCharArray()) {
            dialogueSentance.AddLetter(letter);
            yield return null;
        }
    }
    
    void EndDialogue() {
        animator.SetBool("isOpen",false);
        player.stopReadDialogue();
    }
}
