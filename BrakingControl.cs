using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrakingControl : MonoBehaviour
{
    public GameObject colliderObject;
    public float brakingAcceleration,acceleration;


    float targetVelocity,velocity;
    PedestrianController controller;
    CollidingChecker cc;

    // Start is called before the first frame update
    void Start()
    {
        controller = this.GetComponent<PedestrianController>();
        targetVelocity = controller.velocity;
        cc = colliderObject.GetComponent<CollidingChecker>();
        velocity = targetVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        if (cc.isCollided)
        {
            velocity -= brakingAcceleration*Time.deltaTime;
            if (velocity < 0)
                velocity = 0;
        }
        else
        {
            if(velocity < targetVelocity)
            {
                velocity += acceleration * Time.deltaTime;
                if (velocity > targetVelocity)
                    velocity = targetVelocity;
            }
        }

        controller.velocity = velocity;
    }
}
