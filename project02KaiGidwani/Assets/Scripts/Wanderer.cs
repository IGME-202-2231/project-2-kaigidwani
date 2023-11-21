using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Wanderer : Agent
{
    private Vector3 wanderForce;
    [SerializeField] private float wanderTime;
    [SerializeField, Min(1f)] private float wanderScalar;
    private Vector3 boundsForce;
    [SerializeField] private float boundsTime;
    [SerializeField, Min(1f)] private float boundsScalar;

    private float wanderAngle = 0f;
    [SerializeField] public float maxWanderAngle = 45f;

    protected override void Init()
    {
        wanderAngle = Random.Range(-maxWanderAngle, maxWanderAngle);
    }

    protected override void CalcSteeringForces()
    {
        // Get wander force
        wanderForce = Wander(wanderTime, wanderAngle, maxWanderAngle);
        wanderForce *= wanderScalar;
        ultimaForce += wanderForce;


        // Get bounds force
        boundsForce = StayInBounds(boundsTime);
        boundsForce *= boundsScalar;
        ultimaForce += boundsForce;

        // Apply forces to the physics object
        physicsObject.ApplyForce(ultimaForce);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, transform.position + wanderForce);
    }
}
