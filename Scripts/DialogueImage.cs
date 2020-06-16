using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueImage : MonoBehaviour
{
    public static DialogueImage instance = null;
    private Image image;
    
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
        image = GetComponent<Image>();
    }
    
    public void SetSprite(Sprite sprite) {
        image.sprite = sprite; 
    }
}
