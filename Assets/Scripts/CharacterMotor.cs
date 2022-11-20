using DURK.CharacterControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MoveMotorBase;
using UnityEngine.InputSystem;

public class CharacterMotor : MonoBehaviour, ICharacterControl
{
    #region �ƶ�����
    [SerializeField, Header("����㼶")]
    protected LayerMask m_groundLayer;

    [SerializeField, Header("ǽ�ڲ㼶")]
    protected LayerMask m_wallLayer;

    [SerializeField, Header("�����ٶ�")]
    protected float m_moveSpeed_Walk = 1.45f;

    [SerializeField, Header("�ܶ��ٶ�")]
    protected float m_moveSpeed_Run = 2.7f;

    [SerializeField, Header("����ٶ�")]
    protected float m_moveSpeed_Rash = 5.85f;

    [SerializeField, Header("������ת�ٶ�")]
    protected float m_rotateSpeed_Walk = 300f;

    [SerializeField, Header("�ܶ���ת�ٶ�")]
    protected float m_rotateSpeed_Run = 2f;

    [SerializeField, Header("�Ϳ���ת�ٶ�")]
    protected float m_rotateSpeed_Air = 200f;

    [SerializeField, Header("��ת����ת�ٶ�")]
    protected float m_rotateSpeed_Sharp = 50f;

    [SerializeField, Header("��Ծ�߶�")]
    protected float m_jumpHeight = 2f;

    [SerializeField, Header("��Ծ����")]
    protected int m_jumpFrequency = 2;

    [SerializeField, Header("�������ٶ�")]
    protected float m_gravity = -9.8f;

    [SerializeField, Header("��������")]
    protected float m_airDamping = 2f;
    #endregion

    public Transform rootTransform { get; set; }
    public Animator animator { get; set; }
    public float forwardSpeed { get; set; }
    public float verticalSpeed { get; set; }
    public bool isGround { get; set; }
    public bool isFall { get; set; }
    public CharacterController characterController { get; set; }

    protected Camera m_mainCamera;

    protected float m_targetSpeed;

    protected Vector2 m_inputDirection;

    protected Vector3 m_targetDirection;

    protected Vector3 m_currentDirection;

    protected Vector3[] m_speedMark = new Vector3[3];

    protected int m_speedMarkIndex;

    protected ICharacterControl.MoveType m_moveType = ICharacterControl.MoveType.RUN;

    protected ICharacterControl.MovementType m_movementType = ICharacterControl.MovementType.IDLE;

    private Transform m_leftFootTran;

    private Transform m_rightFootTran;

    private bool m_holdJumpBtn;

    private int m_jumpCount;

    private bool m_jumpFlag;


    protected virtual void Start()
    {
        m_mainCamera = Camera.main;
        animator = GetComponent<Animator>();
        characterController = GetComponentInParent<CharacterController>();
        rootTransform = characterController.gameObject.transform;
        m_leftFootTran = transform.Find("root/pelvis/thigh_l/calf_l/foot_l/ball_l");
        m_rightFootTran = transform.Find("root/pelvis/thigh_r/calf_r/foot_r/ball_r");
    }

    protected virtual void Update()
    {
        CalculateFootStep();
        CalculateWallSpace();
        CalculateGravity();
        CalculateGround();
        UpdateMovementType();
    }

    protected virtual void OnAnimatorMove()
    {
        UpdateRotate();
        UpdateMove();
    }

    public void CalculateGravity()
    {
        float damping = UpdateAirDamping();
        verticalSpeed = isGround && verticalSpeed <= 0f ? 0f : verticalSpeed + (damping + m_gravity) * Time.deltaTime;
    }

    public void CalculateFootStep()
    {
        Vector3 localForward = transform.TransformPoint(Vector3.forward);
        float left = Vector3.Dot(localForward, m_leftFootTran.position);
        float right = Vector3.Dot(localForward, m_rightFootTran.position);
        animator.SetInteger(ICharacterControl.Int_FootStep_Hash, left > right ? -1 : 1);

#if UNITY_EDITOR
        Debug.DrawLine(localForward, m_leftFootTran.position, Color.green);
        Debug.DrawLine(localForward, m_rightFootTran.position, Color.green);
#endif
    }

