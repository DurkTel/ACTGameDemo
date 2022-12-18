using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MoveMotorBase;
using UnityEngine.InputSystem;
using static Demo_MoveMotor.ICharacterControl;
using UnityEditor;


namespace Demo_MoveMotor
{

    public class CharacterMotor : MonoBehaviour, ICharacterControl
    {
        #region �ƶ�����
        [SerializeField, Header("����㼶")]
        protected LayerMask m_groundLayer;

        [SerializeField, Header("�����㼶")]
        protected LayerMask m_climbLayer;

        [SerializeField, Header("ǽ�ܲ㼶")]
        protected LayerMask m_wallRunLayer;

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

        [SerializeField, Header("��������߶�")]
        protected float m_maxClimbHeight = 3f;
        #endregion
        /// <summary>
        /// ���ڵ�
        /// </summary>
        public Transform rootTransform { get; set; }
        /// <summary>
        /// ���ĵ�
        /// </summary>
        public Transform pelvisTransform { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        public Animator animator { get; set; }
        /// <summary>
        /// ǰ���ٶ�
        /// </summary>
        public float forwardSpeed { get; set; }
        /// <summary>
        /// ��ֱ�ٶ�
        /// </summary>
        public float verticalSpeed { get; set; }
        /// <summary>
        /// �Ƿ��ŵ�
        /// </summary>
        public bool isGround { get; set; }
        /// <summary>
        /// �Ƿ��������
        /// </summary>
        public bool isFall { get; set; }
        /// <summary>
        /// ��ɫ������
        /// </summary>
        public CharacterController characterController { get; set; }
        /// <summary>
        /// �ƶ�ģʽ
        /// </summary>
        protected MoveType m_moveType = MoveType.RUN;
        /// <summary>
        /// ��Ϊģʽ
        /// </summary>
        protected MovementType m_movementType = MovementType.IDLE;
        /// <summary>
        /// �����˳����
        /// </summary>
        protected Camera m_mainCamera;
        /// <summary>
        /// Ŀ���ٶ�
        /// </summary>
        protected float m_targetSpeed;
        /// <summary>
        /// ���뷽��
        /// </summary>
        protected Vector2 m_inputDirection;
        /// <summary>
        /// Ŀ�귽��
        /// </summary>
        protected Vector3 m_targetDirection;
        /// <summary>
        /// ��ǰ����
        /// </summary>
        protected Vector3 m_currentDirection;
        /// <summary>
        /// �ٶȼ�¼
        /// </summary>
        protected Vector3[] m_speedMark = new Vector3[3];
        /// <summary>
        /// �ٶȼ�¼�±�
        /// </summary>
        protected int m_speedMarkIndex;
        /// <summary>
        /// ǰ���Ӵ�ǽ�ķ��߷���
        /// </summary>
        protected Vector3 m_wallHitNormal;
        /// <summary>
        /// ǰ��/����Ӵ�ǽ�ı�Ե��
        /// </summary>
        protected Vector3 m_wallHitEdge;
        /// <summary>
        /// ��ż�
        /// </summary>
        private Transform m_leftFootTran;
        /// <summary>
        /// �ҽż�
        /// </summary>
        private Transform m_rightFootTran;
        /// <summary>
        /// �Ƿ�ס��Ծ
        /// </summary>
        private bool m_holdJumpBtn;
        /// <summary>
        /// ��Ծ����
        /// </summary>
        private int m_jumpCount;


        protected virtual void Start()
        {
            m_mainCamera = Camera.main;
            animator = GetComponent<Animator>();
            characterController = GetComponent<CharacterController>();
            rootTransform = characterController.gameObject.transform;
            m_leftFootTran = transform.Find("Model/ClazyRunner/root/pelvis/thigh_l/calf_l/foot_l/ball_l");
            m_rightFootTran = transform.Find("Model/ClazyRunner/root/pelvis/thigh_r/calf_r/foot_r/ball_r");
            pelvisTransform = transform.Find("Model/ClazyRunner/root/pelvis");
        }

        protected virtual void Update()
        {
            CalculateFootStep();
            CalculateGravity();
            CalculateGround();

        }

        protected virtual void OnAnimatorMove()
        {
            UpdateMove();
            UpdateRotate();
        }

        public void CalculateGravity()
        {
            if (!characterController.enabled || m_moveType == MoveType.WALLRUN)
            {
                verticalSpeed = 0f;
                return;
            }
            float damping = UpdateAirDamping();
            verticalSpeed = isGround && verticalSpeed <= 0f ? 0f : verticalSpeed + (damping + m_gravity) * Time.deltaTime;
        }

        public void CalculateFootStep()
        {
            Vector3 localForward = transform.TransformPoint(Vector3.forward);
            float left = Vector3.Dot(localForward, m_leftFootTran.position);
            float right = Vector3.Dot(localForward, m_rightFootTran.position);
            animator.SetInteger(Int_FootStep_Hash, left > right ? -1 : 1);

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
            }
            else
            {
                isGround = false;
            }
            isFall = !Physics.SphereCast(rootTransform.position + Vector3.up * 0.5f, characterController.radius, Vector3.down, out RaycastHit hit, 1f, m_groundLayer);

            animator.SetBool(Bool_Fall_Hash, isFall);
            animator.SetBool(Bool_Ground_Hash, isGround);

            if (!isGround && isFall && m_movementType != MovementType.JUMP && verticalSpeed < 0f)
                UpdateMovementType(MovementType.FALL);
        }

