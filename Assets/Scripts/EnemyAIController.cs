using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIController : MonoBehaviour
{
    [SerializeField] private float agroRadius = 30f;
    [SerializeField] private float meleeRadius = 3f;
    [SerializeField] private float rangedRadius = 45f;
    [SerializeField] private float rocketCooldown = 3f;
    [SerializeField] private int health = 100;
    [SerializeField] private Transform rocketLauncherTransform;
    [SerializeField] private GameObject rocketPrefab;
    [SerializeField] private AudioSource killSound;
    [SerializeField] private Transform torsoTransform;
    float rocketTimer = 0f;
    public bool isAttacking = false;
    EnemyAnimationController animationController;
    bool isAllowedMeshing = true;
    public NavMeshAgent meshAgent;
    public Vector2 basicVelocity;
    [SerializeField] Collider collider;
    public bool isBrawler = true;
    public bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        meshAgent = GetComponent<NavMeshAgent>();
        animationController = GetComponent<EnemyAnimationController>();
        EnemyManager.inst.enemyAIControllers.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(health<=0)
        {   
            if(killSound!=null)
                killSound.Play();
            isDead=true;
            meshAgent.enabled=false;
            Invoke(nameof(ObliterateThis), 10f);
        }
        if(isDead)
            return;
        float dist = (transform.position-PlayerManager.inst.GetPlayerPos()).magnitude;
        basicVelocity = new(Mathf.Cos(Vector3.Angle(meshAgent.velocity,transform.forward)*Mathf.Deg2Rad),Mathf.Cos(Vector3.Angle(meshAgent.velocity,transform.right)*Mathf.Deg2Rad));
        if(isBrawler)
        {
            if(dist<=agroRadius && dist>meleeRadius && meshAgent.enabled)
            {
                meshAgent.SetDestination(PlayerManager.inst.GetPlayerPos());
                isAttacking=false;
            }
            else if(dist<=meleeRadius)
            {
                isAttacking=true;
                collider.enabled=true;
            }
            else
            {
                isAttacking=false;
                collider.enabled=false;
            }
        }
        else
        {
           if(dist<=agroRadius*2 && dist>rangedRadius && meshAgent.enabled)
            {
                isAttacking=false;
                meshAgent.SetDestination(PlayerManager.inst.GetPlayerPos());   
            }
            if(dist<=rangedRadius && meshAgent.enabled)
            {
                meshAgent.SetDestination(transform.position);
                isAttacking=true;
                transform.LookAt(PlayerManager.inst.GetPlayerPos()-new Vector3(0,1,0),Vector3.up);
                transform.eulerAngles = new(0,transform.eulerAngles.y,0);
                //torsoTransform.localEulerAngles.Set(torsoTransform.localEulerAngles.x-90f,torsoTransform.localEulerAngles.y,torsoTransform.localEulerAngles.z);
                //transform.rotation=new Vector3(0,transform.rotation.y,0);
                if(rocketTimer<=0f)
                {
                    Invoke(nameof(FireRocket), rocketCooldown);
                    rocketTimer=rocketCooldown;
                }
            }
            else
            {
                isAttacking=false;
            } 
        }
        if(rocketTimer>0f)
            rocketTimer-=Time.deltaTime;
    }
    Vector3 offset = new(0,0,0);
    public void FireRocket()
    {
        Instantiate(rocketPrefab,rocketLauncherTransform.position+offset,rocketLauncherTransform.rotation);
    }

    readonly int layerMask = ~(1 << 8);

    private void FixedUpdate() {
        if(isAllowedMeshing)
        {
            if(Physics.Raycast(transform.position,Vector3.down,out _,.3f,layerMask))
            {
                meshAgent.enabled=true;
            }
        }

    }

    public void ToggleNav()
    {
        if(isAllowedMeshing)
        {
            isAllowedMeshing=false;
            meshAgent.enabled=false;
            Invoke(nameof(ToggleNav), .5f);
        }
        else
        {
            isAllowedMeshing=true;
        }
    }

    public void TakeDamage(int damage)
    {
        health-=damage;
        if(health<=0)
        {   
            if(killSound!=null)
                killSound.Play();
            isDead=true;
            meshAgent.enabled=false;
            Invoke(nameof(ObliterateThis), 10f);
        }
    }

    public void ObliterateThis()
    {
        Destroy(gameObject);
    }
}