    public void CalculateGround()
    {
        if (Physics.SphereCast(rootTransform.position + Vector3.up * 0.5f, characterController.radius,
            Vector3.down, out RaycastHit hitInfo, 0.5f - characterController.radius + characterController.skinWidth * 2, m_groundLayer))
        {
            m_jumpCount = 0;
            isGround = true;
            m_jumpFlag = false;
        }
        else
        {
            isGround = false;
        }
        isFall = !Physics.SphereCast(rootTransform.position + Vector3.up * 0.5f, characterController.radius, Vector3.down, out RaycastHit hit, 1f, m_groundLayer);

        animator.SetBool(ICharacterControl.Bool_Fall_Hash, isFall);
        animator.SetBool(ICharacterControl.Bool_Ground_Hash, isGround);
    }

    public void CalculateWallSpace()
    {
        Debug.DrawRay(rootTransform.position + Vector3.up + Vector3.up * 0.5f, rootTransform.forward, Color.red);
        if (Physics.Raycast(rootTransform.position + Vector3.up + Vector3.up * 0.5f, rootTransform.forward, out RaycastHit obsHit, 1f, m_wallLayer))
        {
            //ǽ��ķ��߷���
            Vector3 climbHitNormal = obsHit.normal; 
            Debug.DrawRay(rootTransform.position + Vector3.up + Vector3.up * 1f, -climbHitNormal, Color.red);
            if (Physics.Raycast(rootTransform.position + Vector3.up + Vector3.up * 1f, -climbHitNormal, out RaycastHit hit2, 1f, m_wallLayer))
            {
                Debug.DrawRay(rootTransform.position + Vector3.up + Vector3.up * 1.5f, -climbHitNormal, Color.red);
                if (Physics.Raycast(rootTransform.position + Vector3.up + Vector3.up * 1.5f, -climbHitNormal, out RaycastHit hit3, 1f, m_wallLayer))
                {

                }
                else
                {
                    Debug.DrawRay(hit2.point + Vector3.up * 2f, Vector3.down * 2f, Color.red);
                    if (Physics.Raycast(hit2.point + Vector3.up * 2f, Vector3.down, 2f))
                    {
                        animator.SetInteger(ICharacterControl.Int_ClimbType_Hash, 1);
                    }
                }
            }
        }
    }

    public float GetMoveSpeed()
    {
        float moveSpeed = m_rotateSpeed_Run;

        switch (m_moveType)
        {
            case ICharacterControl.MoveType.WALK:
                moveSpeed = m_moveSpeed_Walk;
                break;
            case ICharacterControl.MoveType.RUN:
                moveSpeed = m_moveSpeed_Run;
                break;
            case ICharacterControl.MoveType.DASH:
                moveSpeed = m_moveSpeed_Rash;
                break;
            default:
                break;
        }

        return moveSpeed;
    }

    public float GetRotateSpeed()
    {
        float rotateSpeed = m_rotateSpeed_Run;
        if (m_movementType == ICharacterControl.MovementType.JUMP || m_movementType == ICharacterControl.MovementType.FALL)
            return m_rotateSpeed_Air;
        else if (animator.CurrentlyInAnimationTag("SharpTurn"))
            return m_rotateSpeed_Sharp;

        switch (m_moveType)
        {
            case ICharacterControl.MoveType.WALK:
                rotateSpeed = m_rotateSpeed_Walk;
                break;
            case ICharacterControl.MoveType.RUN:
                rotateSpeed = m_rotateSpeed_Run;
                break;
            case ICharacterControl.MoveType.DASH:
                rotateSpeed = 0;
                break;
            default:
                break;
        }


        return rotateSpeed;
    }

    public void UpdateMovementType()
    {
        m_movementType = ICharacterControl.MovementType.IDLE;

        if (Mathf.Abs(verticalSpeed) > 0f)
            m_movementType = !m_jumpFlag && verticalSpeed < 0f ? ICharacterControl.MovementType.FALL : ICharacterControl.MovementType.JUMP;
        else if (forwardSpeed >= 0.01f)
            m_movementType = ICharacterControl.MovementType.MOVE;

        animator.SetInteger(ICharacterControl.Int_MovementType_Hash, (int)m_movementType);
    }

