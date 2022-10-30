using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Vector3 oleVect;
    public Vector3 newVect;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        newVect = transform.TransformDirection(oleVect);
    }
}
