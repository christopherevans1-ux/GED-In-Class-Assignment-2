using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BetterEnemy : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public LayerMask obstacles;

    //patrol
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //attack or Weapon stuff
    public float timeBetweenAttacks;
    bool alreadyattacked;
    public GameObject projectile;
    public float damage;
    public Animator weaponAnimator;
    public Animator enemyAnimator;
    public AudioSource meleeSwing;
    public AudioSource meleeHit;
    public bool isSwing;
    public bool isMelee;
    public int givenWeapon;

    //states
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    public ParticleSystem enemyTrail;


    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        enemyAnimator.SetBool("isMelee", isMelee);
        if (enemyTrail == null)
        {
            enemyTrail = GetComponentInChildren<ParticleSystem>();
        }
        if (enemyTrail != null)
        {
            var emission = enemyTrail.emission;
            emission.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(weaponAnimator != null && gameObject.name != "Bull(WarOnCowsReference)")
        {
            weaponAnimator.SetInteger("weaponType", givenWeapon);
        }
        if (PauseMenu.GameIsPaused) return;
        //check player sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if ((!HasLOS() && !playerInSightRange) && !playerInAttackRange) Patroling();
        if ((HasLOS() && playerInSightRange) && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }
        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if(distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }
    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if(Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        //make sure enemy doesn't move(scrapping this)
        //agent.SetDestination(transform.position);
        //transform.LookAt(player);

        if (!alreadyattacked)
        {
            //attack code here
            isSwing = true;
            enemyAnimator.SetBool("isSwing", isSwing);
            if(weaponAnimator != null) weaponAnimator.SetBool("isSwing", isSwing);
            if (weaponAnimator != null) meleeSwing.Play();
            Invoke("EndSwing", 0.55f);
            if (enemyTrail != null)
            {
                var emission = enemyTrail.emission;
                emission.enabled = true;
                enemyTrail.Play();
            }


            alreadyattacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyattacked = false;  
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
    /*private void OnTriggerEnter(Collider other)
    {
            if (other.name == "Player")
            {
                //Destroy(other.gameObject);
                other.GetComponent<Movement>().health -= damage;
                Debug.Log("i touched the player");
            }
    }*/
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player") && player.gameObject.activeSelf == true)
        {
            collision.gameObject.GetComponent<Movement>().health -= damage;
            meleeHit.Play();
        }
    }

    public bool HasLOS()
    {
        Vector3 directionToTarget = player.position - transform.position;
        float distanceToTarget = directionToTarget.magnitude;

        Debug.DrawLine(transform.position, player.position, Color.red);

        if(Physics.Linecast(transform.position, player.position, out RaycastHit hit, obstacles))
        {
            if(hit.transform == player)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return true;
        }
    }
    public void EndSwing()
    {
        isSwing = false;
        weaponAnimator.SetBool("isSwing", isSwing);
        enemyAnimator.SetBool("isSwing", isSwing);
        if (enemyTrail != null)
        {
            var emission = enemyTrail.emission;
            emission.enabled = false;
        }
    }
}
