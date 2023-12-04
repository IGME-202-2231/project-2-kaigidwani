using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Agent : MonoBehaviour
{
    [SerializeField] protected PhysicsObject physicsObject;

    [SerializeField] float maxSpeed;
    [SerializeField] float maxForce;

    [SerializeField] public float maxWanderChangePerSecond = 10f;

    [SerializeField] private float wanderRadius = 1f;

    [SerializeField] private float wanderRange = 1f;

    [SerializeField] private float separateRange = 1f;

    [SerializeField] private float cohesionRange = 5f;

    [SerializeField] private float alignRange = 5f;

    public float SeparateRange { get { return separateRange; } }

    protected List<Vector3> foundObstacles = new List<Vector3>();

    protected Vector3 ultimaForce;

    [SerializeField] protected string agentType;
    public string AgentType { get { return agentType; } }

    [SerializeField] protected SceneManager manager;
    public SceneManager Manager { get { return manager; } set { manager = value; } }


    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Update()
    {
        ultimaForce = Vector3.zero;

        CalcSteeringForces();

        StateChangeCheck();

        Vector3.ClampMagnitude(ultimaForce, maxForce);
        physicsObject.ApplyForce(ultimaForce);
    }

    protected abstract void Init();

    protected abstract void CalcSteeringForces();

    protected abstract void StateChangeCheck();

    protected Vector3 Seek(Vector3 targetPos)
    {
        // Calculate desired velocity
        Vector3 desiredVelocity = targetPos - gameObject.transform.position;

        // Set desired = max speed
        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        // Calculate seek steering force
        Vector3 seekingForce = desiredVelocity - physicsObject.Velocity;

        // Return seek steering force
        return seekingForce;
    }

    protected Vector3 Seek(GameObject target)
    {
        // Call the other version of Seek 
        //   which returns the seeking steering force
        //  and then return that returned vector. 
        return Seek(target.transform.position);
    }


    protected Vector3 Flee(Vector3 targetPos)
    {
        // Calculate desired velocity
        Vector3 desiredVelocity = gameObject.transform.position - targetPos;

        // Set desired = max speed
        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        // Calculate seek steering force
        Vector3 seekingForce = desiredVelocity - physicsObject.Velocity;

        // Return seek steering force
        return seekingForce;
    }

    protected Vector3 Flee(GameObject target)
    {
        // Call the other version of Seek 
        //   which returns the seeking steering force
        //  and then return that returned vector. 
        return Flee(target.transform.position);
    }

    public Vector3 CalcFuturePosition(float time = 1f)
    {
        return physicsObject.Velocity * time + transform.position;
    }

    protected Vector3 Wander(float time, float wanderAngle, float maxWanderAngle)
    {
        // Choose a distance ahead
        Vector3 futurePos = CalcFuturePosition(time);

        // “Project a circle” into that space
        // What radius works best? Do radii have an effect on agent’s movement?
        // Get a random angle to determine displacement vector

        //float maxWanderChange = maxWanderChangePerSecond * time;
        wanderAngle += UnityEngine.Random.Range(-wanderRange, wanderRange);

        // If it goes above or below the maximum, bring it back within the range
        
        
        if (wanderAngle > maxWanderAngle)
        {
            wanderAngle = maxWanderAngle;
        }
        else if (wanderAngle < -maxWanderAngle)
        {
            wanderAngle = -maxWanderAngle;
        }
        

        // Where would that displacement vector end?  Go there.
        Vector3 targetPos = futurePos;
        //targetPos.x / y += Mathf.Sin / Cos(randAngle) * radius
        targetPos.x += Mathf.Cos(wanderAngle * Mathf.Deg2Rad) * wanderRadius;
        targetPos.y += Mathf.Sin(wanderAngle * Mathf.Deg2Rad) * wanderRadius;

        // Need to return a force - how do I get that?
        return Seek(targetPos);
    }

    protected Vector3 StayInBounds(float time)
    {
        Vector3 futurePosition = CalcFuturePosition(time);

        if (futurePosition.x + physicsObject.Radius > physicsObject.ScreenMax.x ||
            futurePosition.x - physicsObject.Radius < -physicsObject.ScreenMax.x ||
            futurePosition.y + physicsObject.Radius > physicsObject.ScreenMax.y ||
            futurePosition.y - physicsObject.Radius < -physicsObject.ScreenMax.y)
        {
            return Seek(Vector3.zero);
        }
        return Vector3.zero;
    }

    protected Vector3 Separate()
    {
        // Sum of all forces to separate
        Vector3 separateForce = Vector3.zero;

        foreach (GameObject bird in manager.allFish) 
        {
            // Worse distance getting method:
            float dist = Vector3.Distance(transform.position, bird.transform.position);

            // Better distance getting method:
            //float dist = Vector3.SqrMagnitude(Seek(bird));

            if (dist > Mathf.Epsilon)
            {
                separateForce += Flee(bird) * (separateRange / dist);
            }
        }
        return separateForce;
    }

    protected Vector3 Cohesion(List<GameObject> allFish)
    {
        Vector3 sum = Vector3.zero;
        int count = 0;
        foreach (GameObject other in allFish)
        {
            if (agentType == other.GetComponent<Agent>().agentType)
            {
                float d = Vector3.Distance(transform.position, other.transform.position);
                if ((d > 0) && (d < cohesionRange))
                {
                    sum += other.transform.position;
                    count++;
                }
            }
        }
        if (count > 0)
        {
            sum /= count;
            return Seek(sum);
        }
        else
        {
            return Vector3.zero;
        }
    }

    protected Vector3 Align(List<GameObject> allFish)
    {
        Vector3 sum = Vector3.zero;
        int count = 0;
        foreach (GameObject other in allFish)
        {
            if (agentType == other.GetComponent<Agent>().agentType)
            {
                float d = Vector3.Distance(transform.position, other.transform.position);
                if ((d > 0) && (d < alignRange))
                {
                    sum += other.GetComponent<PhysicsObject>().Velocity;
                    count++;
                }
            }
        }
        if (count > 0)
        {
            sum /= count;
            sum.Normalize();
            sum *= maxSpeed;
            Vector3 steer = (sum - this.GetComponent<PhysicsObject>().Velocity);
            steer = Vector3.ClampMagnitude(steer, maxForce);
            return steer;

        }
        else
        {
            return Vector3.zero;
        }

    }

    protected Vector3 AvoidObstacles(float avoidTime)
    {
        Vector3 totalAvoidForce = Vector3.zero;
        foundObstacles.Clear();


        foreach(GameObject obstacle in manager.allFish)
        {
            // A bunch of checks to see if I care

            Vector3 agentToObstacle = obstacle.transform.position - transform.position;
            float rightDot = 0, forwardDot = 0;

            forwardDot = Vector3.Dot(physicsObject.Direction, agentToObstacle);

            Vector3 futurePos = CalcFuturePosition(avoidTime);
            float dist = Vector3.Distance(transform.position, futurePos);

            if (agentType != obstacle.GetComponent<Agent>().agentType)
            {
                // If in front of me
                if (forwardDot >= -physicsObject.Radius)
                {
                    // Within the box in front of us
                    if (forwardDot <= dist + obstacle.GetComponent<PhysicsObject>().Radius)
                    {
                        // how far left/right?
                        rightDot = Vector3.Dot(transform.right, agentToObstacle);

                        Vector3 steeringForce = transform.right * (1 - forwardDot / dist) * physicsObject.MaxSpeed;

                        // Is the obstacle within the safe box width?
                        if (Mathf.Abs(rightDot) <= physicsObject.Radius + obstacle.GetComponent<PhysicsObject>().Radius)
                        {
                            // If I care
                            foundObstacles.Add(obstacle.transform.position);


                            // If left, steer right
                            if (rightDot < 0)
                            {
                                totalAvoidForce += steeringForce;
                            }

                            // If right, steer left
                            else
                            {
                                totalAvoidForce -= steeringForce;
                            }
                        }
                    }
                }
                // Otherwise it's behind me and I don't care
            }


        }

        return totalAvoidForce;
    }
}
