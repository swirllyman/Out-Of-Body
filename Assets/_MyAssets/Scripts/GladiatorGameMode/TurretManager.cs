using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretManager : MonoBehaviour
{
    [SerializeField] Turret[] turrets;
    [SerializeField] float minTurretDistance = 5.0f;

    PlayerDissociation player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerDissociation>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Turret t in turrets)
        {
            t.followPlayer = Vector3.Distance(t.transform.position, player.transform.position) < minTurretDistance;


            if(t.followPlayer && t.currentCD <= 0.0f &! player.invincible &! player.dissociated)
            {
                t.ShootAtPlayer();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        foreach(Turret t in turrets)
        {
            Gizmos.DrawWireSphere(t.transform.position, minTurretDistance);
        }
    }
}

[System.Serializable]
public struct TimedTurrets
{
    public Turret turret;
    public float turretCD;
}