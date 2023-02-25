using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Demo_MoveMotor.ICharacterControl;
using UnityEditor;
using UnityEngine.InputSystem.XR;
using static UnityEditor.Experimental.GraphView.GraphView;


namespace Demo_MoveMotor
{

    public partial class CharacterMotor : MonoBehaviour, ICharacterControl
    {
        protected DebugHelper m_debugHelper;


        #region �ƶ�����
        [SerializeField, Header("������")]
        protected OrbitCamera m_camera;
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

        protected XAnimationStateInfos m_stateInfos;
        /// <summary>
        /// ���뷽��
        /// </summary>
        protected Vector2 m_input;
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

        protected virtual void Awake()
        {
            m_debugHelper = GetComponent<DebugHelper>();
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
            direction.y = 0f;
            if (direction.normalized.magnitude == 0)
                direction = rootTransform.forward;

            var euler = rootTransform.rotation.eulerAngles.NormalizeAngle();
            var targetEuler = Quaternion.LookRotation(direction.normalized).eulerAngles.NormalizeAngle();
            euler.y = Mathf.LerpAngle(euler.y, targetEuler.y, rotationSpeed * Time.deltaTime);
            Quaternion newRotation = Quaternion.Euler(euler);
            rootTransform.rotation = newRotation;
            //rootTransform.rotation = Quaternion.RotateTowards(rootTransform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.fixedDeltaTime);

            m_debugHelper.DrawLine(rootTransform.position, rootTransform.position + direction, Color.green); //������ת
            m_debugHelper.DrawLine(rootTransform.position, rootTransform.position + rootTransform.forward, Color.red, 0.05f); //��ǰ��ת

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

        /// <summary>
        /// �����ƶ� ֱ���޸�λ��
        /// </summary>
        public virtual void CurveMove()
        {
            
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

        #endregion

    }
}
