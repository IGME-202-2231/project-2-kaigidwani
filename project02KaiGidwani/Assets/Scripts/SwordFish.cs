using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;

public enum SwordFishState
{
    Stalking,
    Hunting
}

public class SwordFish : Agent
{
    private Vector3 wanderForce;
    [SerializeField] private float wanderTime;
    [SerializeField] private float wanderScalar;

    private Vector3 boundsForce;
    [SerializeField] private float boundsTime;
    [SerializeField] private float boundsScalar;

    private Vector3 separateForce;
    [SerializeField] private float separateScalar;

    private Vector3 cohesionForce;
    [SerializeField] private float cohesionScalar;

    private Vector3 alignForce;
    [SerializeField] private float alignScalar;

    [SerializeField] private float avoidScalar;

    private float wanderAngle = 0f;
    [SerializeField] public float maxWanderAngle = 45f;

    [SerializeField] float avoidTime;


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

        // Get cohesion force
        cohesionForce = Cohesion(this.GetComponent<Agent>().Manager.allFish);
        cohesionForce *= cohesionScalar;
        ultimaForce += cohesionForce;

        // Get align force
        alignForce = Align(this.GetComponent<Agent>().Manager.allFish);
        alignForce *= alignScalar;
        ultimaForce += alignForce;

        // Get avoiding obstacles force
        ultimaForce += AvoidObstacles(avoidTime) * avoidScalar;

        // Apply forces to the physics object
        physicsObject.ApplyForce(ultimaForce);
    }

    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, transform.position + ultimaForce);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + wanderForce);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position + boundsForce);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + cohesionForce);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + alignForce);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + separateForce);
        Gizmos.DrawWireSphere(transform.position, this.GetComponent<Agent>().SeparateRange);


        //
        //  Draw safe space box
        //
        Vector3 futurePos = CalcFuturePosition(avoidTime);

        float dist = Vector3.Distance(transform.position, futurePos) + physicsObject.Radius;

        Vector3 boxSize = new Vector3(physicsObject.Radius * 2f,
            dist
            , physicsObject.Radius * 2f);

        Vector3 boxCenter = Vector3.zero;
        boxCenter.y += dist / 2f;

        Gizmos.color = Color.green;

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(boxCenter, boxSize);
        Gizmos.matrix = Matrix4x4.identity;


        //
        //  Draw lines to found obstacles
        //
        Gizmos.color = Color.yellow;


        foreach (Vector3 pos in foundObstacles)
        {
            Gizmos.DrawLine(transform.position, pos);
        }
    } 
}
