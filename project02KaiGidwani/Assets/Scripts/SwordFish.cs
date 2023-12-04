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
    private float wanderAngle = 0f;
    [SerializeField] public float maxWanderAngle = 45f;

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
    [SerializeField] float avoidTime;

    private Vector3 huntingForce;
    private GameObject chosenFood = null;
    [SerializeField] private float huntingScalar;
    [SerializeField] private float huntingRange = 5f;


    [SerializeField] SwordFishState state;

    protected override void Init()
    {
        wanderAngle = Random.Range(-maxWanderAngle, maxWanderAngle);
    }

    protected override void CalcSteeringForces()
    {
        switch (state)
        {
            case SwordFishState.Stalking:
                // Get wander force
                wanderForce = Wander(wanderTime, wanderAngle, maxWanderAngle);
                wanderForce *= wanderScalar;
                ultimaForce += wanderForce;
                return;

            case SwordFishState.Hunting:
                // Seek food in food radius
                huntingForce = Seek(chosenFood);
                huntingForce *= huntingScalar;
                ultimaForce += huntingForce;
                return;
        }

        // Get bounds force
        boundsForce = StayInBounds(boundsTime);
        boundsForce *= boundsScalar;
        ultimaForce += boundsForce;

        // Apply forces to the physics object
        physicsObject.ApplyForce(ultimaForce);
    }

    protected override void StateChangeCheck()
    {
        List<GameObject> nearbyFood = new List<GameObject>();

        // Add every nearby fishfood to the list
        foreach (GameObject food in manager.allFish)
        {
            if (food.GetComponent<Agent>().AgentType != agentType)
            {
                float d = Vector3.Distance(transform.position, food.transform.position);
                if ((d > 0) && (d < huntingRange))
                {
                    nearbyFood.Add(food);
                }
            }
        }


        if (nearbyFood.Count > 0)
        {
            // Change states
            state = SwordFishState.Hunting;

            // If prevoiusly chosen food is gone, choose a new food to seek
            if (chosenFood == null)
            {
                chosenFood = nearbyFood[Random.Range(0, nearbyFood.Count)];
            }
        }
        else
        {
            // Change states
            state = SwordFishState.Stalking;

            // Set chosenFood to null
            chosenFood = null;
        }
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