        public bool RequestClimb()
        {
            if (m_movementType == MovementType.CLIMB) return false;

            if (Physics.Raycast(rootTransform.position + Vector3.up + Vector3.up * 0.5f, rootTransform.forward, out RaycastHit obsHit, 1f, m_climbLayer))
            {
                //ǽ��ķ��߷���
                m_wallHitNormal = obsHit.normal;
                Vector3 target = obsHit.point;
                //ǽ�����߶�
                target.y = obsHit.collider.bounds.size.y;
                if (Physics.Raycast(target + Vector3.up, Vector3.down, out RaycastHit wallHit, obsHit.collider.bounds.size.y + 1f, m_climbLayer))
                {
                    m_wallHitEdge = wallHit.point;
                    if (m_wallHitEdge.y <= m_maxClimbHeight)
                        return true;
                }
            }

            return false;
        }

        public bool RequestWallRun()
        {
            if (isGround) return false;
            RaycastHit hit;
            if(Physics.Raycast(rootTransform.position, rootTransform.right, out hit, 1f, m_wallRunLayer) 
                || Physics.Raycast(rootTransform.position, -rootTransform.right, out hit, 1f, m_wallRunLayer))
            {
                m_wallHitNormal = hit.normal;
                m_wallHitEdge = hit.point;
                return true;
            }

            return false;
        }

        protected virtual void Jump()
        {
            if(m_movementType == MovementType.CLIMB)
            {
                animator.SetTrigger(Trigger_ClimbUp_Hash);
                animator.SetInteger(Int_ClimbType_Hash, 0);
                return;
            }
            else if (RequestClimb())
            {
                animator.SetInteger(Int_ClimbType_Hash, 1);
                UpdateMovementType(MovementType.CLIMB);
                return;
            }

            if (++m_jumpCount >= m_jumpFrequency)
                return;

            if (m_movementType == MovementType.JUMP)
                animator.SetTrigger(DoubleJump_Hash);

            UpdateMovementType(MovementType.JUMP);

            verticalSpeed = Mathf.Sqrt(-2 * m_gravity * m_jumpHeight);
        }

        public void UpdateMovementType(MovementType movementType)
        {
            if (m_movementType == movementType) return;

            m_movementType = movementType;
            animator.SetInteger(Int_MovementType_Hash, (int)m_movementType);

        }

        #region �ƶ�����
        public float GetMoveSpeed()
        {
            float moveSpeed = m_rotateSpeed_Run;

            switch (m_moveType)
            {
                case MoveType.WALK:
                    moveSpeed = m_moveSpeed_Walk;
                    break;
                case MoveType.RUN:
                    moveSpeed = m_moveSpeed_Run;
                    break;
                case MoveType.DASH:
                    moveSpeed = m_moveSpeed_Rash;
                    break;
                default:
                    break;
            }

            return moveSpeed;
        }

