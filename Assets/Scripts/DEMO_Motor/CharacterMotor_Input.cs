using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Demo_MoveMotor.ICharacterControl;

namespace Demo_MoveMotor
{
    public class CharacterMotor_Input : MonoBehaviour
    {
        public CrossPlatformInput inputActions;

        [SerializeField, Header("������")]
        protected OrbitCamera m_camera;
        /// <summary>
        /// ���ƽű�
        /// </summary>
        protected CharacterMotor_Controller m_controller;   
        /// <summary>
        /// ���뷽��
        /// </summary>
        protected Vector2 m_inputDirection;
        /// <summary>
        /// ��ǰ����
        /// </summary>
        protected Vector3 m_currentDirection;
        /// <summary>
        /// Ŀ�귽��
        /// </summary>
        protected Vector3 m_targetDirection;

        protected Vector3 m_currentDirectionSmooth;

        public void Awake()
        {
            inputActions = new CrossPlatformInput();
            m_controller = GetComponent<CharacterMotor_Controller>();
        }

        public void OnEnable()
        {
            inputActions.Enable();
            inputActions.GamePlay.Walk.performed += m_controller.ControlWalk;
            inputActions.GamePlay.Lock.performed += m_controller.ControlGazing;
            inputActions.GamePlay.Escape.performed += m_controller.ControlEscape;
            inputActions.GamePlay.Jump.performed += m_controller.ControlJump;

        }

        public void OnDisable()
        {
            inputActions.Disable();
            inputActions.GamePlay.Walk.performed -= m_controller.ControlWalk;
            inputActions.GamePlay.Lock.performed -= m_controller.ControlGazing;
            inputActions.GamePlay.Escape.performed -= m_controller.ControlEscape;
            inputActions.GamePlay.Jump.performed -= m_controller.ControlJump;

        }

        public void Update()
        {
            MoveInput();
            CameraInput();
        }

        public virtual void MoveInput()
        {
            m_inputDirection = inputActions.GamePlay.Move.ReadValue<Vector2>();
            m_currentDirection.x = m_inputDirection.x;
            m_currentDirection.z = m_inputDirection.y;

            m_currentDirectionSmooth = Vector3.Lerp(m_currentDirectionSmooth, m_currentDirection, Time.deltaTime);

            Vector3 moveDirection = m_camera.transform.TransformDirection(m_currentDirection);
            m_controller.SetTargetDirection(moveDirection);
            m_controller.SetInputDirection(m_inputDirection);

            m_controller.ControlSprint(inputActions.GamePlay.Run.phase);
        }

        public virtual void CameraInput()
        {
            m_camera.GetAxisInput(inputActions.GamePlay.Look.ReadValue<Vector2>());
        }
    }
}
