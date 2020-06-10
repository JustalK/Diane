using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject gplayer;
    private PlayerMovementsScript player;
    private Queue<string> sentances;
    
    
    void Start() {
        sentances = new Queue<string>();
        player = gplayer.GetComponent<PlayerMovementsScript>();
    }
    
    public void StartDialogue(Dialogue dialogue) {
        player.willReadDialogue();
        anim.SetBool("isOpen",true);
        name.text = dialogue.name;
        
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
        
        StopAllCoroutines();
        StartCoroutine(TypeSentance(sentance));
    }
    
    IEnumerator TypeSentance(string sentance) {
        dialogueText.text = "";
        foreach(char letter in sentance.ToCharArray()) {
            dialogueText.text += letter;
            yield return null;
        }
    }
    
    void EndDialogue() {
        anim.SetBool("isOpen",false);
        player.stopReadDialogue();
    }
}
