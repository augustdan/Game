using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerCombatD : NetworkBehaviour
{
    public Animator anim;

    public Transform attackPoint;
    public LayerMask enemyLayer;

    public float attackRange = 0.5f;
    public int attackDamage = 40;

    [Header("Combo punch")]
    public int combo;
    public bool attackDo;

    public int maxHealth = 100;
    public int currentHealth = 100;
    private void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        if (!isLocalPlayer)
            return;
        Combo();
       //if (Input.GetKeyDown(KeyCode.F))
       // {
       //     Attack();
       // }
    }
    private void Attack()
    {

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);
        foreach(GameObject collider in hitEnemies)
        {
            
            //collider.GetComponent<PlayerCombatD>().TakeDamage(attackDamage);
            if(collider.tag == "Team2")
            {
                collider.GetComponent<PlayerCombatD>().TakeDamage(attackDamage);
                Debug.Log("We hit enemy");
                
            }

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
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        anim.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        Debug.Log("Enemy Died!");

        //anim.SetBool("IsDead", true);

        //GetComponent<Collider2D>().enabled = false;

        //this.enabled = false;
        Destroy(this.gameObject, 1.5f);
    }
}
