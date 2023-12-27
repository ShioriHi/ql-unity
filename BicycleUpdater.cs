using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BicycleUpdater : MonoBehaviour
{
    public enum BicycleUpdateMethod
    {
        CharacterVelocity,
        Network,
        Manual,
    }

    public BicycleUpdateMethod updateMethod;

    NetworkCharacterVelocity vel;
    BicycleAnimaterChanger bicycle;
    DataStructGetter getter;

    // public GameObject manualBicycleObject;
    BicycleModelCalculator bicycleCul;

    float hight = 0.53f;
    Vector3 hightVec;
    float yaw;

    // Start is called before the first frame update
    void Start()
    {
        bicycle = this.GetComponent<BicycleAnimaterChanger>();

        vel = this.GetComponent<NetworkCharacterVelocity>();

        if (updateMethod == BicycleUpdateMethod.Network)
        {
            getter = this.GetComponent<DataStructGetter>();
        }

        hightVec = new Vector3(0, hight, 0);
        yaw = this.transform.localRotation.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (updateMethod == BicycleUpdateMethod.CharacterVelocity)
        {
            bicycle.velocity = vel.velocity.magnitude;

            this.transform.position += vel.velocity * Time.deltaTime;

            if(vel.velocity.z != 0)
            {
                this.transform.localRotation = Quaternion.Euler(0, Mathf.Atan2(vel.velocity.x, vel.velocity.z) * 180 / Mathf.PI, 0);
            }

        }
        else if (updateMethod == BicycleUpdateMethod.Network)
        {
            vel.velocity.x = getter.unit.accelpedal;
            vel.velocity.y = getter.unit.brakepedal;
            vel.velocity.z = getter.unit.speed;
            bicycle.velocity = vel.velocity.magnitude;
            //bicycle.yaw = getter.unit.yaw;
            bicycle.roll = getter.unit.eng_rpm;
            bicycle.steer = getter.unit.steering;

        }
        else if (updateMethod == BicycleUpdateMethod.Manual)
        {
            this.transform.localRotation = Quaternion.Euler(0, yaw, bicycle.roll * 180 / Mathf.PI);

            this.transform.localPosition = transform.localRotation * hightVec;

            vel.subObject1 = bicycle.steer;
            vel.subObject2 = bicycle.roll;
        }
    }
}
