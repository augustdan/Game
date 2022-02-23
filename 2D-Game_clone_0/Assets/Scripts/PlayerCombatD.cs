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
    public string tag;
    [Header("Combo punch")]
    public int combo;
    public bool attackDo;
    
    public int maxHealth = 100;
    
    [SyncVar (hook = nameof(HealthValue))] public int currentHealth;
    public override void OnStartServer()
    {
        base.OnStartServer();
        currentHealth = maxHealth;
    }
    private void Start()
    {
        if (!isLocalPlayer)
            return;
        tag = gameObject.tag;
        
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        if (!isLocalPlayer)
            return;
            CmdCombo();
    }
    [Command]
    public void CmdCombo()
    {
        if (Input.GetKeyDown(KeyCode.F) && !attackDo)
        {
            attackDo = true;
            anim.SetTrigger("" + combo);
            CmdAttack();
            Debug.Log("Attack");
        }
    }
    [Command]
    private void CmdAttack()
    {

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);
        foreach (Collider2D collider in hitEnemies)
        {

            if (gameObject.tag == "Team1" && collider.tag == "Team2")
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
    
    public void Start_Combo()
    {
        attackDo = false;
        if (combo < 2)
        {
            combo++;
        }
    }
    public void Finish_Animation()
    {
        attackDo = false;
        combo = 0;
    }
    [Server]
    public void TakeDamage(int damage)
    {
        if (!isServer)
            return;
        currentHealth -= damage;

        anim.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    [Server]
    void Die()
    {
        Debug.Log("Enemy Died!");

        //Destroy(this.gameObject, 1.5f);
        NetworkServer.Destroy(gameObject);
    }
    void HealthValue(int oldValue, int newValue)
    {
        maxHealth = newValue;
    }
    [Server]
    public void ChangeHealthValue(int newValue)
    {
        currentHealth = newValue;
    }
    public void CmdChangeHealth(int newValue)
    {
        ChangeHealthValue(newValue);
    }
}
