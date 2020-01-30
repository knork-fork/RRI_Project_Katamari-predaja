using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleTest : MonoBehaviour
{
    public bool collected;

    public float volume;
    public float density = 1;
    public float mass;

    private Rigidbody rb;
    private BoxCollider uncollectedCollider;    // "normal" collider
    private SphereCollider collectedCollider;   // "small" collider - when object is picked up

    void Start()
    {
        collected = false;

        rb = GetComponent<Rigidbody>();
        uncollectedCollider = GetComponent<BoxCollider>();
        collectedCollider = GetComponent<SphereCollider>();

        Vector3 size = transform.lossyScale;
        volume = size.x * size.y * size.z;
        rb.mass = mass = volume * density;
    }

    public bool IsIrregular(float radius)
    {
        float magnitude = transform.lossyScale.magnitude;
        return radius < magnitude;
    }

    public void AddRigidbody()
    {
        rb = this.gameObject.AddComponent<Rigidbody>();
        rb.mass = mass;
        rb.isKinematic = true;
    }

    public void RemoveRigidbody()
    {
        Destroy(rb);
    }

    public void SetRegular()
    {
        uncollectedCollider.enabled = false;
        collectedCollider.enabled = false;
    }

    public void SetIrregular()
    {
        uncollectedCollider.enabled = false;
        collectedCollider.enabled = true;
    }
}
