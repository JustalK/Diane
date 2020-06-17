using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Message : MonoBehaviour
{
    public static Message instance = null;
    private TextMeshProUGUI title;
    private TextMeshProUGUI description;
    
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
    }
    
    public void SetTitleText(string text) {
        title.text=text;
    }

    public void SetDescriptionText(string text) {
        description.text=text;
    }    
    
    public TextMeshProUGUI GetTitle() {
        return title;
    }
    
    public TextMeshProUGUI GetDescription() {
        return description;
    }
}
