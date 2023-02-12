using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public EnemyType enemy;
    public NavMeshAgent agent;
    public GameObject player;
    public PlayerTarget playerTarget;
    private Player playerMove;
    public LayerMask whatIsGround, whatIsPlayer;

    //Patrolling
    public Vector3 walkPoint;
    bool walkPointSet;

    //Attacking
    bool alreadyAttacked;

    //States 
    public bool playerInSightRange, playerInAttackRange;

    private void Awake(){
        player = GameObject.Find("Player");
        playerTarget = player.GetComponent<PlayerTarget>();
        playerMove = player.GetComponent<Player>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = enemy.speed;
    }

    private void Update(){
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, enemy.sightRange, whatIsPlayer); 
        playerInAttackRange = Physics.CheckSphere(transform.position, enemy.attackRange, whatIsPlayer); 

        if(!playerInSightRange && !playerInAttackRange) Patroling();
        if(playerInSightRange && !playerInAttackRange) Chasing();
        if(playerInAttackRange) Attacking();
    }

    private void Patroling(){
        if(!walkPointSet) SearchWalkPoint();

        if(walkPointSet){
            agent.SetDestination(walkPoint);
        }

        Vector3 distToWalkPoint = transform.position - walkPoint;
        
        //Walkpoint Reached
        if(distToWalkPoint.magnitude < 1){
            walkPointSet = false;
        }
    }

    private void SearchWalkPoint(){
        //Calculate random point in range
        float randomZ = Random.Range(-enemy.walkPointRange, enemy.walkPointRange);
        float randomX = Random.Range(-enemy.walkPointRange, enemy.walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if(Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround)){
            walkPointSet = true;
        }
    }

    private void Chasing(){
        agent.SetDestination(player.transform.position);
    }

    private void Attacking(){
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);
        var lookPos = player.transform.position;
        lookPos.y = transform.position.y;
        transform.LookAt(lookPos);

        if(!alreadyAttacked && playerMove.controller.isGrounded){

            //Attack
            enemy.ps.Play();

            if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, enemy.attackRange)){
                playerTarget.TakeDamage(enemy.damage);
                alreadyAttacked = true;
            }

            StartCoroutine(resetAttack());
        }
    }

    private IEnumerator resetAttack(){
        yield return new WaitForSeconds(enemy.timeBetweenAttacks);
        alreadyAttacked = false;
    }

}
