using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DEMO_MoveFSM
{
    public class FSM_Define
    {
        public enum FSM_Status
        {
            IDLE,
            MOVE,
            JUMP,
            CLIMB
        }
    }
}