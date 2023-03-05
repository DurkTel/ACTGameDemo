using Demo_MoveMotor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Demo_MoveMotor.ICharacterControl;
using static UnityEngine.InputSystem.InputAction;

public class CharacterMotor_Controller : CharacterMotor_Animation
{
    protected override void Update()
    {
        base.Update();

    }

    protected override void FixedUpdate()
    {

        base.FixedUpdate();

        RequestClimb();
        RequestFall();
        RequestVault();

        ControlLocomotion();
        ControlRotationType();

        ControllWallRun();


    }

    protected override void OnAnimatorMove()
    {
        base.OnAnimatorMove();
    }

    public void SetTargetDirection(Vector3 direction)
    {
        m_targetDirection = direction;
    }

    public void SetInputDirection(Vector3 input)
    {
        CalculateRelativityTarget(input);
    }

    public virtual void ControlLocomotion()
    {
        if (IsEnableCurveMotion())
            CurveMove();
        else if (ControlMoveRootMotor())
            MoveByMotor();
        else
            MoveToDirection(m_isWallRunning ? m_wallRunForward : m_targetDirection);
    }

    public virtual void ControlRotationType()
    {
        if (ControlRotatioRootMotor())
            RotateByRootMotor();
        else
        {
            if (!m_isClimbing && !IsInAnimationName("Height Climb Exit"))
                RotateToDirection(m_isWallRunning ? m_wallRunForward : m_targetDirection);
        }
    }

    public virtual bool ControlMoveRootMotor()
    {
        if (IsEnableRootMotion(1))
            return true;

        return false;
    }

    public virtual bool ControlRotatioRootMotor()
    {
        if (IsEnableRootMotion(2))
            return true;

        if (IsInAnimationTag("Free Movement") && IsInTransition())
            return true;

        return false;
    }

    public virtual void RequestWalk(CallbackContext value)
    {
        m_isWalk = !m_isWalk;
        m_moveType = m_isWalk ? MoveType.WALK : MoveType.RUN;
    }

    public virtual void RequestSprint(InputActionPhase value)
    {
        if (m_isWalk) return;
        m_isSprint = value == InputActionPhase.Performed;
        m_moveType = m_isSprint ? MoveType.SPRINT : MoveType.RUN;
    }

    public virtual void RequestGazing(CallbackContext value)
    {
        m_isGazing = !m_isGazing;
        CalculateLockon();
    }

    public virtual void RequestEscape(CallbackContext value)
    {
        if (m_relativityForward == 0f && m_relativityRight == 0f) return;
        Escape();
    }

    public virtual void RequestJump(CallbackContext value)
    {
        if (++m_jumpCount >= m_jumpFrequency || m_isVault || m_isClimbing || IsInAnimationTag("BanJump") || IsInTransition()) return;
        m_isAirbone = true;
        Jump();

    }

    public virtual void RequestFall()
    {
        if (m_isAirbone || !isFall || m_isVault || m_isClimbing || m_isWallRunning || IsInTransition()) return;
        Fall();
    }

    public virtual void RequestVault()
    {
        if(!m_isVault && m_relativityForward >= 0.5f && !IsInTransition() && CalculateVault())
        {
            m_isVault = true;
            Vault();
        }
    }

    public virtual void RequestClimb()
    {
        if (!m_isClimbing && m_relativityForward >= 0.5f && !IsInTransition() && CalculateClimb(out int climbType))
        {
            m_isClimbing = true;
            if (climbType == 1)
                ShortClimb();
            else if (climbType == 2)
                HeightClimb();
        }

        if(m_isClimbing)
        {
            Vector3 p1 = rootTransform.position + Vector3.up * 1.75f;
            m_debugHelper.DrawLine(p1, p1 - rootTransform.right - rootTransform.forward * 0.2f, Color.blue);
            m_debugHelper.DrawLine(p1, p1 - rootTransform.right, Color.red);
            m_debugHelper.DrawLine(p1, p1 - rootTransform.forward, Color.red);

            m_targetDirection = Vector3.zero;
        }
    }

    public virtual void RequestClimbEnd(CallbackContext value)
    {
        if (m_isClimbing)
        {
            if (IsInAnimationName("Climb Jump Hold"))
            { 
                if(CalculateJumpClimb(DirectionCast.Backward))
                {
                    PlayAnimation("Climb Jump Mid", 0f);
                }
            }
            else if (m_relativityForward <= -0.5f)
            {
                m_isClimbing = false;
                PlayAnimation("Climb Jump Exit", 0f);
            }
            else if (!IsInAnimationName("Height Climb Up") && !IsInAnimationName("Height Climb End"))
            {
                Vector3 point = rootTransform.position;
                point.y = m_currentClimbPoint.y;
                m_stateInfos.AddMatchTargetList(new List<Vector3>() { point });
                PlayAnimation("Height Climb End", 0.05f);
            }

        }
    }

    protected bool m_wallRunHolding;
    public virtual void ControllWallRun()
    {
        if(CalculateWallRun(out RaycastHit wallHit, out m_wallRunDir) && !isGround)
        {
            if (!m_isWallRunning)
            { 
                m_isWallRunning = true;
                string animation = verticalSpeed <= -2f ? "Wall Run Jump Start" : "Wall Run Start";
                rootTransform.position = CalculateOffset(0.4f);
                PlayAnimation(animation, 0.05f);
                print("开启蹬墙跑");
            }
        }
        else
        {
            if (m_isWallRunning)
            {
                m_isWallRunning = false;
                print("结束蹬墙跑");
            }
        }

        if (m_isWallRunning)
        {
            //叉乘得出与墙面平行方向
            m_wallRunForward = Vector3.Cross(wallHit.normal, Vector3.up);
            //点乘得出与角色面朝方向相同的
            m_wallRunForward = Vector3.Dot(rootTransform.forward, m_wallRunForward) > 0 ? m_wallRunForward : -m_wallRunForward;
            m_wallRunForward = m_wallRunForward * Mathf.Abs(m_relativityForward);
            verticalSpeed = 0f;

            if (m_relativityForward < 0f)
            {
                rootTransform.rotation = Quaternion.LookRotation(-rootTransform.forward);
                m_wallRunDir = -m_wallRunDir;
            }

            if (m_wallRunDir == 0f) return;

            if (Mathf.Abs(m_relativityForward) == 0f)
            {
                if (!m_wallRunHolding)
                {
                    print("开始蹬墙跑悬挂");
                    m_wallRunHolding = true;
                    rootTransform.position = CalculateOffset(0.25f);
                }
            }
            else
            {
                if (m_wallRunHolding)
                {
                    print("结束蹬墙跑悬挂");
                    m_wallRunHolding = false;
                    rootTransform.position = CalculateOffset(0.4f);
                }
            }
        }

        Vector3 CalculateOffset(float offset)
        { 
            return wallHit.point + Vector3.down + wallHit.normal.normalized * offset;
        }
    }

}
