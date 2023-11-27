using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;

public class Wanderer : Agent
{
    private Vector3 wanderForce;
    [SerializeField] private float wanderTime;
    [SerializeField, Min(1f)] private float wanderScalar;

    private Vector3 boundsForce;
    [SerializeField] private float boundsTime;
    [SerializeField, Min(1f)] private float boundsScalar;

    private Vector3 separateForce;
    [SerializeField] private float separateScalar;

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

        // Get separate force
        separateForce = Separate();
        separateForce *= separateScalar;
        ultimaForce += separateForce;

        // Apply forces to the physics object
        physicsObject.ApplyForce(ultimaForce);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, transform.position + ultimaForce);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + wanderForce);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position + boundsForce);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + separateForce);
        Gizmos.DrawWireSphere(transform.position, this.GetComponent<Agent>().SeparateRange);
    }
}
