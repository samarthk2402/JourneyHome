using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    //Patrolling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //States 
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake(){
        player = GameObject.Find("Player").transform;
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

        Debug.Log("Patroling");
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
        agent.SetDestination(player.position);
        Debug.Log("Chasing");
    }

    private void Attacking(){
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if(!alreadyAttacked){
            //Attack

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }

        Debug.Log("Attacking");
    }

    private void ResetAttack(){

    }

}
