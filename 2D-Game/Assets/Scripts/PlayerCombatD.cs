using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Animator anim;

    public Transform attackPoint;
    public LayerMask enemyLayer;

    public float attackRange = 0.5f;
    public int attackDamage = 40;

    [Header("Combo punch")]
    public int combo;
    public bool attackDo;
    
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        Combo();
       //if (Input.GetKeyDown(KeyCode.F))
       // {
       //     Attack();
       // }
    }
    private void Attack()
    {

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        foreach(Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    public void Combo()
    {
        if (Input.GetKeyDown(KeyCode.F) && !attackDo)
        {
            attackDo = true;
            anim.SetTrigger("" + combo);
            Attack();
        }
    }
    public void Start_Combo()
    {
        attackDo = false;
        if( combo < 2)
        {
            combo++;
        }
    }
    public void Finish_Animation()
    {
        attackDo = false;
        combo = 0;
    }
}
