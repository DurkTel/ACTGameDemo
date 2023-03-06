using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Demo_MoveMotor.ICharacterControl;
using UnityEditor;
using UnityEngine.InputSystem.XR;
using static UnityEditor.Experimental.GraphView.GraphView;
using System.Net;


namespace Demo_MoveMotor
{

    public partial class CharacterMotor : MonoBehaviour, ICharacterControl
    {
        protected OrbitCamera m_camera;

        #region ��������
        protected DebugHelper m_debugHelper;
        #endregion

        #region ���㼶
        [SerializeField, Header("����㼶")]
        protected LayerMask m_groundLayer;

        [SerializeField, Header("�����㼶")]
        protected LayerMask m_climbLayer;

        [SerializeField, Header("ǽ�ܲ㼶")]
        protected LayerMask m_wallRunLayer;

        [SerializeField, Header("��Խ�㼶")]
        protected LayerMask m_vaultLayer;

        [SerializeField, Header("�������㼶")]
        protected LayerMask m_lockonLayer;
        #endregion

        #region �ƶ�����

        [Space]

        [SerializeField, Header("������Χ")]
        protected float m_lockonRadius = 10f;

        [SerializeField, Header("�ƶ��ٶ�")]
        protected float m_moveSpeed = 1f;

        [SerializeField, Header("�ƶ��ٶ�ϵ��")]
        protected float m_moveMultiplier = 1f;

        [SerializeField, Header("��ת�ٶ�")]
        protected float m_rotateSpeed = 10f;

        [SerializeField, Header("��Ծ�߶�")]
        protected float m_jumpHeight = 2f;

        [SerializeField, Header("��Ծ����")]
        protected int m_jumpFrequency = 2;

        [SerializeField, Header("�������ٶ�")]
        protected float m_gravity = -9.8f;

        [SerializeField, Header("��������")]
        protected float m_airDamping = 2f;
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
        /// ������
        /// </summary>
        public Animator animator { get; set; }
        /// <summary>
        /// ����״̬
        /// </summary>
        protected AnimatorStateInfo m_baseLayerInfo, m_fullBodyLayerInfo;
        /// <summary>
        /// ����״̬
        /// </summary>
        protected XAnimationStateInfos m_stateInfos;
        /// <summary>
        /// �ƶ�ģʽ
        /// </summary>
        protected MoveType m_moveType = MoveType.RUN;
        /// <summary>
        /// ��Ϊģʽ
        /// </summary>
        protected MovementType m_movementType = MovementType.IDLE;
        /// <summary>
        /// Ŀ��Ƕ��뵱ǰ�Ƕȵļн�
        /// </summary>
        protected float m_targetDeg;
        /// <summary>
        /// �Ƿ�ס��Ծ
        /// </summary>
        protected bool m_holdJumpBtn;
        /// <summary>
        /// ��Ծ����
        /// </summary>
        protected int m_jumpCount;
        /// <summary>
        /// ��һ֡��ǰ����
        /// </summary>
        protected Vector3 m_lastForward;
        /// <summary>
        /// Ŀ�귽��
        /// </summary>
        protected Vector3 m_targetDirection;
        /// <summary>
        /// ������ǰ�ߵ�����
        /// </summary>
        protected float m_relativityForward;
        /// <summary>
        /// �������ұߵ�����
        /// </summary>
        protected float m_relativityRight;
        /// <summary>
        /// ǽ�ܵķ���
        /// </summary>
        protected float m_wallRunDir; 
        /// <summary>
        /// Ŀ��λ��
        /// </summary>
        protected int m_targetPositionIndex;
        /// <summary>
        /// �Ƿ�������״̬
        /// </summary>
        protected bool m_isWalk;
        /// <summary>
        /// �Ƿ��ڳ��״̬
        /// </summary>
        protected bool m_isSprint;
        /// <summary>
        /// �Ƿ�����״̬
        /// </summary>
        protected bool m_isGazing;
        /// <summary>
        /// �Ƿ��ڿ���
        /// </summary>
        protected bool m_isAirbone;
        /// <summary>
        /// �Ƿ�������׹
        /// </summary>
        protected bool m_isFalling;
        /// <summary>
        /// �Ƿ���������
        /// </summary>
        protected bool m_isEscape;
        /// <summary>
        /// �Ƿ����ڷ�Խ
        /// </summary>
        protected bool m_isVault;
        /// <summary>
        /// �Ƿ���������
        /// </summary>
        protected bool m_isClimbing;
        /// <summary>
        /// �Ƿ����ڵ�ǽ��
        /// </summary>
        protected bool m_isWallRunning;

