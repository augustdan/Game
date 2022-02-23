using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CombatController : NetworkBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    [SyncVar] public int currentHealth;

    [Header("Tag")]
    public string tag;
    [SyncVar] public string currentTag;

    [Header("AttackLogic")]
    public Animator anim;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public int attackDamage = 40;

    public override void OnStartServer()
    {
        base.OnStartServer();
        currentHealth = maxHealth;
        tag = gameObject.tag;
    }
    private void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Attack");
            CmdAttack();
            RpcAttackAnimation();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            ChangeTagTeam1(true);
            Debug.Log("Tag has changed to team 2!");
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            ChangeTagTeam1(false);
            Debug.Log("Tag has changed to team 1!");
        }
    }
    [Command]
    private void ChangeTagTeam1(bool teams)
    {
        if (teams == true)
        {
            gameObject.tag = "Team2";
            RpcChangeTagTeam1(true);
        }
        else
        {
            gameObject.tag = "Team1";
            RpcChangeTagTeam1(false);
        }
    }
    [ClientRpc]
    private void RpcChangeTagTeam1(bool teams)
    {
        if (teams == true)
        {
            gameObject.tag = "Team2";
        }
        else
        {
            gameObject.tag = "Team1";
        }
    }
    [Command]
    private void CmdAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);
        foreach (Collider2D collider in hitEnemies)
        {

            if (collider.tag == "Team1" && gameObject.tag == "Team2")
            {
                NetworkIdentity opponentIdentity = collider.GetComponent<NetworkIdentity>();
                collider.GetComponent<CombatController>().ServerTargetToDamage(opponentIdentity, attackDamage);
                Debug.Log("We hit enemy");
            }
            if (collider.tag == "Team2" && gameObject.tag == "Team1")
            {
                NetworkIdentity opponentIdentity = collider.GetComponent<NetworkIdentity>();
                collider.GetComponent<CombatController>().TargetTakeDamage(opponentIdentity.connectionToClient, attackDamage);
                Debug.Log("We hit enemy");
            }
        }
    }
    [Server]
    private void ServerTargetToDamage(NetworkIdentity opponentIdentity, int damage)
    {
        TargetTakeDamage(opponentIdentity.connectionToClient, attackDamage);
    }
    [TargetRpc]
    private void TargetTakeDamage(NetworkConnection target, int damage)
    {
        //if (!isServer)
        //   return;
        currentHealth -= damage;
        Debug.Log("Damage has been assigned!");
        anim.SetTrigger("Hurt");

    }
    private void RpcAttackAnimation()
    {
        anim.SetTrigger("Attack");
    }
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
