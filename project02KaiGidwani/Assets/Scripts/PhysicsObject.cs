using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    [SerializeField] private Vector3 position;
    [SerializeField] private Vector3 direction;
    [SerializeField] private Vector3 velocity;
    [SerializeField] private Boolean boolFriction;
    [SerializeField] private float frictionStrength;
    [SerializeField] private Boolean boolGravity;
    [SerializeField] private Vector3 gravityStrength;

    public Vector3 Velocity { get { return velocity; } }

    public Vector3 Direction { get { return direction; } }

    // Sum of all forces in a frame - New
    [SerializeField] private Vector3 acceleration = Vector3.zero;

    // Mass of object - New
    [SerializeField] private float mass = 1;

    [SerializeField] private float maxSpeed = 10;
    public float MaxSpeed { get { return maxSpeed; } }

    // window bounds
    private Vector3 screenMax = Vector3.zero;

    public Vector3 ScreenMax { get {  return screenMax; } }

    // For collisions later
    [SerializeField] private float radius;
    public float Radius { get { return radius; } }


    // Start is called before the first frame update
    void Start()
    {
        // Set the position to the transform's position
        position = transform.position;

        // Grab the camera's ortographic size
        screenMax.y = Camera.main.orthographicSize;

        // Give starter velocity. Required for object avoidance.
        acceleration += new Vector3(0.1f, 0.1f, 0.0f);

        // Width is not a value found on the Camera component
        // Must calculate the width yourself:
        // Get the camera's height and multiply it by the Camera's aspect ratio:
        screenMax.x = screenMax.y * Camera.main.aspect;
    }

    // Update is called once per frame
    void Update()
    {
        // Apply ALL forces first

        // If using gravity
        if (boolGravity)
        {
            // Apply the gravity
            ApplyGravity(gravityStrength);
        }

        // If using friction
        if (boolFriction)
        {
            // Apply the friction
            ApplyFriction(frictionStrength);
        }

        // Calculate the velocity for this frame - New
        velocity += acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        position += velocity * Time.deltaTime;

        //CheckForBounce();


        // Grab current direction from velocity  - New
        direction = velocity.normalized;

        transform.position = position;

        // Zero out acceleration - New
        acceleration = Vector3.zero;

        // Make the object face the direction it is going
        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
    }

    public void ApplyForce(Vector3 force)
    {
        acceleration += force / mass;
    }

    void ApplyGravity(Vector3 force)
    {
        acceleration += force;
    }

    void ApplyFriction(float coeff)
    {
        Vector3 friction = velocity * -1;
        friction.Normalize();
        friction = friction * coeff;
        ApplyForce(friction);
    }

    void CheckForBounce()
    {
        // Check the positions against the boundaries

        if (position.x > screenMax.x) // Right
        {
            velocity.x *= -1;
            position.x = screenMax.x;
        }
        else if (position.x < -screenMax.x) // Left
        {
            velocity.x *= -1;
            position.x = -screenMax.x;
        }

        if (position.y > screenMax.y) // Top
        {
            velocity.y *= -1;
            position.y = screenMax.y;
        }
        else if (position.y < -screenMax.y) // Bottom
        {
            velocity.y *= -1;
            position.y = -screenMax.y;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.DrawLine(transform.position, transform.position + Velocity);
    }
}
