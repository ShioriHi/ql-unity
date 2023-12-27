using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BicycleModelCalculator : MonoBehaviour
{

    public GameObject steer, driveGear, leanRight, learnLeft;
    private SerialCommunication steerSerial, driveGearSerial, leanRightSerial, learnLeftSerial;
    [Header("----------------------")]
    public int steerData;
    public int driveGearData;
    public int leanRightData;
    public int learnLeftData;
    [Header("----------------------")]
    public float velocity;
    //public float accelaration;
    public float steerAngle;
    public float rollAngle;
    [Header("----------------------")]
    public float yaw;
    public float yawRate;
    public float slipAngle;
    public float yawRateDot;
    public float slipAngleDot;
    public float x;
    public float y;
    [Header("----------------------")]
    public float mass;
    public float frontCenter;
    public float rearCenter;
    public float centerHight;
    public float inertiaMomentYaw;
    public float inertiaMomentRoll;
    public float frontCorneringStiffness;
    public float rearCorneringStiffness;
    public float frontCamberThrust;
    public float rearCamberThrust;

    const float gravitationalAcceleration = 9.798f;

    float A1, A2, A3, A5;
    float B2, B4, B5;

    float lastTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Register serial ports
        if (steer != null) steerSerial = steer.GetComponent<SerialCommunication>();
        if (driveGear != null) driveGearSerial = driveGear.GetComponent<SerialCommunication>();
        if (leanRight != null) leanRightSerial = leanRight.GetComponent<SerialCommunication>();
        if (learnLeft != null) learnLeftSerial = learnLeft.GetComponent<SerialCommunication>();

        //A1*V,(A1*v+A2/V),A3-mass*a,frontCorneringStiffness,A5

        A1 = mass * (1 + mass * centerHight * centerHight / inertiaMomentRoll);
        A2 = (frontCorneringStiffness * frontCenter - rearCorneringStiffness * rearCenter);
        A3 = frontCorneringStiffness + rearCorneringStiffness;
        A5 = (gravitationalAcceleration * mass * mass * centerHight * centerHight / inertiaMomentRoll + frontCamberThrust + rearCamberThrust);

        //inertiaMomentYaw,B2/V,A2,B4,B5

        B2 = (frontCorneringStiffness * frontCenter * frontCenter - rearCorneringStiffness * rearCenter * rearCenter);
        B4 = frontCorneringStiffness * frontCenter;
        B5 = frontCamberThrust * frontCenter - rearCamberThrust * rearCenter;

        x = this.transform.localPosition.x;
        y = this.transform.localPosition.z;
    }

    const float dt = 0.001f;

    // Update is called once per frame
    void Update()
    {
        //Get Data
        if (steerSerial != null) steerData = steerSerial.data;
        if (driveGearSerial != null) driveGearData = driveGearSerial.data;
        if (leanRightSerial != null) leanRightData = leanRightSerial.data;
        if (learnLeftSerial != null) learnLeftData = learnLeftSerial.data;

        if (velocity <= 0)
            return;

        while (lastTime < Time.time)
        {
            UpdateState(dt);
            lastTime += dt;
        }

        this.transform.localPosition = new Vector3(x, 0, y);
        this.transform.rotation = Quaternion.Euler(0, -(float)(yaw*180/System.Math.PI), 0);


    }

    void UpdateState(float dt)
    {
        //yawRateDot = frontCorneringStiffness * steerAngle - (gravitationalAcceleration * mass * mass * centerHight * centerHight / inertiaMomentRoll + frontCamberThrust + rearCamberThrust) * rollAngle

        //slipAngleDot = -(A1 * velocity + A2 / velocity) * slipAngle - (A3 - mass * acceleration) * yawRate + frontCorneringStiffness * steerAngle + A5 * rollAngle;
        slipAngleDot = -(A1 * velocity + A2 / velocity) * yawRate- (A3) * slipAngle + frontCorneringStiffness * steerAngle - A5 * rollAngle;
        slipAngleDot /= A1* velocity;


        yawRateDot = -B2 / velocity * yawRate - A2 * slipAngle + B4 * steerAngle + B5 * rollAngle;
        yawRateDot /= inertiaMomentYaw;

        slipAngle += slipAngleDot * dt;

        if (slipAngle < -0.3)
            slipAngle = -0.3f;
        else if (slipAngle > 0.3)
            slipAngle = 0.3f;

        yawRate += yawRateDot * dt;
        yaw += yawRate * dt;

        x += (float)System.Math.Cos(yaw) * velocity * dt;
        y += (float)System.Math.Sin(yaw) * velocity * dt;
    }
}
