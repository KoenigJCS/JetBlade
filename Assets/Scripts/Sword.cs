using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] private float baseDamage = 18f;
    [SerializeField] private float baseForce = 5f;
    [SerializeField] private AudioSource swordSwing;
    Vector3 lastPos;
    Vector3 currPos;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        lastPos = currPos;
        currPos = gameObject.transform.position;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            Vector3 direction = (currPos - lastPos).normalized;
            float magnitude = Mathf.Abs((currPos - lastPos).magnitude / Time.deltaTime);
            other.gameObject.GetComponent<EnemyAIController>().ToggleNav();
            other.gameObject.GetComponent<Rigidbody>().AddForce(baseForce * magnitude * direction, ForceMode.Impulse);
            other.gameObject.GetComponent<EnemyAIController>().TakeDamage(Mathf.RoundToInt(baseDamage * magnitude));
            swordSwing.Play();
        }
        if (other.gameObject.CompareTag("Rocket"))
        {
            swordSwing.Play();
        }
    }
    public void PlaySwingSound() {
        Debug.Log("here");
        swordSwing.Play();
    }
}
