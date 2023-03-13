using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerActions
{
    public Vector3 move = Vector3.zero;
    public bool sprint = false;
    public bool walk = false;
    public bool jump = false;


    public void ResetActions()
    {
        sprint = false;
        walk = false;
    }
}
