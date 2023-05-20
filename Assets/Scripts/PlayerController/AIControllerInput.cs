using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControllerInput : MonoBehaviour
{
    public LayerMask enemyLayer;

    public float searchEnemyRadius;

    public PlayerController playerController;

    private Transform m_enemyTrans;

    private bool m_isPatrol;

    private Vector3 m_patrolPoint;


    private void Start()
    {
        playerController = GetComponent<PlayerController>();    
    }

    private void Update()
    {
        OnPatrol();
    }

    private void FixedUpdate()
    {
        SearchForEnemy();
    }


    private void SearchForEnemy()
    {
        m_enemyTrans = transform.ObtainNearestTarget(searchEnemyRadius, enemyLayer, playerController.rootTransform);
    }

    private void OnPatrol()
    {
        if (m_enemyTrans != null) return;
        if (!m_isPatrol)
        {
            TimerManager.Instance.AddTimer(GetNewPatrolPoint, 0, 1, 2);
            //GetNewPatrolPoint();
        }

        playerController.actions.move = m_patrolPoint;
        if (Vector3.Distance(playerController.rootTransform.position, m_patrolPoint) <= 1f)
        {
            m_patrolPoint = Vector3.zero;
            m_isPatrol = false;
        }

    }

    private void GetNewPatrolPoint()
    {
        print("Ë¢ÐÂÑ²Âßµã");
        Random.InitState((int)Time.realtimeSinceStartup);
        float temp = 1.5f;
        m_patrolPoint = new Vector3(Random.Range(temp, -temp), 0, Random.Range(temp, -temp)) + playerController.rootTransform.position;
        float distance = Vector3.Distance(playerController.rootTransform.position, m_patrolPoint);
        //playerController.moveController.Move(m_patrolPoint, distance / 0.5f, 0f);
        //playerController.moveController.Rotate(Quaternion.LookRotation(m_patrolPoint), 0.15f, 0f);
        m_isPatrol = true;
    }
}
