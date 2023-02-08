using UnityEngine;

public class Turret : MonoBehaviour
{
    public bool followPlayer;
    public float currentCD = 1.0f;

    [Header("Options")]
    [SerializeField] bool manualInput = true;
    [SerializeField] KeyCode shootingKeyCode = KeyCode.LeftControl;

    [Header("References")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField, Tooltip("Muzzle Transform for getting position/rotation of where to Instantiate projectile")] Transform muzzle;
    [SerializeField, Tooltip("This Transform looks at the player")] Transform neckTransform;

    PlayerDissociation player;

    float totalTimeCD;

    private void Awake()
    {
        player = FindObjectOfType<PlayerDissociation>();
        totalTimeCD = currentCD;
    }

    private void Update()
    {
        if (manualInput && Input.GetKeyDown(shootingKeyCode) && CanShoot() &! player.dissociated)
        {
            ShootAtPlayer();
        }

        if(currentCD > 0)
        {
            currentCD -= Time.deltaTime;
        }
    }

    public bool CanShoot()
    {
        return currentCD <= 0.0f;
    }

    private void LateUpdate()
    {
        if (followPlayer && !player.dissociated)
        {
            Vector3 lookPos = player.transform.position;
            lookPos.y += 2;
            neckTransform.LookAt(new Vector3(lookPos.x, neckTransform.position.y, lookPos.z));
            muzzle.LookAt(lookPos);
        }
    }

    internal void ShootAtPlayer()
    {
        Instantiate(projectilePrefab, muzzle.position, muzzle.rotation, transform);
        currentCD = totalTimeCD;
    }
}

