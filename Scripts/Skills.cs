using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skills : MonoBehaviour
{
    [SerializeField] private string skill;
    [SerializeField] private float inLayer;
    [SerializeField] private string title;
    [SerializeField] private string description;
    
    public string getSkill() {
        return skill;
    }

    public string getTitle() {
        return title;
    }
    
    public string getDescription() {
        return description;
    }
    
    public float getInLayer() {
        return inLayer;
    }
}
