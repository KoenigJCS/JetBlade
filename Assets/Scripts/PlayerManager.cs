using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public float health = 100f;
    public float fuel = 100f;
    public static PlayerManager inst;
    [SerializeField] private GameObject HealthBar;
    Vector3 barInitialScale = Vector3.one;
    public Transform playerTransform;
    bool tempInvincibility = false;
    [SerializeField] private float invincTimer = 2f;

    private void Awake() {
        inst  = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        barInitialScale = HealthBar.transform.localScale;

    }

    // Update is called once per frame
    void Update()
    {
        HealthBar.transform.localScale = new(barInitialScale.x,barInitialScale.y,barInitialScale.z*PlayerManager.inst.health/100f);
    }

    public Vector3 GetPlayerPos()
    {
        return playerTransform.position;
    }

    public void DealDamage(float damage)
    {
        if(tempInvincibility)
            return;
        health-=damage;
        tempInvincibility=true;
        Invoke(nameof(TurnOffInvincible), invincTimer);
        if(health<=0f)
        {
            
        }
        health = Mathf.Clamp(health,0f,100f);
    }

    public void TurnOffInvincible()
    {
        tempInvincibility=false;
    }
}
