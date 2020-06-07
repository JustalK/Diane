using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skills : MonoBehaviour
{
    [SerializeField] private string skill;
    [SerializeField] private float inLayer;
    
    public string getSkill() {
        return skill;
    }
    
    public float getInLayer() {
        return inLayer;
    }
}
