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
    public float SeparateRange { get { return separateRange; } }

    protected Vector3 ultimaForce;

    [SerializeField] protected string agentType;

    [SerializeField] SceneManager manager;
    public SceneManager Manager { set { manager = value; } }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Update()
    {
        ultimaForce = Vector3.zero;

        CalcSteeringForces();

        Vector3.ClampMagnitude(ultimaForce, maxForce);
        physicsObject.ApplyForce(ultimaForce);
    }

    protected abstract void Init();

    protected abstract void CalcSteeringForces();

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
        /*
        if (wanderAngle > maxWanderAngle)
        {
            wanderAngle = maxWanderAngle;
        }
        else if (wanderAngle < -maxWanderAngle)
        {
            wanderAngle = -maxWanderAngle;
        }*/

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

        foreach (GameObject bird in manager.allBirds) 
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
}
