using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterForwardMove : StateMachineBehaviour
{
    /// <summary>
    /// 记录前5s的向前速度
    /// </summary>
    private float[] m_forwardSpeedMark = new float[10];
    /// <summary>
    /// 记录前5s的向前速度数组下标
    /// </summary>
    private int m_forwardSpeedMarkIndex = 0;
    /// <summary>
    /// 时间标识
    /// </summary>
    private float m_timeFlag = 0;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(MoveMotorBase.SharpTurn_Hash);
        for (int i = 0; i < m_forwardSpeedMark.Length; i++)
            m_forwardSpeedMark[i] = 0;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_timeFlag >= 0.5f)
        {
            m_forwardSpeedMark[m_forwardSpeedMarkIndex++] = animator.GetFloat(MoveMotorBase.Forward_Hash);
            m_forwardSpeedMarkIndex = m_forwardSpeedMarkIndex >= 10 ? 0 : m_forwardSpeedMarkIndex;
            m_timeFlag = 0;
        }

        m_timeFlag += Time.deltaTime;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(MoveMotorBase.SharpTurn_Hash);

        float averageSpeed = 0f;
        int flag = 0;
        for (int i = 0; i < m_forwardSpeedMark.Length; i++)
        {
            averageSpeed += m_forwardSpeedMark[i];
            flag = m_forwardSpeedMark[i] != 0 ? flag + 1 : flag;
        }

    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Implement code that processes and affects root motion

    }

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
