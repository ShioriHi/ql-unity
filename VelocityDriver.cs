using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityDriver : MonoBehaviour
{
    NetworkCharacterVelocity vel;
    Quaternion yawRotate;

    // Start is called before the first frame update
    void Start()
    {
        vel = this.GetComponent<NetworkCharacterVelocity>();
        yawRotate = Quaternion.Euler(0, -90, 0);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.localPosition += vel.velocity * Time.deltaTime;
        if (vel.velocity.magnitude > 0.1)
        {
            transform.forward = vel.velocity;
            transform.rotation *= yawRotate;
        }
    }
}
