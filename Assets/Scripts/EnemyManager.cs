using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager inst;
    private void Awake() {
        inst = this;
    }
    public List<EnemyAIController> enemyAIControllers;
    // Start is called before the first frame update
    void Start()
    {
        enemyAIControllers = new();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
}
