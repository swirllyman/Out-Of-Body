using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider), typeof(TrailRenderer))]
public class Projectile : MonoBehaviour
{
    [SerializeField, Tooltip("Renderer of projectile to flash out on destroy")] Renderer myRend;
    [SerializeField, Tooltip("Amount of Force applied to projectile on start")] float shotPower = 50.0f;
    [SerializeField, Tooltip("Bonus force applied to Player when being hit by projectile")] float explosionForceModifier = 15.0f;
    
    bool beenUsed = false;

    Rigidbody myBody;
    TrailRenderer myTrail;
    Coroutine killRoutine;

    private void Awake()
    {
        myBody = GetComponent<Rigidbody>();
        myTrail = GetComponent<TrailRenderer>();
    }

    private void Start()
    {
        myBody.AddForce(transform.forward * shotPower, ForceMode.Impulse);
        killRoutine = StartCoroutine(KillProjectile());
    }

    void OnCollisionEnter(Collision collision)
    {
        //Since we aren't destroying immediately, beenUsed flag is set to prevent getting hit multiple times by the same projectile
        if (!beenUsed && collision.collider.CompareTag("Player"))
        {
            PlayerDissociation hitPlayer = collision.collider.GetComponent<PlayerDissociation>();
            if(hitPlayer != null &! hitPlayer.dissociated &! hitPlayer.invincible)
            {
                hitPlayer.PlayerHit(collision.contacts[0].point, -myBody.velocity * explosionForceModifier);
                myBody.useGravity = true;
                beenUsed = true;
                
                if(killRoutine != null) StopCoroutine(killRoutine);
                killRoutine = StartCoroutine(KillProjectile(2.0f));
            }
        }
    }

    IEnumerator KillProjectile(float waitTime = 4.0f)
    {
        yield return new WaitForSeconds(waitTime);
        myTrail.time = 0.0f;
        for (int i = 0; i < 5; i++)
        {
            myRend.enabled = !myRend.enabled;
            yield return new WaitForSeconds(.1f);
        }
        myRend.enabled = false;


        //TODO: Setup Pooling
        Destroy(gameObject);
    }
}
