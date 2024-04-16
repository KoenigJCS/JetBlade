using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    Animator animator;
    EnemyAIController enemyAI;
    int isAttackingHash;
    int xVelHash;
    int zVelHash;
    int isBrawlerHash;
    int isDeadHash;
    // Start is called before the first frame update
    void Start()
    {
        animator=GetComponent<Animator>();     
        enemyAI=GetComponent<EnemyAIController>();  

        isAttackingHash=Animator.StringToHash("isAttacking");
        isBrawlerHash=Animator.StringToHash("isBrawler");
        xVelHash=Animator.StringToHash("VelocityX");  
        zVelHash=Animator.StringToHash("VelocityZ"); 
        isDeadHash=Animator.StringToHash("isDead"); 
        animator.SetBool(isBrawlerHash,enemyAI.isBrawler);
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool(isAttackingHash,enemyAI.isAttacking);
        animator.SetFloat(xVelHash,enemyAI.basicVelocity.x);
        animator.SetFloat(zVelHash,enemyAI.basicVelocity.y);
        animator.SetBool(isDeadHash,enemyAI.isDead);
    }
}
