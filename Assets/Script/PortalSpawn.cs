using UnityEngine;
using UnityEngine.Events;
using Oculus.Interaction;

public class PortalSpawn : MonoBehaviour
{
    public OVRHand leftHand;
    public OVRHand rightHand;
    public GameObject leftObject;  
    public GameObject rightObject;
    public PortalManager portalManager;
    public Transform playerCamera; // used for orientation so portals face the user

    public float grabThreshold = 0.7f; 
    public float centerOffset = 0.1f;

    // how far the finishing hand must be from the camera to map from partial to max distance:
    public float minHandDistance = 0.3f; // finishing hand very close
    public float maxHandDistance = 1.0f; // finishing hand fully extended (arm's length) 
    // the portal's distance from the camera is LERPed between these values:
    public float minPortalDistance = 0.5f; // near the original partial spawn
    public float maxPortalDistance = 3.0f; // far away from the user

    [Header("Cooldown Settings")]
    public float portalCooldownDuration = 2.0f; // Seconds to wait after full spawn before new spawn

    private bool portalCooldownActive = false;
    private float timeSinceLastPortal = 0.0f;

    private enum PortalState { Idle, Initiated }
    private PortalState currentState = PortalState.Idle;

    private OVRHand initiatingHand = null;
    private Transform activePortal = null;
    
    // store the partial spawn position so we can later update the portal's distance
    private Vector3 partialSpawnPos;

    public void OnLeftCustomPoseDetected()
    {
        if (portalCooldownActive) return;  // Don't initiate if under cooldown
        if (currentState == PortalState.Idle)
        {
            currentState = PortalState.Initiated;
            initiatingHand = leftHand;
            partialSpawnPos = ComputePartialSpawnPos(leftHand);
            activePortal = portalManager.CreatePortal(partialSpawnPos, GetSpawnRotation(), false);
        }
        else if (currentState == PortalState.Initiated && initiatingHand != leftHand)
        {
            activePortal.GetComponent<PortalRenderer>().AnimatePortalFinalOpen();
            BeginCooldownAndReset();
        }
    }

    public void OnRightCustomPoseDetected()
    {
        if (portalCooldownActive) return;  // Don't initiate if under cooldown
        if (currentState == PortalState.Idle)
        {
            currentState = PortalState.Initiated;
            initiatingHand = rightHand;
            partialSpawnPos = ComputePartialSpawnPos(rightHand);
            activePortal = portalManager.CreatePortal(partialSpawnPos, GetSpawnRotation(), false);
        }
        else if (currentState == PortalState.Initiated && initiatingHand != rightHand)
        {
            activePortal.GetComponent<PortalRenderer>().AnimatePortalFinalOpen();
            BeginCooldownAndReset();
        }
    }

    public void OnInitialPoseStopped()
    {
        if (currentState == PortalState.Initiated)
        {
            if (activePortal != null)
            {
                activePortal.GetComponent<PortalRenderer>().AnimatePortalClose();
            }
            ResetToIdle();
        }
    }

    void Update()
    {
        if (leftHand == null || rightHand == null || portalManager == null || playerCamera == null)
        {
            Debug.LogError("References to hands, portal manager, or camera are not set properly.");
            return;
        }

        ManageObjectVisibility();

        // cooldown
        if (portalCooldownActive)
        {
            timeSinceLastPortal += Time.deltaTime;
            if (timeSinceLastPortal >= portalCooldownDuration)
            {
                portalCooldownActive = false;
                timeSinceLastPortal = 0.0f;
            }
            return;
        }

        // in Initiated state, update portal's position so it follows the initiating hand while letting finishing hand control distance
        if (currentState == PortalState.Initiated)
        {
            // update partial spawn position to follow initiating hand
            partialSpawnPos = ComputePartialSpawnPos(initiatingHand);
            // determine finishing hand (the one not initiating).
            OVRHand finishingHand = (initiatingHand == leftHand) ? rightHand : leftHand;
            // dynamically update the portal's position
            Vector3 newPos = ComputeDynamicDistance(finishingHand);
            if (activePortal != null)
            {
                activePortal.position = newPos;
                activePortal.rotation = GetSpawnRotation();
            }
        }
    }

   //partially spawn near the initiating hand plus offset
    private Vector3 ComputePartialSpawnPos(OVRHand initiator)
    {
        Vector3 basePos = initiator.transform.position + playerCamera.forward * 0.3f; // ~0.3m forward
        if (initiator == leftHand)
            basePos += playerCamera.right * centerOffset;
        else
            basePos -= playerCamera.right * centerOffset;
        return basePos;
    }

    // finishing hand extends or retracts to push/pull the portal.
    private Vector3 ComputeDynamicDistance(OVRHand finishingHand)
    {
        Vector3 toPartial = partialSpawnPos - playerCamera.position;
        float partialDist = toPartial.magnitude;

        float finishingForward = Vector3.Dot(finishingHand.transform.position - playerCamera.position, playerCamera.forward);
        float clampedForward = Mathf.Clamp(finishingForward, minHandDistance, maxHandDistance);
        float t = Mathf.InverseLerp(minHandDistance, maxHandDistance, clampedForward);

        float finalDist = Mathf.Lerp(partialDist, maxPortalDistance, t);
        Vector3 direction = toPartial.normalized;
        return playerCamera.position + direction * finalDist;
    }

    // keep the portal orientation to face the camera.
    private Quaternion GetSpawnRotation()
    {
        return playerCamera.rotation;
    }

    // ---------------------------------------------------------------------------
    // STATE MACHINE HELPERS
    // ---------------------------------------------------------------------------
    private void BeginCooldownAndReset()
    {
        portalCooldownActive = true;
        currentState = PortalState.Idle;
        initiatingHand = null;
        activePortal = null;
    }

    private void ResetToIdle()
    {
        currentState = PortalState.Idle;
        initiatingHand = null;
        activePortal = null;
    }

    private void ManageObjectVisibility()
    {
        if (leftHand != null)
            leftObject?.SetActive(true);
        if (rightHand != null)
            rightObject?.SetActive(true);
    }
}
