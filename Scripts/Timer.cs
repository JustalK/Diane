using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    public static Timer instance = null;
    private TextMeshProUGUI textMeshPro;
    private float time=0f;
    private bool startTime=false;
    
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
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }
    
    void Update() {
        if(startTime) {
            time+=Time.deltaTime;
            textMeshPro.text=FormatTime(time);
        }
    }

    public string FormatTime( float time )
    {
        int minutes = (int) time / 60 ;
        int seconds = (int) time - 60 * minutes;
        int milliseconds = (int) (1000 * (time - minutes * 60 - seconds));
        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds );
    }

    public void StartTime() {
        startTime=true;
    }
    
    public void ResetTime() {
        time=0f;
    }
}