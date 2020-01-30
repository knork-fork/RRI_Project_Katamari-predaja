using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallStick : MonoBehaviour
{
    // Precalculated value of 1/3 for convenience
    private const float ONE_THIRD = 1.0f / 3.0f;

    [Header("Properties")]
    public float volume;
    public float density = 1;
    public float mass;
    public float CLIMB_RATIO = 2f;

    [Header("Objects")]
    public EventHandler eventHandler;
    public Transform center;
    public Player player;
    public GameObject lastPickedUpRep;

    private Rigidbody rb;
    private SphereCollider sphere;

    // Velocity before collision with pickupable
    private Vector3 savedVelocity;

    // Childed pickup objects
    private List<Collectible> collectibles = new List<Collectible>();
    private List<Collectible> irregularCollectibles = new List<Collectible>();
    private List<Transform> climbables = new List<Transform>();

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sphere = GetComponent<SphereCollider>();

        Vector3 size = transform.lossyScale;
        volume = Mathf.PI * 4 * Mathf.Pow(sphere.radius, 3) / 3; 
        rb.mass = mass = volume * density;
    }

    void FixedUpdate()
    {
        // Should this be before applying upwards force?
        // To make it 100% positive, move this to public function and call from player script
        // Save velocity before collision with pickupable
        savedVelocity = rb.velocity;
    }

    void OnCollisionEnter(Collision col)
    {
        // Collisions with objects on layer "pickups"
        if (col.gameObject.layer == 9)
        {
            bool collected = Collide(col.gameObject);

            if (!collected)
            {
                // Object too large, climb instead

                Collider hit = col.rigidbody.GetComponent<Collider>();
                float targetTop = hit.bounds.extents.y + col.transform.position.y;
                float sphereBottom = transform.position.y - sphere.radius;

                // Climb if taller than CLIMB_RATIO * radius above initial contact
                if (targetTop > sphereBottom && sphereBottom + CLIMB_RATIO * sphere.radius > targetTop)
                {
                    // Normally this should be checked with climbables.Contains, but it doesn't matter
                    player.climbables++;
                    climbables.Add(col.transform);
                }
            }
            else
            {
                // Regain pre-collision speed
                rb.velocity = savedVelocity;
                
                // Change mesh of representation of last picked up object
                UpdateLastPickedUp(col.gameObject);
            }
        }
    }

    private void UpdateLastPickedUp(GameObject pickup)
    {
        // change mesh of representation
        lastPickedUpRep.GetComponent<MeshFilter>().mesh = pickup.GetComponent<MeshFilter>().mesh;
        
        // change material
        lastPickedUpRep.GetComponent<MeshRenderer>().material = pickup.GetComponent<MeshRenderer>().material;
    }

    void OnCollisionExit(Collision col)
    {
        // Remove from climbables list
        if (climbables.Contains(col.transform))
        {
            player.climbables--;
            climbables.Remove(col.transform);
        }
    }

    private bool Collide(GameObject obj)
    {
        // Was anything collected?
        bool collected = false;

        Collectible collectible = obj.GetComponent<Collectible>();
        if (collectible)
        {
            // Check if collectible smaller than third of total mass
            if (collectible.mass < mass * ONE_THIRD)
            {
                if (!collectible.collected)
                {
                    // Collect
                    collectible.collected = true;
                    collected = true;

                    // Move to player layer
                    obj.layer = 8;

                    // Remove physics
                    collectible.RemoveRigidbody();

                    // Child to ball
                    obj.transform.SetParent(this.transform);

                    // Update properties
                    volume += collectible.volume;
                    mass += collectible.mass;
                    rb.mass = mass;

                    // Recalculare sphere collider of the ball
                    RecalculateRadius();

                    // Move camera
                    player.relocateCamera = true;

                    // Call events
                    eventHandler.UpdateVolume(volume);
                    eventHandler.UpdateMass(mass);
                    eventHandler.UpdateRadius(sphere.radius);

                    collectibles.Add(collectible);

                    // Reposition collectible
                    Vector3 delta = (collectible.transform.position - transform.position);
                    float distance = delta.magnitude - sphere.radius;
                    Vector3 direction = delta.normalized;
                    collectible.transform.position = collectible.transform.position - direction * distance;

                    if (collectible.IsIrregular(sphere.radius))
                    {
                        // The object is "sticking out"
                        collectible.SetIrregular();
                        irregularCollectibles.Add(collectible);
                    }
                    else 
                    {
                        // Regularly attached object
                        collectible.SetRegular();
                    }
                }
            }
            else
            {
                // Remove objects from ball (if fast enough)
            }
        }


        return collected;
    }

    private void RecalculateRadius()
    {
        // Set radius of the collider
        sphere.radius = Mathf.Pow((3 * volume) / (4 * Mathf.PI), ONE_THIRD);

        int irNum = irregularCollectibles.Count;
        for (int i = irNum - 1; i >= 0; i--)
        {
            Collectible col = irregularCollectibles[i];
            if (!col.IsIrregular(sphere.radius))
            {
                // The object is not "sticking out" anymore, remove its collider and other properties
                col.SetRegular();
                irregularCollectibles.RemoveAt(i);
            }
        }
    }

    public float GetRadius()
    {
        return sphere.radius;
    }
}
