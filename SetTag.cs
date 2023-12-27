using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTag : MonoBehaviour
{
    public string tag;

    // Start is called before the first frame update
    void Start()
    {
        Camera camera = this.GetComponent<Camera>();
        camera.tag = tag;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