        public void UpdateMove()
        {
            animator.SetInteger(Int_MoveState_Hash, (int)m_moveType);

            //1.45f��5.85f����ֵ�ɶ���Ƭ�μ���ó�
            m_targetSpeed = GetMoveSpeed();
            m_targetSpeed *= m_inputDirection.magnitude;
            forwardSpeed = Mathf.Lerp(forwardSpeed, m_targetSpeed, 0.1f);
            forwardSpeed = forwardSpeed <= 0.01f ? 0f : forwardSpeed;

            animator.SetBool(Bool_Moving_Hash, !m_inputDirection.Equals(Vector2.zero));
            animator.SetFloat(Float_Forward_Hash, forwardSpeed);
            animator.SetFloat(Float_Vertical_Hash, verticalSpeed);
            
            switch (m_movementType)
            {
                case MovementType.IDLE:
                    UpdateLocomotionMove();
                    break;
                case MovementType.MOVE:
                    UpdateLocomotionMove();
                    break;
                case MovementType.JUMP:
                    UpdateAirMove();
                    break;
                case MovementType.FALL:
                    UpdateAirMove();
                    break;
                case MovementType.CLIMB:
                    UpdateClimbMove();
                    break;
                default:
                    break;
            }
        }

        private void UpdateLocomotionMove()
        {
            if (RequestWallRun())
            {
                m_moveType = MoveType.WALLRUN;
                animator.SetInteger(Int_WallRunType_Hash, 1);
            }
            else
                m_moveType = MoveType.RUN;
            characterController.enabled = true;
            Vector3 deltaMove = animator.deltaPosition;
            deltaMove.y = m_moveType == MoveType.WALLRUN? deltaMove.y : verticalSpeed * Time.deltaTime;
            characterController.Move(deltaMove);
            
            m_speedMark[m_speedMarkIndex++] = animator.velocity;
            m_speedMarkIndex %= 3;
            
            if (m_moveType == MoveType.WALLRUN && animator.CurrentlyInAnimationTag("WallRunMatchCatch"))
            {
                animator.MatchTarget(m_wallHitEdge + m_wallHitNormal.normalized * 0.8f, Quaternion.identity, AvatarTarget.Root, new MatchTargetWeightMask(Vector3.one, 0f), 0f, 0.1f);
            }

        }

        private void UpdateAirMove()
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

        private void UpdateClimbMove()
        {
            characterController.enabled = false;
            animator.ApplyBuiltinRootMotion();

            if (animator.CurrentlyInAnimationTag("ClimbMatchCatch"))
            {
                animator.MatchTarget(m_wallHitEdge + new Vector3(0, -0.06f, 0), Quaternion.identity, AvatarTarget.RightHand, new MatchTargetWeightMask(Vector3.one, 0f), 0f, 0.5f);
            }


        }

        #endregion

        #region ��ת����
        public float GetRotateSpeed()
        {
            float rotateSpeed = m_rotateSpeed_Run;
            if (m_movementType == MovementType.JUMP || m_movementType == MovementType.FALL)
                return m_rotateSpeed_Air;
            else if (animator.CurrentlyInAnimationTag("SharpTurn"))
                return m_rotateSpeed_Sharp;

            switch (m_moveType)
            {
                case MoveType.WALK:
                    rotateSpeed = m_rotateSpeed_Walk;
                    break;
                case MoveType.RUN:
                    rotateSpeed = m_rotateSpeed_Run;
                    break;
                case MoveType.DASH:
                    rotateSpeed = 0;
                    break;
                default:
                    break;
            }


            return rotateSpeed;
        }

        public void UpdateRotate()
        {
            m_currentDirection.x = m_inputDirection.x;
            m_currentDirection.z = m_inputDirection.y;

            switch (m_movementType)
            {
                case MovementType.CLIMB:
                    UpdateClimbRotate();
                    break;
                default:
                    UpdateLocomotionRotate();
                    break;
            }
        }

