using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityUpdateMode
{
    Update,
    FixedUpdate,
    AnimatorMove
}

public abstract class PlayerAbility : MonoBehaviour
{
    public int priority;

    public bool isEnable;
    [HideInInspector]
    public PlayerController playerController;

    public AbilityUpdateMode updateMode = AbilityUpdateMode.FixedUpdate;

    public abstract bool Condition();

    public virtual void OnEnableAbility() { }

    public virtual void OnUpdateAbility()
    {
        OnUpdateAnimatorParameter();
    }

    public virtual void OnDisableAbility()
    {
        OnResetAnimatorParameter();
    }

    public virtual void OnUpdateAnimatorParameter() { }

    public virtual void OnResetAnimatorParameter() { }

    public virtual void OnAnimatorEvent(AnimationEvent cmd) { }

    public virtual AnimationEvent[] GetAnimatorEvent() { return null; }

    protected PlayerControllerActions m_actions;

    protected virtual void Start()
    {
        playerController = GetComponent<PlayerController>();
        m_actions = playerController.actions;
    }
}
