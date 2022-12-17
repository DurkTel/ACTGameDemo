﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DEMO_MoveFSM
{
    public class FSM_Status_Idle : FSM_Status<FSM_Define.FSM_Status>
    {
        public override void OnAction()
        {
            Debug.Log("当前是Idle状态");
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("进入Idle状态");
        }

        public override void OnExit()
        {
            base.OnExit();
            Debug.Log("退出Idle状态");
        }
    }
}