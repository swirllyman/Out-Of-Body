using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;

[RequireComponent(typeof(PlayerLocomotion))]
public class PlayerDissociation : MonoBehaviour
{
    public delegate void OnPlayerHit();
    public event OnPlayerHit onPlayerHitCallback;

    public bool invincible = false;
    public bool dissociated { private set; get; }

    [Header("Child Body References")]
    [SerializeField, Tooltip("Rigidbody of the Child Object we wish to dissociate. This object MUST have a collider attached!")] Rigidbody childRigidBody;

    [Header("General References")]
    [SerializeField, Tooltip("Post Process Volume for changing the contrast value. Should be attached to Camera")] Volume postVolume;
    [SerializeField, Tooltip("Notification Text, should be parented under the Player Camera")] TMP_Text notificationText;

    [Header("Player Specific Settings")]
    [SerializeField] internal int reviveTimeInSeconds = 5;

    Pose startPose;
    Collider childCollider;
    PlayerLocomotion locomotion;
    ColorAdjustments colorAdjustment;
    Coroutine reviveRoutine;


    private void Awake()
    {
        locomotion = GetComponent<PlayerLocomotion>();
        childCollider = childRigidBody.GetComponent<Collider>();

        postVolume.profile.TryGet(out colorAdjustment);

        notificationText.enabled = false;
        colorAdjustment.saturation.value = 0;

        startPose.position = childCollider.transform.localPosition;
        startPose.rotation = childCollider.transform.localRotation;

        childRigidBody.isKinematic = true;
        childCollider.enabled = false;
    }

    private void LateUpdate()
    {
        if(!dissociated)
            childRigidBody.transform.rotation = locomotion.head.rotation;
    }

    /// <summary>
    /// Called from Projectile when contact is made with Player
    /// </summary>
    internal void PlayerHit(Vector3 contactPoint, Vector3 force)
    {
        Dissociate();
        childRigidBody.AddForceAtPosition(force, contactPoint);
    }

    [ContextMenu("Dissociate")]
    //Enable and un-parent child rigidbody and collider, then add force to it.
    void Dissociate()
    {
        colorAdjustment.saturation.value = -100f;
        childRigidBody.isKinematic = false;
        childCollider.enabled = true;
        childCollider.transform.parent = null;
        dissociated = true;

        if (reviveRoutine != null) StopCoroutine(reviveRoutine);
        reviveRoutine = StartCoroutine(ReviveAfterTime());

        onPlayerHitCallback?.Invoke();
    }

    [ContextMenu("Associate")]
    //Disable and re-parent child rigidbody and collider, then reset to start position.
    void Associate()
    {
        colorAdjustment.saturation.value = 0;
        childRigidBody.isKinematic = true;
        childCollider.enabled = false;
        childCollider.transform.parent = transform;

        childCollider.transform.localRotation = startPose.rotation;
        childCollider.transform.localPosition = startPose.position;

        dissociated = false;
    }

    IEnumerator ReviveAfterTime()
    {
        notificationText.enabled = true;
        for (int i = reviveTimeInSeconds; i > 0; i--)
        {
            notificationText.text = "Reviving in..\n" + i.ToString("F0");
            yield return new WaitForSeconds(1.0f);
        }
        notificationText.enabled = false;
        Associate();
    }
}