    public void UpdateMove()
    {
        animator.SetInteger(ICharacterControl.Int_MoveState_Hash, (int)m_moveType);

        //1.45f��5.85f����ֵ�ɶ���Ƭ�μ���ó�
        m_targetSpeed = GetMoveSpeed();
        m_targetSpeed *= m_inputDirection.magnitude;
        forwardSpeed = Mathf.Lerp(forwardSpeed, m_targetSpeed, 0.1f);
        forwardSpeed = forwardSpeed <= 0.01f ? 0f : forwardSpeed;

        animator.SetBool(ICharacterControl.Bool_Moving_Hash, !m_inputDirection.Equals(Vector2.zero));
        animator.SetFloat(ICharacterControl.Float_Forward_Hash, forwardSpeed);
        animator.SetFloat(ICharacterControl.Float_Vertical_Hash, verticalSpeed);

        if (m_movementType != ICharacterControl.MovementType.JUMP && m_movementType != ICharacterControl.MovementType.FALL)
        {
            Vector3 deltaMove = animator.deltaPosition;
            deltaMove.y = verticalSpeed * Time.deltaTime;
            characterController.Move(deltaMove);

            m_speedMark[m_speedMarkIndex++] = animator.velocity;
            m_speedMarkIndex %= 3;
        }
        else
        {
            Vector3 averageSpeed = Vector3.zero;
            foreach (var item in m_speedMark)
            {
                averageSpeed += item;
            }

            //��¼�����ٶ� �����λ�� ��ֱ�Ӽ�¼λ������Ϊ����Ϊ֡��������
            Vector3 deltaMove = ((averageSpeed / 6) + m_targetDirection * 2) * Time.deltaTime;
            deltaMove.y = verticalSpeed * Time.deltaTime;
            characterController.Move(deltaMove);
        }
    }

    public void UpdateRotate()
    {
        if (m_inputDirection.Equals(Vector2.zero))
        {
            animator.SetFloat(ICharacterControl.Float_Turn_Hash, 0);
            m_targetDirection = Vector3.zero;
            return;
        }

        m_currentDirection.x = m_inputDirection.x;
        m_currentDirection.z = m_inputDirection.y;

        //���뷽�����������ķ���
        Vector3 target = m_mainCamera.transform.TransformDirection(m_currentDirection);
        //����ƽ��ƽ�е�����
        target = Vector3.ProjectOnPlane(target, Vector3.up).normalized;
        Vector3 roleDelta = rootTransform.InverseTransformDirection(target);
        m_targetDirection = target;
        //����Ŀ��Ƕ��뵱ǰ�Ƕȵļнǻ���
        float rad = Mathf.Atan2(roleDelta.x, roleDelta.z);
        float deg = rad * Mathf.Rad2Deg;

        float rotateSpeed = GetRotateSpeed();
        Quaternion targetRotate = Quaternion.LookRotation(target, Vector3.up);
        //��������ת�������������ת
        rootTransform.rotation = Quaternion.RotateTowards(rootTransform.rotation, targetRotate, rotateSpeed * Time.deltaTime) * animator.deltaRotation;
        animator.SetFloat(ICharacterControl.Float_TargetDir_Hash, deg);
        animator.SetFloat(ICharacterControl.Float_Turn_Hash, rad, 0.2f, Time.deltaTime);
    }

    public void UpdateTargetDirection(Vector2 targetDir)
    {
        m_inputDirection = targetDir;
    }

    protected float UpdateAirDamping()
    {
        return m_holdJumpBtn ? 0f : m_airDamping;
    }

    protected virtual void Jump()
    {
        if (++m_jumpCount >= m_jumpFrequency)
            return;

        if (m_movementType == ICharacterControl.MovementType.JUMP)
            animator.SetTrigger(DoubleJump_Hash);

        m_jumpFlag = true;
        verticalSpeed = Mathf.Sqrt(-2 * m_gravity * m_jumpHeight);
    }

    public void GetInputDirection(InputAction.CallbackContext context)
    {
        UpdateTargetDirection(context.ReadValue<Vector2>());
    }

    public void RequestRun(InputAction.CallbackContext context)
    {
        if (m_moveType == ICharacterControl.MoveType.WALK) return;
        m_moveType = context.phase == InputActionPhase.Performed ? ICharacterControl.MoveType.DASH : ICharacterControl.MoveType.RUN;
    }

    public void RequestWalk(InputAction.CallbackContext context)
    {
        if (m_moveType == ICharacterControl.MoveType.DASH) return;
        if (context.performed)
        {
            m_moveType = m_moveType == ICharacterControl.MoveType.RUN ? ICharacterControl.MoveType.WALK : ICharacterControl.MoveType.RUN;
        }
    }

    public void RequestJump(InputAction.CallbackContext context)
    {

        m_holdJumpBtn = context.phase != InputActionPhase.Canceled;
        //if (m_jumpState != JumpState.NONE || !m_isGround || !m_animator.CurrentlyInAnimationTag("Forward")) return;
        if (context.performed)
        {
            Jump();
        }
    }

}
