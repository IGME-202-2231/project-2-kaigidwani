using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seeker : Agent
{
    [SerializeField] GameObject target;

    protected override void CalcSteeringForces()
    {
        // Seek to where it is now
        physicsObject.ApplyForce(Seek(target));
    }

    protected override void Init()
    {
        // Nothing for this.
    }
}
