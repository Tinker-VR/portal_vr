using UnityEngine;

public class CrossSectionTrigger : MonoBehaviour
{
    public Material crossSectionMaterial;
    private bool isInCrossSection = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CrossSectionObject"))
        {
            crossSectionMaterial = other.GetComponent<MeshRenderer>().material;
            isInCrossSection = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CrossSectionObject"))
        {
            if (crossSectionMaterial == null) return;
            isInCrossSection = false;
            crossSectionMaterial.SetVector("_SectionPlane", Vector4.zero);
            crossSectionMaterial = null;
        }
    }

    private void Update()
    {
        if (isInCrossSection)
        {
            if (crossSectionMaterial == null) return;
            var normal = transform.forward;
            var point = transform.position;
            var d = -Vector3.Dot(normal, point);
            var plane = new Vector4(normal.x, normal.y, normal.z, d);

            crossSectionMaterial.SetVector("_SectionPlane", plane);
        }
    }
}