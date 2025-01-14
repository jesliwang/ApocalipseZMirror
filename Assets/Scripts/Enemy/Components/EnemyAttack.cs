using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{

    public bool IsAttacking;
    public float timeBetweenAttacks;
    public float distanceAttack;
    public Transform target;

    public void Attack()
    {
        if (target)
        {
            if (Vector3.Distance(transform.position, target.position) > distanceAttack)
            {
                IsAttacking = false;
                return;
            }
            if (!IsAttacking)
            {
                StartCoroutine("ResetAtack");
                IsAttacking = true;
            }
        }
    }
    void Update()
    {
        Attack();
    }
    IEnumerator ResetAtack()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        DamageTarget();
        yield return new WaitForSecondsRealtime(timeBetweenAttacks);
        IsAttacking = false;
        yield break;
    }
    public void FightHit()
    {

    }
    public void DamageTarget()
    {
        if (target == null)
        {
            return;
        }
        float distanceFromTarget = Vector3.Distance(transform.position, target.position);
        if (distanceFromTarget <= distanceAttack)
        {
            target.GetComponent<IStats>().TakeDamage((int)GetComponent<IStats>().GetDamage());

        }
    }
}
