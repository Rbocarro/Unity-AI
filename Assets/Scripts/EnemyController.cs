using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
public class EnemyController : MonoBehaviour
{   
    public enum EnemyState { patrol,chase, attack,flee};
    
    
    
    public EnemyState enemyState;
    public GameObject player;
    public float fireRate = 5f;
    public GameObject bulletPrefab;
    private float nextFire;
    public int enemyHealth;
    public int criticalHealth = 3;
    public string enemyName;


    public TextMeshProUGUI enemyHealthText;
    public TextMeshProUGUI enemyStateText;
    public TextMeshProUGUI enemyIDText;
    public Transform cameraTranform;

    //nav shiz
    public NavMeshSurface surface;  // Reference to the NavMesh surface
    public float moveRadius = 10f;  // Maximum distance the enemy can move from its starting position

    private NavMeshAgent agent;     // Reference to the NavMesh Agent component
    private Vector3 startingPos;    // The enemy's starting position
 

    void Start()
    {   
        enemyHealthText.color=Color.red;
        enemyHealth = 10;
        enemyName=gameObject.name;
        enemyState=EnemyState.patrol;
        player= GameObject.FindGameObjectWithTag("Player");


        //nav shiz
        surface= GameObject.FindGameObjectWithTag("Terrain").GetComponent<NavMeshSurface>();
        agent = GetComponent<NavMeshAgent>();
        startingPos = transform.position;

        cameraTranform= GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    // Update is called once per frame
    void Update()
    {
        DetermineActionState();//check what state to be in
        UpdateEnemyUI();

        switch (enemyState)
        {
            case(EnemyState.patrol):
                Patrol();
                break;
            case (EnemyState.chase):
                Chase();
                break;
            case (EnemyState.attack):
                Attack();
                break;
            case (EnemyState.flee):
                Flee();
                break;
            default:
                break;
        }


        if (enemyHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    void Patrol()
    {
        // Check if the agent has reached its destination
        if (agent.remainingDistance < agent.stoppingDistance)
        {
            // Set a new random destination within the moveRadius
            Vector3 randomPos = RandomPointOnNavMesh();
          //  GameObject newObject = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere),new Vector3(randomPos.x,randomPos.y,randomPos.z),Quaternion.identity);
            //newObject.transform.localScale= new Vector3(20, 20, 20);
            NavMesh.SamplePosition(randomPos, out NavMeshHit hit, moveRadius, NavMesh.AllAreas);
            agent.SetDestination(hit.position);

        }
    }

    void Chase()
    {
        Vector3 targetPos=player.transform.position;
        agent.SetDestination(targetPos);
        if(Random.Range(0,100)>75)
        {
           // Attack();
            enemyState= enemyState = EnemyState.attack;
        }
    }

    void Attack()
    {
        if (Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;

            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            bullet.GetComponent<BulletController>()?.InitializaBullet(this.transform.rotation * Vector3.forward);
            if (Random.Range(0, 100)> 95)
            {
                enemyState = enemyState = EnemyState.chase;
            }
                
        }
    }

    void Flee()
    {
        // Check if the agent has reached its destination
        if (agent.remainingDistance < agent.stoppingDistance)
        {
            // Set a new random destination within the moveRadius
            Vector3 randomPos = RandomPointOnNavMesh();
            if (Vector3.Distance(randomPos, player.transform.position)<=100f)
            {
                randomPos = RandomPointOnNavMesh();
            }
            else
            {
                //  GameObject newObject = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere),new Vector3(randomPos.x,randomPos.y,randomPos.z),Quaternion.identity);
                //newObject.transform.localScale= new Vector3(20, 20, 20);
                NavMesh.SamplePosition(randomPos, out NavMeshHit hit, moveRadius, NavMesh.AllAreas);
                agent.SetDestination(hit.position);
            }


        }
    }

    void DetermineActionState()
    {
        if(enemyHealth<=criticalHealth&& Vector3.Distance(transform.position, player.transform.position) < 250f)//if low on health and close to player then run away
        {
            agent.speed += 100;
            enemyState=EnemyState.flee;
        }

        if ((enemyHealth >= criticalHealth)&& Vector3.Distance(transform.position,player.transform.position)<250f)// if not low on health and player is nearby then chase it
        {
            enemyState = EnemyState.chase;
        }

        if ((enemyHealth >= criticalHealth) && Vector3.Distance(transform.position, player.transform.position) > 500f)
        {
            enemyState = EnemyState.patrol;
        }


    }

    private Vector3 RandomPointOnNavMesh()
    {
        Vector3 randomDirection = Random.insideUnitSphere * moveRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, moveRadius, -1))
        {
            finalPosition = hit.position;

        }
        return finalPosition;
    }

    private void UpdateEnemyUI()
    {   
        
        enemyHealthText.text="HP: "+enemyHealth.ToString();
        enemyStateText.text="STATE: "+enemyState.ToString();
        enemyIDText.text = "ID: " + enemyName;

        enemyHealthText.transform.LookAt(cameraTranform.position*-1);
        enemyStateText.transform.LookAt(cameraTranform.position * -1);
        enemyIDText.transform.LookAt(cameraTranform.position * -1);

    }

    void OnTriggerEnter(Collider col)
    {
        switch (col.tag)
        {
            case "Bullet":
                if(Random.Range(0, 10) >= 9)
                {
                    enemyHealth -= 5;//take critical damage
                    Debug.Log(enemyName + " Took Critical Damage!");
                }
                else
                {
                    enemyHealth -= 1;
                }
                Destroy(col.gameObject);
                break;
            default:
                Debug.Log("Tf Dis");
                break;

        }

    }
}
