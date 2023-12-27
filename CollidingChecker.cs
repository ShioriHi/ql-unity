using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidingChecker : MonoBehaviour
{
    public bool isCollided;
    int count;

    // Start is called before the first frame update
    void Start()
    {
        isCollided = false;
        count = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        count++;
        isCollided = true;
    }

    private void OnTriggerExit(Collider other)
    {
        count--;
        if(count == 0)
            isCollided = false;
    }

}
