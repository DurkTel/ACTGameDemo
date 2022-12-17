using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DEMO_MoveFSM
{
    public class FSM_Status_Move : FSM_Status<FSM_Define.FSM_Status>
    {
        public override void OnAction()
        {
            Debug.Log("当前是Move状态");
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("进入Move状态");
        }

        public override void OnExit()
        {
            base.OnExit();
            Debug.Log("退出Move状态");
        }
    }
}