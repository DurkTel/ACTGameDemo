using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerActions
{
    /// <summary>
    /// �ƶ�
    /// </summary>
    public Vector3 move = Vector3.zero;
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

    public void ResetActions()
    {
        sprint = false;
        walk = false;
        jump = false;
        escape = false;
    }
}
