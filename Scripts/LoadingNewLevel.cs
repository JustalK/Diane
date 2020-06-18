using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingNewLevel : MonoBehaviour
{
    [SerializeField] private int level;
    [SerializeField] private string transition;
    
    public int GetLevel() {
        return level;
    }
    
    public string GetTransition() {
        return transition;
    }
}
