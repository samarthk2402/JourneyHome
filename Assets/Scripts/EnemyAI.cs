using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float walkPointRange;
    public float speed;
    public float timeBetweenAttacks;
    public float damage = 10;
    public float sightRange, attackRange;
    public ParticleSystem ps;
    public float psTime;
    public LineRenderer lineRenderer;
    public bool damageOverTime;
    public int hit_num;
    public NavMeshAgent agent;
    public GameObject player;
    public PlayerTarget playerTarget;
    private Player playerMove;
    public LayerMask whatIsGround, whatIsPlayer;

    public bool isShot = false;

    //Patrolling
    public Vector3 walkPoint;
    bool walkPointSet;

    //Attacking
    bool alreadyAttacked;

    //States 
    public bool playerInSightRange, playerInAttackRange;

    private Animator animator;
    [SerializeField] List<Transform> effectTransforms = new List<Transform>();

    private void Awake(){
        player = GameObject.Find("Player");
        playerTarget = player.GetComponent<PlayerTarget>();
        playerMove = player.GetComponent<Player>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        animator = GetComponentInChildren<Animator>();
    }

    private void Update(){
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer); 
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer); 

        if(!playerInSightRange && !playerInAttackRange && !isShot) Patroling();
        if((playerInSightRange && !playerInAttackRange) || isShot) Chasing();
        if(playerInAttackRange) Attacking();
    }

    private void Patroling(){
        //rig.weight = 0;
        animator.SetBool("isMoving", true);
        if(!walkPointSet) SearchWalkPoint();

        if(walkPointSet){
            agent.SetDestination(walkPoint);
        }

        Vector3 distToWalkPoint = transform.position - walkPoint;
        
        //Walkpoint Reached
        if(distToWalkPoint.magnitude < 1){
            walkPointSet = false;
        }

        //Obstacle found
        if(Physics.Raycast(transform.position, transform.forward, 1f)){
            walkPointSet = false;
        }
    }

    private void SearchWalkPoint(){
        //Calculate random point in range
        float randomZ = Random.Range(walkPointRange, walkPointRange);
        float randomX = Random.Range(walkPointRange, walkPointRange);
        float randomY = Random.Range(-4f, 4f);

        walkPoint = new Vector3(transform.position.x + randomX, randomY, transform.position.z + randomZ);

        if(Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround)){
            walkPointSet = true;
        }
    }

    private void Chasing(){
        //rig.weight = 0;
        animator.SetBool("isMoving", true);
        agent.SetDestination(player.transform.position);
    }

    private void Attacking(){
        //rig.weight = 1;
        //Make sure enemy doesn't move
        animator.SetBool("isMoving", false);
        agent.SetDestination(transform.position);
        var lookPos = player.transform.position;
        lookPos.y = transform.position.y;
        transform.LookAt(lookPos);

        if(!alreadyAttacked){

            //Attack
            GetComponent<SlimeAttack>().ThrowSlime(player.transform.position);

            // if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, attackRange)){
            //     if(damageOverTime){
            //         IEnumerator takedot =  playerTarget.TakeDamageOverTime(damage, hit_num);
            //         StartCoroutine(takedot);
            //         // if(playerTarget.currentHealth-enemy.damage<=0){
            //         //     Destroy(playerTarget.gameObject);
            //         // }
            //     }else{
            //         playerTarget.TakeDamage(damage);
            //     }

            //     alreadyAttacked = true;
            // }
            alreadyAttacked = true;

            StartCoroutine(resetAttack());
        }
    }

    private IEnumerator resetAttack(){
        yield return new WaitForSeconds(timeBetweenAttacks);
        alreadyAttacked = false;
        isShot = false;
    }

    private IEnumerator DestroyAfterSeconds(float seconds, GameObject muzzleFlash){
        yield return new WaitForSeconds(seconds);
        Destroy(muzzleFlash.gameObject);
    }

}
