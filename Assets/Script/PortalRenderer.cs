using System.Collections.Generic;
using UnityEngine;

public class PortalRenderer : MonoBehaviour
{
    public float animationDuration = 0.5f;
    public Transform specialObject;      
    public float specialPartialMultiplier = 0.5f; 
    private Vector3 originalScale;
    private Dictionary<Transform, Vector3> childOriginalScales;

    private void Awake()
    {
        originalScale = transform.localScale;
        // recursively store the original local scale for all children.
        childOriginalScales = new Dictionary<Transform, Vector3>();
        StoreChildScales(transform);
    }

    // recursively store the original local scale of each child.
    private void StoreChildScales(Transform parent)
    {
        foreach (Transform child in parent)
        {
            childOriginalScales[child] = child.localScale;
            StoreChildScales(child);
        }
    }

    // animate from scale 0 to 25% of original scale (partial open).
    public void AnimatePortalPartialOpen()
    {
        // animate the parent object.
        transform.localScale = Vector3.zero;
        LeanTween.scale(gameObject, originalScale * 0.25f, animationDuration).setEaseInOutQuad();

        // animate all children.
        foreach (var kvp in childOriginalScales)
        {
            kvp.Key.localScale = Vector3.zero;
            if (specialObject != null && kvp.Key == specialObject)
            {
                LeanTween.scale(kvp.Key.gameObject, kvp.Value * specialPartialMultiplier, animationDuration).setEaseInOutQuad();
            }
            else
            {
                LeanTween.scale(kvp.Key.gameObject, kvp.Value * 0.25f, animationDuration).setEaseInOutQuad();
            }
        }
    }

    // animate from 25% scale to 100% of original scale (final open).
    public void AnimatePortalFinalOpen()
    {
        // animate the parent object.
        LeanTween.scale(gameObject, originalScale, animationDuration*2).setEaseInOutQuad();

        // animate all children.
        foreach (var kvp in childOriginalScales)
        {
            LeanTween.scale(kvp.Key.gameObject, kvp.Value, animationDuration*2).setEaseInOutQuad();
        }
    }

    // animate closing (scale to 0) then disable the portal.
    public void AnimatePortalClose()
    {
        // animate the parent object.
        LeanTween.scale(gameObject, Vector3.zero, animationDuration*3).setEaseInOutQuad().setOnComplete(() =>
        {
            gameObject.SetActive(false);
        });

        // animate all children.
        foreach (var kvp in childOriginalScales)
        {
            LeanTween.scale(kvp.Key.gameObject, Vector3.zero, animationDuration*3).setEaseInOutQuad();
        }
    }
}