        #region �����˶�
        /// <summary>
        /// �����˶�Ŀ��λ��
        /// </summary>
        private Vector3 m_curveMoveTarget;
        /// <summary>
        /// �����˶���
        /// </summary>
        private bool m_isCurveMoving;
        /// <summary>
        /// �����˶���ʼʱ��
        /// </summary>
        private float m_curveMoveBeginTime;
        /// <summary>
        /// �����˶����
        /// </summary>
        private float m_curveMoveDelta;

        /// <summary>
        /// ������תĿ��λ��
        /// </summary>
        private Quaternion m_curveRotationTarget;
        /// <summary>
        /// ������ת��
        /// </summary>
        private bool m_isCurveRotating;
        /// <summary>
        /// ������ת��ʼʱ��
        /// </summary>
        private float m_curveRotationBeginTime;
        /// <summary>
        /// ������ת���
        /// </summary>
        private float m_curveRotationDelta;
        #endregion

        protected virtual void Awake()
        {
            m_debugHelper = GetComponent<DebugHelper>();
            m_camera = Camera.main.GetComponent<OrbitCamera>();
        }

        protected virtual void Start()
        {
            animator = GetComponent<Animator>();
            characterController = GetComponent<CharacterController>();
            rootTransform = characterController.gameObject.transform;
            pelvisTransform = transform.Find("Model/ClazyRunner/root/pelvis");
        }

        protected virtual void Update()
        {
            
        }

        protected virtual void FixedUpdate()
        {
            
        }

        protected virtual void OnAnimatorMove()
        {
            
        }

        protected virtual void OnEnable()
        {

        }

        protected virtual void OnDisable()
        {

        }

        protected virtual void PlayAnimation(string name, float duration) { }
        protected virtual void PlayMachine(int type) { }

        #region ƽ���ƶ�
        /// <summary>
        /// ��ת��Ŀ�귽��
        /// </summary>
        /// <param name="direction">Ŀ�귽��</param>
        /// <param name="rotationSpeed">��ת�ٶ�</param>
        public virtual void RotateToDirection(Vector3 direction, float rotationSpeed)
        {
            if (m_isClimbing) return;
            direction.y = 0f;
            if (direction.normalized.magnitude == 0)
                direction = rootTransform.forward;

            var euler = rootTransform.rotation.eulerAngles.NormalizeAngle();
            var targetEuler = Quaternion.LookRotation(direction.normalized).eulerAngles.NormalizeAngle();
            euler.y = Mathf.LerpAngle(euler.y, targetEuler.y, rotationSpeed * Time.deltaTime);
            Quaternion newRotation = Quaternion.Euler(euler);
            rootTransform.rotation = newRotation;
            //rootTransform.rotation = Quaternion.RotateTowards(rootTransform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.fixedDeltaTime);

            //m_debugHelper.DrawLine(rootTransform.position, rootTransform.position + direction, Color.green); //������ת
            //m_debugHelper.DrawLine(rootTransform.position, rootTransform.position + rootTransform.forward, Color.red, 0.05f); //��ǰ��ת

        }

        public virtual void RotateToDirection(Vector3 direction)
        {
            direction = m_isGazing ? m_camera.transform.forward : direction;

            RotateToDirection(direction, m_rotateSpeed);
        }

