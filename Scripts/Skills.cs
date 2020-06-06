using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skills : MonoBehaviour
{
    [SerializeField] private string skill;
    
    public string getSkill() {
        return skill;
    }
}
