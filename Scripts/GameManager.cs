using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    private Player player;
    private Timer timer;
    private Transitions transitions;
    
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
        timer = Timer.instance;
        transitions = Transitions.instance;
    }
    
    void Update() {
        if(player.IsPlayerMadeOneMove() && !timer.IsTimeStarted()) timer.StartTime(); 
    }
    
    public void ActionBenediction(GameObject obj) {
        if(obj.layer== LayerMask.NameToLayer("LoadingNewLevel")) LoadingNewLevel(obj);
        if(obj.layer== LayerMask.NameToLayer("Teleportation")) LoadingNewLevel(obj);
    }
    
    private void LoadingNewLevel(GameObject obj) {
        LoadingNewLevel lnl = obj.GetComponent<LoadingNewLevel>();
        if(lnl.GetTransition()=="Glowing") transitions.Glowing();
        StartCoroutine(changeNewLevel(3,lnl.GetLevel()));
    }
    
    IEnumerator changeNewLevel(float seconds,int level) {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene(level);
    }
    
}