        /// <summary>
        /// ʹ�ø���ת
        /// </summary>
        public virtual void RotateByRootMotor()
        {
            rootTransform.rotation *= animator.deltaRotation;
        }

        /// <summary>
        /// �ƶ���ĳ������ ʹ��CC
        /// </summary>
        /// <param name="direction"></param>
        public virtual void MoveToDirection(Vector3 direction)
        {
            if (!characterController.enabled) return;
            direction.y = 0f;
            direction = direction.normalized * Mathf.Clamp(direction.magnitude, 0, 1f);
            //��һ֡���ƶ�λ��
            Vector3 targetPosition = rootTransform.position + direction * m_moveSpeed * m_moveMultiplier * (float)m_moveType / 2f * Time.deltaTime;
            //��һ֡���ƶ��ٶ�
            Vector3 targetVelocity = (targetPosition - rootTransform.position) / Time.deltaTime;
            targetVelocity.y = verticalSpeed * Time.deltaTime;
            
            characterController.Move(targetVelocity);
            
        }

        /// <summary>
        /// ʹ�ø��ƶ�
        /// </summary>
        public virtual void MoveByMotor()
        {
            characterController.Move(animator.deltaPosition);
        }

        public void DOCurveMove(Vector3 target, float time, float delay = 0f)
        {
            m_isCurveMoving = true;
            characterController.enabled = false;
            m_curveMoveTarget = target;
            m_curveMoveBeginTime = Time.realtimeSinceStartup + delay;
            m_curveMoveDelta = Vector3.Distance(rootTransform.position, target) / time;
        }

        /// <summary>
        /// �����ƶ� ֱ���޸�λ��
        /// </summary>
        public virtual void MoveByCurve()
        {
            rootTransform.position = Vector3.MoveTowards(rootTransform.position, m_curveMoveTarget, m_curveMoveDelta * Time.fixedDeltaTime);
        }


        public bool IsEnableCurveMotion()
        {
            if (!m_isCurveMoving || Time.realtimeSinceStartup < m_curveMoveBeginTime)
                return false;

            if (Vector3.Distance(m_curveMoveTarget, rootTransform.position) <= 0.01f)
            {
                m_isCurveMoving = false;
                characterController.enabled = true;
                return false;
            }

            return true;
        }

        public void DOCurveRotate(Quaternion target, float time, float delay = 0f)
        {
            m_isCurveRotating = true;
            m_curveRotationTarget = target;
            m_curveRotationBeginTime = Time.realtimeSinceStartup + delay;
            m_curveRotationDelta = Quaternion.Angle(target, rootTransform.rotation) / time;
        }

        /// <summary>
        /// �����ƶ� ֱ���޸�λ��
        /// </summary>
        public virtual void RotateByCurve()
        {
            rootTransform.rotation = Quaternion.Lerp(rootTransform.rotation, m_curveRotationTarget, m_curveRotationDelta * Time.fixedDeltaTime);
        }


        public bool IsEnableCurveRotate()
        {
            if (!m_isCurveRotating || Time.realtimeSinceStartup < m_curveRotationBeginTime)
                return false;
            
            if (Quaternion.Angle(m_curveRotationTarget, rootTransform.rotation) <= 0.01f)
            {
                m_isCurveRotating = false;
                return false;
            }

            return true;
        }

        #endregion


        #region ��Ϊ
        protected virtual void Escape()
        {
            PlayMachine(1);
        }

        protected virtual void Jump()
        {
            verticalSpeed = Mathf.Sqrt(-2 * m_gravity * m_jumpHeight);
            PlayMachine(2);
        }

        protected virtual void Fall()
        {
            PlayMachine(3);
        }

        protected virtual void Vault()
        {
            PlayAnimation("Vault", 0f);
        }

        protected virtual void ShortClimb()
        {
            PlayAnimation("Short Climb", 0f);
        }

        protected virtual void HeightClimb()
        {
            PlayAnimation("Height Climb Up", 0.05f);
        }

        #endregion

    }
}
