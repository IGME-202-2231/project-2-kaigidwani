using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fleer : Agent
{
    [SerializeField] GameObject target;

    protected override void CalcSteeringForces()
    {
        physicsObject.ApplyForce(Flee(target));
    }

    protected override void Init()
    {
        // Nothing for this.
    }
}
