using UnityEngine;

public class BoxSuccessZone : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private FactoryProgressManager progressManager;

    [Header("Detection Settings")]
    [SerializeField]
    private string validBoxTag = "LiftableBox";

    private void OnTriggerEnter(Collider other)
    {
        GameObject detectedBox = FindTaggedBox(other);

        if (detectedBox == null)
        {
            return;
        }

        progressManager.RegisterSuccessfulBox(detectedBox);
    }

    private GameObject FindTaggedBox(Collider detectedCollider)
    {
        // First check the collider object.
        if (detectedCollider.CompareTag(validBoxTag))
        {
            return detectedCollider.gameObject;
        }

        // Then check its Rigidbody root.
        Rigidbody attachedRigidbody = detectedCollider.attachedRigidbody;

        if (attachedRigidbody != null &&
            attachedRigidbody.CompareTag(validBoxTag))
        {
            return attachedRigidbody.gameObject;
        }

        // Finally check the hierarchy root.
        Transform root = detectedCollider.transform.root;

        if (root.CompareTag(validBoxTag))
        {
            return root.gameObject;
        }

        return null;
    }
}