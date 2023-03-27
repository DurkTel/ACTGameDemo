using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerActions
{
    /// <summary>
    /// ÒÆ¶¯
    /// </summary>
    public Vector3 move = Vector3.zero;
    /// <summary>
    /// Ëø¶¨
    /// </summary>
    public bool gazing = false;
    /// <summary>
    /// ³å´Ì
    /// </summary>
    public bool sprint = false;
    /// <summary>
    /// ÐÐ×ß
    /// </summary>
    public bool walk = false;
    /// <summary>
    /// ÌøÔ¾
    /// </summary>
    public bool jump = false;
    /// <summary>
    /// ÉÁ±Ü
    /// </summary>
    public bool escape = false;
    /// <summary>
    /// ³ÖÎäÆ÷
    /// </summary>
    public bool weapon = false;
    /// <summary>
    /// Çá¹¥»÷
    /// </summary>
    public bool lightAttack = false;
    /// <summary>
    /// ÖØ¹¥»÷
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
