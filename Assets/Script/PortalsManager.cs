using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public Transform[] portals = new Transform[2];

    // accepts a bool to determine whether to play the final (full) animation or partial.
    // it returns the activated portal.
    public Transform CreatePortal(Vector3 position, Quaternion rotation, bool isFinal)
    {
        Transform portalToActivate = null;

        // look for an inactive portal.
        foreach (var portal in portals)
        {
            if (!portal.gameObject.activeInHierarchy)
            {
                portalToActivate = portal;
                break;
            }
        }

        if (portalToActivate == null)
        {
            // if both portals are active, reuse the oldest one.
            portals[0].position = position;
            portals[0].rotation = rotation;
            portalToActivate = portals[0];

            // cycle portals so that the reused one becomes the newest.
            portals[0] = portals[1];
            portals[1] = portalToActivate;

            if (isFinal)
                portals[1].GetComponent<PortalRenderer>().AnimatePortalFinalOpen();
            else
                portals[1].GetComponent<PortalRenderer>().AnimatePortalPartialOpen();
        }
        else
        {
            // activate and position the selected portal.
            portalToActivate.position = position;
            portalToActivate.rotation = rotation;
            portalToActivate.gameObject.SetActive(true);

            if (isFinal)
                portalToActivate.GetComponent<PortalRenderer>().AnimatePortalFinalOpen();
            else
                portalToActivate.GetComponent<PortalRenderer>().AnimatePortalPartialOpen();
        }

        // setup portal links if both are active.
        if (portals[0].gameObject.activeInHierarchy && portals[1].gameObject.activeInHierarchy)
        {
            SetupPortalPair();
        }

        return portalToActivate;
    }

    private void SetupPortalPair()
    {

    }
}
