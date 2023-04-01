using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Events;

public class PlayerControllerInput : MonoBehaviour, CrossPlatformInput.IGamePlayActions
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
    /// <summary>
    /// 双击的间隔时间
    /// </summary>
    public double multiTime = 0.2f;
    /// <summary>
    /// 记录按钮行为
    /// </summary>
    public Dictionary<string, ButtonBehaviour> buttonBehaviour = new Dictionary<string, ButtonBehaviour>();
    /// <summary>
    /// 移动输入事件
    /// </summary>
    public event UnityAction<Vector2> moveInputEvent = delegate { };
    /// <summary>
    /// 按键按下事件
    /// </summary>
    public event UnityAction<string> buttonPressEvent = delegate { };
    /// <summary>
    /// 按键松开事件
    /// </summary>
    public event UnityAction<string> buttonReleaseEvent = delegate { };
    /// <summary>
    /// 按键双击事件
    /// </summary>
    public event UnityAction<string> buttonMultiEvent = delegate { };
    /// <summary>
    /// 按键长按事件
    /// </summary>
    public event UnityAction<string> buttonHoldEvent = delegate { };

    public void Awake()
    {
        inputActions = new CrossPlatformInput();
        inputActions.GamePlay.SetCallbacks(this);
        m_controller = GetComponent<PlayerController>();
        m_camera = Camera.main.GetComponent<OrbitCamera>();
        m_controller.actions.cameraTransform = m_camera.transform;
        InitGameplayButton();
    }

    public void OnEnable()
    {
        inputActions.Enable();

    }

    public void OnDisable()
    {
        inputActions.Disable();

    }

    private void Update()
    {
        MoveInput();
    }

    private void InitGameplayButton()
    {
        ReadOnlyArray<InputActionMap> actions = inputActions.asset.actionMaps;
        foreach (var actionMap in actions)
        {
            foreach (var action in actionMap)
            {
                if (!buttonBehaviour.ContainsKey(action.name))
                {
                    buttonBehaviour.Add(action.name, new ButtonBehaviour(action.name, action));
                }
            }
        }
    }

    private PlayerInputPhase ButtonHandle(InputAction.CallbackContext context)
    {
        buttonBehaviour[context.action.name].onMulti = false;

        if (context.phase == InputActionPhase.Started)
        {
            buttonPressEvent.Invoke(context.action.name);
            //需求要按下算一次点击，自带的双击判断是松开算一次点击，自己模拟一下
            if (context.startTime - buttonBehaviour[context.action.name].startTime <= multiTime)
            {
                buttonBehaviour[context.action.name].onMulti = true;
                buttonMultiEvent.Invoke(context.action.name);
                return PlayerInputPhase.DoubleClick;
            }
            buttonBehaviour[context.action.name].startTime = context.startTime;
            return PlayerInputPhase.Click;
        }
        else if (context.phase == InputActionPhase.Performed)
        {
            if (context.interaction is HoldInteraction)
            {
                buttonHoldEvent.Invoke(context.action.name);
                return PlayerInputPhase.Hold;
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            buttonReleaseEvent.Invoke(context.action.name);
            return PlayerInputPhase.Release;
        }

        return PlayerInputPhase.None;
    }

    public virtual void MoveInput()
    {
        float up = m_controller.actions.moveUp ? 1f : 0f;
        float down = m_controller.actions.moveDown ? -1f : 0f;
        float left = m_controller.actions.moveLeft ? -1f : 0f;
        float right = m_controller.actions.moveRight ? 1f : 0f;

        m_inputDirection.Set(left + right, up + down);
        m_currentDirection.x = m_inputDirection.x;
        m_currentDirection.z = m_inputDirection.y;

        m_currentDirectionSmooth = Vector3.Lerp(m_currentDirectionSmooth, m_currentDirection, Time.deltaTime);

        Vector3 moveDirection = m_camera.transform.TransformDirection(m_currentDirection);
        
        m_controller.actions.move = moveDirection;

        if (m_inputDirection.Equals(Vector2.zero))
            m_controller.actions.sprint = false;
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        ButtonHandle(context);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        m_camera.GetAxisInput(inputActions.GamePlay.Look.ReadValue<Vector2>());
    }

    public void OnWalk(InputAction.CallbackContext context)
    {
        if (ButtonHandle(context) == PlayerInputPhase.Click)
            m_controller.actions.walk = !m_controller.actions.walk;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (ButtonHandle(context) == PlayerInputPhase.Click)
            m_controller.actions.jump = true;
    }

    public void OnLock(InputAction.CallbackContext context)
    {
        PlayerInputPhase phase = ButtonHandle(context);
        m_controller.actions.gazing = phase != PlayerInputPhase.Release && phase != PlayerInputPhase.None;
        m_camera.CalculateLockon(m_controller.actions.gazing);
    }

    public void OnEscape(InputAction.CallbackContext context)
    {
        if (ButtonHandle(context) == PlayerInputPhase.Click)
            m_controller.actions.escape = true;
    }

    public void OnWeapon(InputAction.CallbackContext context)
    {
        if (ButtonHandle(context) == PlayerInputPhase.Click)
            m_controller.actions.weapon = !m_controller.actions.weapon;
    }

    public void OnLightAttack(InputAction.CallbackContext context)
    {
        if (ButtonHandle(context) == PlayerInputPhase.Click)
            m_controller.actions.lightAttack = true;
    }

    public void OnHeavyAttack(InputAction.CallbackContext context)
    {
        if (ButtonHandle(context) == PlayerInputPhase.Click)
            m_controller.actions.heavyAttack = true;
    }

    public void OnMoveUp(InputAction.CallbackContext context)
    {
        PlayerInputPhase phase = ButtonHandle(context);
        m_controller.actions.moveUp = phase != PlayerInputPhase.Release && phase != PlayerInputPhase.None;

        if (phase == PlayerInputPhase.DoubleClick)
            m_controller.actions.sprint = true;
    }

    public void OnMoveDown(InputAction.CallbackContext context)
    {
        PlayerInputPhase phase = ButtonHandle(context);
        m_controller.actions.moveDown = phase != PlayerInputPhase.Release && phase != PlayerInputPhase.None;

        if (phase == PlayerInputPhase.DoubleClick)
            m_controller.actions.sprint = true;
    }

    public void OnMoveLeft(InputAction.CallbackContext context)
    {
        PlayerInputPhase phase = ButtonHandle(context);
        m_controller.actions.moveLeft = phase != PlayerInputPhase.Release && phase != PlayerInputPhase.None;

        if (phase == PlayerInputPhase.DoubleClick)
            m_controller.actions.sprint = true;
    }

    public void OnMoveRight(InputAction.CallbackContext context)
    {
        PlayerInputPhase phase = ButtonHandle(context);
        m_controller.actions.moveRight = phase != PlayerInputPhase.Release && phase != PlayerInputPhase.None;

        if (phase == PlayerInputPhase.DoubleClick)
            m_controller.actions.sprint = true;
    }
}

/// <summary>
/// 按键行为
/// </summary>
public class ButtonBehaviour
{
    public string actionName;
    public double startTime;
    public bool onPressed { get { return inputAction.phase == InputActionPhase.Started; } }
    public bool onRelease { get { return inputAction.phase == InputActionPhase.Performed; } }
    public bool onMulti { get; set; }
    public bool onHold { get; set; }
    public InputAction inputAction;
    public ButtonBehaviour(string actionName, InputAction inputAction)
    {
        this.actionName = actionName;
        this.inputAction = inputAction;
    }
}

public enum PlayerInputPhase
{
    None,
    Click,
    DoubleClick,
    Hold,
    Release
}
