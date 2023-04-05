using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityUpdateMode
{
    Update,
    FixedUpdate,
    AnimatorMove
}

public enum AbilityType
{
    None,
    Locomotion,
    Airbone,
    Escape,
    Vault,
    Climb,
    WallRun,
    Combat,
    Hurt
}

public abstract class PlayerAbility : MonoBehaviour
{
    public int priority;

    public bool isEnable;
    [HideInInspector]
    public PlayerController playerController;

    public AbilityUpdateMode updateMode = AbilityUpdateMode.FixedUpdate;

    protected IMove m_moveController;

    protected PlayerControllerActions m_actions;

    public abstract AbilityType GetAbilityType();

    public abstract bool Condition();

    public virtual void OnEnableAbility() { }

    public virtual void OnUpdateAbility()
    {
        OnUpdateDeltaTime();
        OnUpdateAnimatorParameter();
    }

    public virtual void OnDisableAbility()
    {
        OnResetAnimatorParameter();
    }

    public virtual void OnUpdateAnimatorParameter() { }

    public virtual void OnResetAnimatorParameter() { }

    public virtual void OnAnimatorEvent(AnimationEventDefine cmd) { }

    public virtual AnimationEventDefine[] GetAnimatorEvent() { return null; }

    protected void OnUpdateDeltaTime()
    {
        switch (updateMode)
        {
            case AbilityUpdateMode.Update:
                m_moveController.deltaTtime = Time.deltaTime;
                break;
            case AbilityUpdateMode.FixedUpdate:
                m_moveController.deltaTtime = Time.fixedDeltaTime;
                break;
            case AbilityUpdateMode.AnimatorMove:
                m_moveController.deltaTtime = Time.deltaTime;
                break;
            default:
                break;
        }
    }

    protected virtual void Start()
    {
        playerController = GetComponent<PlayerController>();
        m_moveController = GetComponent<MoveController>();
        m_actions = playerController.actions;
    }
}
