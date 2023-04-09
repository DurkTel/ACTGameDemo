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
    [SerializeField]
    protected bool m_isEnable;
    [HideInInspector]
    public PlayerController playerController;

    public AbilityUpdateMode updateMode = AbilityUpdateMode.FixedUpdate;

    public MoveController moveController;

    protected PlayerControllerActions m_actions;

    public abstract AbilityType GetAbilityType();

    public abstract bool Condition();

    public virtual void OnEnableAbility() 
    {
        m_isEnable = true;
    }

    public virtual void OnUpdateAbility()
    {
        OnUpdateDeltaTime();
        OnUpdateAnimatorParameter();
    }

    public virtual void OnDisableAbility()
    {
        m_isEnable = false;
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
                moveController.deltaTtime = Time.deltaTime;
                break;
            case AbilityUpdateMode.FixedUpdate:
                moveController.deltaTtime = Time.fixedDeltaTime;
                break;
            case AbilityUpdateMode.AnimatorMove:
                moveController.deltaTtime = Time.deltaTime;
                break;
            default:
                break;
        }
    }

    protected virtual void Start()
    {
        playerController = GetComponent<PlayerController>();
        moveController = playerController.moveController;
        m_actions = playerController.actions;
    }
}
