using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using System.Collections;

public enum JetMode
{
    Off,
    Thrust
}

public class InputManager : MonoBehaviour
{
    public static InputManager inst;
    [SerializeField] private GameObject jetEngine;
    [SerializeField] private ParticleSystem smokeEffect;
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource jetStart;
    [SerializeField] private AudioSource jetLoop;
    [SerializeField] private AudioSource jetEnd;
    [SerializeField] private AudioSource jetImpulse;
    [SerializeField] private Rigidbody playerRB;
    [SerializeField] private Transform head;
    [SerializeField] InputActionReference leftControllerClick;
    [SerializeField] InputActionReference modeSwapButton;
    [SerializeField] InputActionReference xStick;
    //[SerializeField] InputActionReference yStick;
    bool modeSwapHoldFlag = false;
    [SerializeField] private int jetPower = 300;
    [SerializeField] private float impulseMultiplier = 2f;
    [SerializeField] private float impulseCooldown = 1f;
    [SerializeField] private float stallCooldown = 2f;
    [SerializeField] private float fuelRegenRate = 30f;
    [SerializeField] private float fuelBurnRate = 20f;
    [SerializeField] private float impulseBurnAmmount = 40f;
    float cooldownTimer = 0f;
    float impulseTimer = 0f;
    JetMode jetMode = JetMode.Thrust;
    [SerializeField] private GameObject fuelBar;
    [SerializeField] private GameObject cooldownBar;
    Vector3 barInitialScale = Vector3.one;

    bool usingJet;

    private void Awake() {
        inst = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        barInitialScale = fuelBar.transform.localScale;
        smokeEffect.enableEmission = false;
        usingJet = false;
    }
    private void FixedUpdate() {
        Vector2 movement = xStick.action.ReadValue<Vector2>();
        Vector3 temp = (movement.x * head.right)+( movement.y * head.forward);
        temp.y=0;
        playerRB.transform.position+=7 * Time.deltaTime * temp.normalized;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerRB.velocity.magnitude * 0.1f > 0.4f) {
            music.volume += 0.1f * Time.deltaTime;
        } else {
            music.volume -= 0.05f * Time.deltaTime;
        }
        music.volume = Mathf.Clamp(music.volume, .1f, 0.5f);

        float fire = leftControllerClick.action.ReadValue<float>();
        float impulseFire = modeSwapButton.action.ReadValue<float>();

        if(PlayerManager.inst.fuel<2f)
        {
            jetMode = JetMode.Off;
            Invoke(nameof(RestartJet), stallCooldown);
            cooldownTimer = stallCooldown;
        }
        switch (jetMode)
        {
            case JetMode.Off:
                PlayerManager.inst.fuel+=fuelRegenRate*.4f*Time.deltaTime;
                smokeEffect.enableEmission = false;
                if (usingJet) {
                        jetStart.Stop();
                        jetLoop.Stop();
                        jetEnd.Play();
                        usingJet = false;
                    }
                // jetEnd.Play();
            break;
            case JetMode.Thrust:
                if(impulseFire>.6f && impulseTimer<=.01f)
                {
                    if (Vector3.Angle(jetEngine.transform.up, Vector3.up) < 20f) {
                        DragUp(jetPower*impulseMultiplier*1.5f*Vector3.up);
                    }
                    playerRB.AddForce(jetPower*impulseMultiplier*jetEngine.transform.up,ForceMode.Impulse);
                    impulseTimer=impulseCooldown;
                    PlayerManager.inst.fuel-=impulseBurnAmmount;

                    smokeEffect.Emit(100);
                    jetImpulse.Play();
                }
                else if(fire>.2f && PlayerManager.inst.fuel>=2f)
                {
                    playerRB.AddForce(fire*jetPower*jetEngine.transform.up*Time.deltaTime,ForceMode.Force);
                    PlayerManager.inst.fuel-=fuelBurnRate*Time.deltaTime;
                    smokeEffect.enableEmission = true;


                    if (!usingJet) {
                        jetStart.Play();
                        jetLoop.PlayScheduled(AudioSettings.dspTime + 3f);
                        usingJet = true;
                    } 
                }
                else
                {
                    PlayerManager.inst.fuel+=fuelRegenRate*Time.deltaTime;    
                    smokeEffect.enableEmission = false;
                    if (usingJet) {
                        jetStart.Stop();
                        jetLoop.Stop();
                        jetEnd.Play();
                        usingJet = false;
                    }
                    
                }        

            break;
            default:
            break;
        }
        fuelBar.transform.localScale = new(barInitialScale.x,barInitialScale.y,barInitialScale.z*PlayerManager.inst.fuel/100f);
        if(cooldownTimer>0f)
            cooldownBar.transform.localScale = new(barInitialScale.x,barInitialScale.y,cooldownTimer/stallCooldown*barInitialScale.z); 
        else if(impulseTimer>0f)
            cooldownBar.transform.localScale = new(barInitialScale.x,barInitialScale.y,impulseTimer/impulseCooldown*barInitialScale.z); 
        
        if(cooldownTimer>0f)
            cooldownTimer-=Time.deltaTime;
        else if(cooldownTimer<0f)
            cooldownTimer=0f;
        if(impulseTimer>0f)
            impulseTimer-=Time.deltaTime;
        else if(impulseTimer<0f)
            impulseTimer=0f;

        PlayerManager.inst.fuel = Mathf.Clamp(PlayerManager.inst.fuel,-10f,100f);
    }
    
    public void RestartJet()
    {
        jetMode = JetMode.Thrust;
    }

    void DragUp(Vector3 vec)
    {
        Collider[] hitColliders = Physics.OverlapSphere(playerRB.position, 10);
        foreach (var hitCollider in hitColliders)
        {
            var gameObject = hitCollider.gameObject;
            if (gameObject.tag == "Enemy") {
                gameObject.GetComponent<EnemyAIController>().ToggleNav();
                gameObject.GetComponent<Rigidbody>().AddForce(vec, ForceMode.Impulse);
            }
        }
    }
}
