using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform otherPortal;
    public Camera portalCamera;
    public Transform playerCamera;
    public Material portalMaterial;

    private void Start()
    {
        portalMaterial.mainTexture = portalCamera.targetTexture;
    }
    private void LateUpdate()
    {
        var playerOffsetFromPortal = playerCamera.position - otherPortal.position;

        portalCamera.transform.position = transform.position + playerOffsetFromPortal;

        var angularDifferenceBetweenPortalRotations = Quaternion.Angle(transform.rotation, otherPortal.rotation);
        var portalRotationDifference = Quaternion.AngleAxis(angularDifferenceBetweenPortalRotations, Vector3.up);

        var newCameraDirection = portalRotationDifference * playerCamera.forward;
        portalCamera.transform.rotation = Quaternion.LookRotation(newCameraDirection, Vector3.up);
    }
}