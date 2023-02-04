using DEMO_MoveFSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DEMO_MoveFSM
{
    public class CharacterFSM : MonoBehaviour
    {
        private FSM_StateMachine<string> m_stateMachine;
        private InputEditor m_inputEditor;
        private void Start()
        {
            m_stateMachine = new FSM_StateMachine<string>();
            m_stateMachine.AddStatus("Idle", new CharacterFSM_Idle());
            m_stateMachine.AddStatus("Idle", new CharacterFSM_Locomotion());
            m_inputEditor = new InputEditor();
            //m_inputEditor.GamePlay.SetCallbacks()
        }

        private void Update()
        {


            m_stateMachine.activeState.OnAction();
        }

        private void OnAnimatorMove()
        {
            m_stateMachine.activeState.OnAnimatorMove();
        }
    }
}
