using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.DefaultInputActions;

public class PlayerController : PlayerAnimation
{
    public PlayerControllerActions actions;

    private PlayerAbility[] m_playerAbilities;

    [SerializeField]
    private PlayerAbility m_currentAbilitiy;

    public PlayerAbility currentAbilitiy { get { return m_currentAbilitiy; } }

    public Transform rootTransform { get; set; }

    public DebugHelper debugHelper { get; set; }

    public bool combatState { get; set; }

    public MoveController moveController;

    public Transform weaponPoint;

    protected override void Awake()
    {
        base.Awake();   
        actions = new PlayerControllerActions();
        m_playerAbilities = GetComponents<PlayerAbility>();
        moveController = GetComponent<MoveController>();
        debugHelper = GetComponent<DebugHelper>();
        Array.Sort(m_playerAbilities, (PlayerAbility x, PlayerAbility y) => { return y.priority - x.priority; });
    }

    protected override void Start()
    {
        base.Start();
        rootTransform = transform;
        RegisterAnimationEvents(m_playerAbilities);
    }

    protected override void Update()
    {
        base.Update();

        if (m_currentAbilitiy != null && m_currentAbilitiy.updateMode == AbilityUpdateMode.Update)
            m_currentAbilitiy.OnUpdateAbility();

        PackUpWeapon();
    }

    public void FixedUpdate()
    {
        if (m_currentAbilitiy != null && m_currentAbilitiy.updateMode == AbilityUpdateMode.FixedUpdate)
            m_currentAbilitiy.OnUpdateAbility();
    }

    public void OnAnimatorMove()
    {
        if (m_currentAbilitiy != null && m_currentAbilitiy.updateMode == AbilityUpdateMode.AnimatorMove)
            m_currentAbilitiy.OnUpdateAbility();
    }

    public void LateUpdate()
    {
        UpdateAbilities();
    }

    public void UpdateAbilities()
    {
        PlayerAbility nextAbility = m_currentAbilitiy;
        //执行优先级最高的行为
        foreach (var ability in m_playerAbilities)
        {
            if (ability == m_currentAbilitiy)
                continue;
            if (ability.Condition())
            {
                if (nextAbility == null || ability.priority > nextAbility.priority || !nextAbility.Condition())
                {
                    nextAbility = ability;
                    break;
                }
            }
        }

        //更新行为
        if (nextAbility != m_currentAbilitiy)
        {
            if (m_currentAbilitiy != null)
                m_currentAbilitiy.OnDisableAbility();

            nextAbility.OnEnableAbility();

            m_currentAbilitiy = nextAbility;
        }
    }

    protected override void UpdateAnimatorInfo()
    {
        base.UpdateAnimatorInfo();
        animator.SetBool(Bool_Ground_Hash, moveController.IsGrounded());
        animator.SetBool(Bool_Gazing_Hash, actions.gazing);

    }

    public float GetGroundClearance()
    {
        if (Physics.Raycast(rootTransform.position, Vector3.down, out RaycastHit raycastHit, 100f))
        { 
            return raycastHit.distance;
        }

        return 0f;
    }

    private void PackUpWeapon()
    {
        if (actions.weapon != combatState)
        {
            combatState = actions.weapon;
            SetAnimationState(combatState ? "Take Out Weapon" : "Pack Up Weapon");
            if (!combatState)
                TimerManager.Instance.AddTimer(() =>
                {
                    weaponPoint.gameObject.SetActive(false);
                }, 0f, 1f);
            else
            {
                weaponPoint.gameObject.SetActive(true);
                actions.lightAttack = false;
                actions.heavyAttack = false;
                actions.attackEx = false;
            }
        }
        animator.SetFloat(Float_IntroWeapon_Hash, combatState ? 1f : 0f, 0.2f, Time.deltaTime);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();   
        RemoveAnimationEvents(m_playerAbilities);
    }

}

public enum DirectionCast
{
    Left = -1,
    Right = 1,
    Forward = 2,
    Backward = -2,
    Up = 3,
    Down = -3,
}