        private void UpdateLocomotionRotate()
        {
            if (m_moveType == MoveType.WALLRUN)
            {
                UpdateWallRunRotate();
                return;
            }
            //���뷽�����������ķ���
            Vector3 target = m_mainCamera.transform.TransformDirection(m_currentDirection);
            //����ƽ��ƽ�е�����
            target = Vector3.ProjectOnPlane(target, Vector3.up).normalized;
            Vector3 roleDelta = rootTransform.InverseTransformDirection(target);
            m_targetDirection = target;
            //����Ŀ��Ƕ��뵱ǰ�Ƕȵļнǻ���
            float rad = Mathf.Atan2(roleDelta.x, roleDelta.z);
            float deg = rad * Mathf.Rad2Deg;
            animator.SetFloat(Float_TargetDir_Hash, deg);
            animator.SetFloat(Float_Turn_Hash, rad, 0.2f, Time.deltaTime);

            if (target.Equals(Vector3.zero))
                return;

            float rotateSpeed = GetRotateSpeed();
            Quaternion targetRotate = Quaternion.LookRotation(target, Vector3.up);
            //��������ת�������������ת
            rootTransform.rotation = Quaternion.RotateTowards(rootTransform.rotation, targetRotate, rotateSpeed * Time.deltaTime) * animator.deltaRotation;
        }

        private void UpdateClimbRotate()
        {
            if (animator.CurrentlyInAnimationTag("ClimbMatchCatch"))
            {
                
                Quaternion targetRotate = Quaternion.LookRotation(-m_wallHitNormal);
                rootTransform.rotation = Quaternion.RotateTowards(rootTransform.rotation, targetRotate, 500f * Time.deltaTime);
            }
        }

        private void UpdateWallRunRotate()
        {
            if (animator.CurrentlyInAnimationTag("WallRunMatchCatch"))
            {
                Quaternion targetRotate = Quaternion.LookRotation(Vector3.Cross(-m_wallHitNormal, rootTransform.up));
                rootTransform.rotation = Quaternion.RotateTowards(rootTransform.rotation, targetRotate, 500f * Time.deltaTime);
            }
        }

        #endregion

        #region ����״̬����
        public void OnAnimationStateEnter(AnimatorStateInfo stateInfo)
        {
            if (stateInfo.IsTag("Idle"))
            {
                UpdateMovementType(MovementType.IDLE);
            }
            else if(stateInfo.IsTag("Forward"))
            {
                UpdateMovementType(MovementType.MOVE);
            }
        }

        public void OnAnimationStateExit(AnimatorStateInfo stateInfo)
        {
            if (Animator.StringToHash("Wall_Climb_Exit_Root") == stateInfo.shortNameHash)
            {
                characterController.enabled = true;
            }
        }

        public void OnAnimationStateMove(AnimatorStateInfo stateInfo)
        {
            if (Animator.StringToHash("Wall_Climb_Exit_Root") == stateInfo.shortNameHash)
            {
                rootTransform.localPosition += Vector3.down * 0.002f;
            }
        }

        #endregion

        #region �û�����
        public void UpdateTargetDirection(Vector2 targetDir)
        {
            m_inputDirection = targetDir;
        }

        protected float UpdateAirDamping()
        {
            return m_holdJumpBtn ? 0f : m_airDamping;
        }

        public void GetInputDirection(InputAction.CallbackContext context)
        {
            UpdateTargetDirection(context.ReadValue<Vector2>());
        }

        public void RequestRun(InputAction.CallbackContext context)
        {
            if (m_moveType == MoveType.WALK) return;
            m_moveType = context.phase == InputActionPhase.Performed ? MoveType.DASH : MoveType.RUN;
        }

        public void RequestWalk(InputAction.CallbackContext context)
        {
            if (m_moveType == MoveType.DASH) return;
            if (context.performed)
            {
                m_moveType = m_moveType == MoveType.RUN ? MoveType.WALK : MoveType.RUN;
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
        #endregion
    }
}
