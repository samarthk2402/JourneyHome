using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject player;
    public PlayerTarget playerTarget;
    public LayerMask whatIsGround, whatIsPlayer;

    //Patrolling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    public float damage = 10;
    bool alreadyAttacked;

    //States 
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    public ParticleSystem ps;

    private void Awake(){
        player = GameObject.Find("Player");
        playerTarget = player.GetComponent<PlayerTarget>();
        agent = GetComponent<NavMeshAgent>();

    }

    private void Update(){
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer); 
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer); 

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
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

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

        if(!alreadyAttacked){
            //Attack
            ps.Play();

            if(Physics.Raycast(transform.position, player.transform.position, out RaycastHit hit, attackRange)){
                playerTarget.TakeDamage(damage);
                alreadyAttacked = true;
                StartCoroutine(resetAttack());
            }
        }
    }

    private IEnumerator resetAttack(){
        yield return new WaitForSeconds(timeBetweenAttacks);
        alreadyAttacked = false;
    }

}
