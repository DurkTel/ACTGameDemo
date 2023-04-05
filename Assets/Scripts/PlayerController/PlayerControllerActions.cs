using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerActions
{
    /// <summary>
    /// ���λ��
    /// </summary>
    public Transform cameraTransform;
    /// <summary>
    /// �ƶ�
    /// </summary>
    public Vector3 move = Vector3.zero;
    /// <summary>
    /// �ƶ� ǰ
    /// </summary>
    public bool moveUp = false;
    /// <summary>
    /// �ƶ� ��
    /// </summary>
    public bool moveDown = false;
    /// <summary>
    /// �ƶ� ��
    /// </summary>
    public bool moveLeft = false;
    /// <summary>
    /// �ƶ� ��
    /// </summary>
    public bool moveRight = false;
    /// <summary>
    /// ����
    /// </summary>
    public bool gazing = false;
    /// <summary>
    /// ���
    /// </summary>
    public bool sprint = false;
    /// <summary>
    /// ����
    /// </summary>
    public bool walk = false;
    /// <summary>
    /// ��Ծ
    /// </summary>
    public bool jump = false;
    /// <summary>
    /// ����
    /// </summary>
    public bool escape = false;
    /// <summary>
    /// ������
    /// </summary>
    public bool weapon = false;
    /// <summary>
    /// �ṥ��
    /// </summary>
    public bool lightAttack = false;
    /// <summary>
    /// �ع���
    /// </summary>
    public bool heavyAttack = false;
    /// <summary>
    /// ������չ
    /// </summary>
    public bool attackEx = false;
    /// <summary>
    /// ���� 0 δ����  1ǰſ  -1��ſ
    /// </summary>
    public int knockDown = 0;
    /// <summary>
    /// ���� ����
    /// </summary>
    public bool knockUp = false;
    /// <summary>
    /// �ܻ�ս��
    /// </summary>
    public CombatBroadcast hurtBroadcast = null;
    public void ResetActions()
    {
        sprint = false;
        walk = false;
        jump = false;
        escape = false;
    }
}
