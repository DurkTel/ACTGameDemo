using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using static UnityEngine.InputSystem.InputAction;

public class PlayerControllerInput : MonoBehaviour
{
    public CrossPlatformInput inputActions;

    protected OrbitCamera m_camera;
    /// <summary>
    /// 控制脚本
    /// </summary>
    protected PlayerController m_controller;
    /// <summary>
    /// 输入方向
    /// </summary>
    protected Vector2 m_inputDirection;
    /// <summary>
    /// 当前方向
    /// </summary>
    protected Vector3 m_currentDirection;
    /// <summary>
    /// 目标方向
    /// </summary>
    protected Vector3 m_targetDirection;

    protected Vector3 m_currentDirectionSmooth;

    public void Awake()
    {
        inputActions = new CrossPlatformInput();
        m_controller = GetComponent<PlayerController>();
        m_camera = Camera.main.GetComponent<OrbitCamera>();
        m_controller.actions.cameraTransform = m_camera.transform;
    }

    public void OnEnable()
    {
        inputActions.Enable();
        inputActions.GamePlay.Walk.performed += RequestWalk;
        inputActions.GamePlay.Escape.performed += RequestEscape;
        //inputActions.GamePlay.Escape.performed += m_controller.RequestClimbEnd;
        inputActions.GamePlay.Jump.performed += RequestJump;
        inputActions.GamePlay.Weapon.performed += RequestWeapon;
        inputActions.GamePlay.LightAttack.performed += RequestLightAttack;
        inputActions.GamePlay.HeavyAttack.performed += RequestHeavyAttack;
        //inputActions.GamePlay.Jump.performed += m_controller.RequestClimbEnd;

    }

    public void OnDisable()
    {
        inputActions.Disable();
        inputActions.GamePlay.Walk.performed -= RequestWalk;
        inputActions.GamePlay.Escape.performed -= RequestEscape;
        //inputActions.GamePlay.Escape.performed -= m_controller.RequestClimbEnd;
        inputActions.GamePlay.Jump.performed -= RequestJump;
        inputActions.GamePlay.Weapon.performed -= RequestWeapon;
        inputActions.GamePlay.LightAttack.performed -= RequestLightAttack;
        inputActions.GamePlay.HeavyAttack.performed -= RequestHeavyAttack;
        //inputActions.GamePlay.Jump.performed -= m_controller.RequestClimbEnd;

    }

    private void Update()
    {
        MoveInput();
        CameraInput();
    }

    private void FixedUpdate()
    {
        GazingInput();

    }

    public virtual void MoveInput()
    {
        float up = inputActions.GamePlay.MoveUp.phase == InputActionPhase.Performed ? 1f : 0f;
        float down = inputActions.GamePlay.MoveDown.phase == InputActionPhase.Performed ? -1f : 0f;
        float left = inputActions.GamePlay.MoveLeft.phase == InputActionPhase.Performed ? -1f : 0f;
        float right = inputActions.GamePlay.MoveRight.phase == InputActionPhase.Performed ? 1f : 0f;
        m_inputDirection.Set(left + right, up + down);
        m_currentDirection.x = m_inputDirection.x;
        m_currentDirection.z = m_inputDirection.y;

        m_currentDirectionSmooth = Vector3.Lerp(m_currentDirectionSmooth, m_currentDirection, Time.deltaTime);

        Vector3 moveDirection = m_camera.transform.TransformDirection(m_currentDirection);
        
        m_controller.actions.move = moveDirection;
        m_controller.actions.sprint = inputActions.GamePlay.Run.phase == InputActionPhase.Performed;
    }

    public virtual void CameraInput()
    {
        m_camera.GetAxisInput(inputActions.GamePlay.Look.ReadValue<Vector2>());
    }


    private void GazingInput()
    {
        m_controller.actions.gazing = inputActions.GamePlay.Lock.phase == InputActionPhase.Performed;
        m_camera.CalculateLockon(m_controller.actions.gazing);
    }

    private void RequestWalk(CallbackContext value)
    {
        m_controller.actions.walk = !m_controller.actions.walk;
    }


    private void RequestJump(CallbackContext value)
    {
        m_controller.actions.jump = value.performed;
    }

    private void RequestEscape(CallbackContext value)
    {
        m_controller.actions.escape = value.performed;
    }

    private void RequestWeapon(CallbackContext value)
    {
        m_controller.actions.weapon = !m_controller.actions.weapon;
    }

    private void RequestLightAttack(CallbackContext value)
    {
        m_controller.actions.lightAttack = value.performed;
    }

    private void RequestHeavyAttack(CallbackContext value)
    {
        m_controller.actions.heavyAttack = value.performed;
    }
}
