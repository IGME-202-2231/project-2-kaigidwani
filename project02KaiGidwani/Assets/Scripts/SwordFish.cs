using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

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

    [SerializeField] float stalkingTimer;
    [SerializeField] float huntingTimer;

    protected override void Init()
    {
        wanderAngle = Random.Range(-maxWanderAngle, maxWanderAngle);
        stalkingTimer = Random.Range(4.5f, 6.5f);
        huntingTimer = 1;
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
                
                // Get cohesion force
                cohesionForce = Cohesion(this.GetComponent<Agent>().Manager.allFish);
                cohesionForce *= cohesionScalar;
                ultimaForce += cohesionForce;

                // Get align force
                alignForce = Align(this.GetComponent<Agent>().Manager.allFish);
                alignForce *= alignScalar;
                ultimaForce += alignForce;
            break;

            case SwordFishState.Hunting:
                if (chosenFood != null)
                {
                    // Get hunting force
                    huntingForce = Seek(chosenFood);
                    huntingForce *= huntingScalar;
                    ultimaForce += huntingForce;
                }
            break;
        }

        // Get separate force
        separateForce = Separate();
        separateForce *= separateScalar;
        ultimaForce += separateForce;

        // Get bounds force
        boundsForce = StayInBounds(boundsTime);
        boundsForce *= boundsScalar;
        ultimaForce += boundsForce;

        // Apply forces to the physics object
        physicsObject.ApplyForce(ultimaForce);
    }

    protected override void StateChangeCheck()
    {
        if (state == SwordFishState.Stalking)
        {
            // Reduce timer
            stalkingTimer = stalkingTimer - Time.deltaTime;
        }
        else
        {
            // Reduce other timer
            huntingTimer = huntingTimer - Time.deltaTime;
        }

        // If stalking timer has run out, switch to hunting
        if (stalkingTimer <= 0 && state == SwordFishState.Stalking)
        {
            // Switch to hunting
            state = SwordFishState.Hunting;

            // Set new hunting timer
            huntingTimer = Random.Range(1.5f, 2);
        }
        // If hunting timer has run out, switch to stalking
        else if (huntingTimer <= 0 && state == SwordFishState.Hunting)
        {
            // Switch to stalking
            state = SwordFishState.Stalking;

            // Set new stalking timer
            stalkingTimer = Random.Range(4.5f, 6.5f);

            // Set chosenFood to null
            chosenFood = null;
        }

        // Do hunting!
        if (state == SwordFishState.Hunting)
        {
            // Make a list for all nearby food
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
                // === Check collisions ===

                // List of food to remove
                List<GameObject> foodToRemove = new List<GameObject>();

                // Loop through each nearby food object to see if we are colliding with it
                foreach (GameObject food in nearbyFood)
                {
                    // Check if we are colliding with it
                    if (manager.circleCheck(this.GetComponent<PhysicsObject>(), food.GetComponent<PhysicsObject>()))
                    {
                        // If so, add it to the list to remove it
                        foodToRemove.Add(food);
                    }
                }

                // Remove the fish food
                foreach (GameObject food in foodToRemove)
                {
                    // Remove the reference to the food
                    manager.AllFish.Remove(food);
                    nearbyFood.Remove(food);

                    // Get rid of the food from the scene
                    Destroy(food);
                }

                // Switch back to stalking if we have caught a fish
                if (foodToRemove.Count > 0)
                {
                    // Switch to stalking
                    state = SwordFishState.Stalking;

                    // Set new stalking timer
                    stalkingTimer = Random.Range(4.5f, 6.5f);

                    // Set chosenFood to null
                    chosenFood = null;
                }

                // === Set new food ===

                // If prevoiusly chosen food is gone, choose a new food to seek
                if (chosenFood == null)
                {
                    chosenFood = nearbyFood[Random.Range(0, nearbyFood.Count)];
                }
            }
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
