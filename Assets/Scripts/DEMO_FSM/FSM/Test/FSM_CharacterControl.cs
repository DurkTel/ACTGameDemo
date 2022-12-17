using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DEMO_MoveFSM
{
    public class FSM_CharacterControl : MonoBehaviour
    {
        private FSM_StateMachine<FSM_Define.FSM_Status> m_stateMachine;

        private void Start()
        {
            m_stateMachine = new FSM_StateMachine<FSM_Define.FSM_Status>();
            m_stateMachine.AddStatus(FSM_Define.FSM_Status.IDLE, new FSM_Status_Idle());
            m_stateMachine.AddStatus(FSM_Define.FSM_Status.MOVE, new FSM_Status_Move());
        }
    }
}
