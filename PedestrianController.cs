using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCoordinator;

/// <summary>
/// NetworkCharacterVelocityで速度参照しているので同じオブジェクトに貼ること
/// </summary>
/// 
public class PedestrianController: MonoBehaviour
{
    [Header("値を入力")]

    public GameObject pathObject;

    [Space(5), Tooltip("初期位置と最終位置の座標")] public List<Vector3> path;
    //[Space(5), Tooltip("動き出すための車椅子の通過点")] public Vector3 triggerPoint;
    public bool useTrigger = true;
    PathLine line;
    //GameObject obj;
    //Vector3 lastPos_w;
    //Vector3 vel_w;
    Vector3 position;
    NetworkCharacterVelocity vel;
    public float velocity;
    [Header("----------------------------------------")]
    public float lookAheadDistance;
    [SerializeField]
    Vector3 uvw;
    [SerializeField]
    Vector3 targetPoint;
    Vector3 tmp;

    // Use this for initialization
    void Start()
    {
        vel = GetComponent<NetworkCharacterVelocity>();
        SettingPath();
        //obj = GameObject.Find("Wheelchair_High");
 
    }

    public void SettingPath()
    {
        line = new PathLine();

        PathCreation.PathCreator bpath = pathObject.GetComponent<PathCreation.PathCreator>();

        path = (new List<Vector3>(bpath.path.localPoints));

        for (int i = 0; i < path.Count; i++)
        {
            path[i] = bpath.gameObject.transform.position+ bpath.path.localPoints[path.Count-i-1];
        }

        line.SetPath(path);

        //line.SetPath(path);
        position = transform.position;
        line.SearchClosedIndexFromWholeArea(ref position);
        vel.velocity = new Vector3(0, 0, 0);
        Debug.Log("hello!");
    }

    // Update is called once per frame
    void Update()
    {
        position = transform.position;
        uvw = line.GetUVW(ref position);
        //vel_w = ((obj.transform.position - lastPos_w) / Time.deltaTime);

        //lastPos_w = obj.transform.position;

            if (lookAheadDistance <= 0)
            {
                targetPoint = transform.position + transform.forward;
            }
            else if (lookAheadDistance < (uvw.y > 0 ? uvw.y : -uvw.y))
            {
                targetPoint = line.GetPathRootPoint() + line.GetPathRootDirection() * uvw.x;
            }
            else
            {
                targetPoint = line.GetDistantPoint(ref position, lookAheadDistance);
            }

            tmp = targetPoint - position;

        if (useTrigger)
        {

            vel.velocity.z = 0;
            vel.velocity.x = 0;
            //if (obj.transform.position.z > triggerPoint.z)
            ////不等号の向きは区間によって適宜変更
            //{
            //    vel.velocity.x = tmp.normalized.x * velocity;
            //    vel.velocity.z = tmp.normalized.z * velocity;
            //}
        }
        else
        {
            vel.velocity.x = tmp.normalized.x * velocity;
            vel.velocity.z = tmp.normalized.z * velocity;
        }



        
    }
}
