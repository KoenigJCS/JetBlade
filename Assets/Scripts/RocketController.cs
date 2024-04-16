using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class RocketController : MonoBehaviour
{
    [SerializeField] private float rocketSpeed = 10f;
    [SerializeField] private Collider hitCollider;
    [SerializeField] private Collider explodeCollider;
    [SerializeField] private GameObject meshL;
    [SerializeField] private GameObject meshR;
    [SerializeField] private VisualEffect visualEffect;
    bool isExploded = false;
    // Start is called before the first frame update
    void Start()
    {
        explodeCollider.enabled=false;
        Invoke(nameof(Dissapate), 10f);
        visualEffect.enabled=false;
        transform.LookAt(PlayerManager.inst.GetPlayerPos()-new Vector3(0,1,0),Vector3.up);
    }

    // Update is called once per frame
    void Update()
    {
        if(!isExploded)
            transform.position+=rocketSpeed * Time.deltaTime * -1 * transform.forward;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Sword") && !isExploded)
        {
            Destroy(gameObject);
        }
        else if(other.gameObject.CompareTag("Player"))
        {
            PlayerManager.inst.DealDamage(50f);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Sword"))
        {
            Destroy(gameObject);
        }
    }
    int layerMask = ~(1<<8);
    private void FixedUpdate() {
        if(!isExploded)
        {
            if(Physics.Raycast(transform.position,transform.forward*-1,out RaycastHit hit,.3f,layerMask))
            {
                if(!hit.collider.gameObject.CompareTag("Sword"))
                {
                    Explode();
                }
            }
        }

    }


    void Explode()
    {
        isExploded=true;
        visualEffect.enabled=true;
        meshL.SetActive(false);
        meshR.SetActive(false);
        hitCollider.enabled=false;
        explodeCollider.enabled=true;
        Invoke(nameof(Dissapate), 2f);
    }

    void Dissapate()
    {
        Destroy(gameObject);
    }
}
