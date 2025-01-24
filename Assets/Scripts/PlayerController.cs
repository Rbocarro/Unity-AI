using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // how fast the player moves
    public float rotationSpeed=50f;
    public int maxHealth = 100; // maximum health of the player
    public int startingMoney = 0; // initial amount of money the player has
    private int health; // current health of the player
    private int money; // current amount of money the player has

    private Rigidbody rb;


    [Header("Bullet Properites")]
    public float fireRate = 0.75f;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPos;
    private float nextFire;

    private float groundAngle;
    public float maxGroundAngle = 75f;

    private void Start()
    {
        health = maxHealth;
        money = startingMoney;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {

        HandleInput();

        //Fire Bullet
        if (Input.GetKey(KeyCode.Mouse0))
            Fire();
    }

    void Fire()
    {
        if (Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;

            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPos.position, Quaternion.identity);
            bullet.GetComponent<BulletController>()?.InitializaBullet(this.transform.rotation * Vector3.forward);
        }
    }


    void HandleInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");



        //rotate the player to be perpendicular to the surface they are standing on
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 1f))
        {
            Quaternion rotation = Quaternion.FromToRotation(transform.up, hit.normal);
            rb.MoveRotation(rotation * transform.rotation);
        }


        Vector3 direction = new Vector3(0f, 0f, verticalInput).normalized;

        Vector3 targetPosition = transform.position + transform.forward * direction.z * moveSpeed * Time.deltaTime
                                                 + transform.right * direction.x * moveSpeed * Time.deltaTime
                                                 + transform.up * direction.y * moveSpeed * Time.deltaTime;

        rb.MovePosition(targetPosition);
        //rb.AddForce(transform.forward*verticalInput * moveSpeed * Time.deltaTime);

        Quaternion targetRotation = transform.rotation * Quaternion.Euler(Vector3.up * (rotationSpeed * horizontalInput * Time.deltaTime));
        rb.MoveRotation(targetRotation);




        //RaycastHit hit;

        // Cast a ray directly downwards from the player's position
        if (Physics.Raycast(transform.position, -transform.up, out hit))
        {
            // Get the normal vector of the surface the player is on
            Vector3 surfaceNormal = hit.normal;

            // Rotate the player to align with the surface normal
            transform.rotation = Quaternion.FromToRotation(transform.up, surfaceNormal) * transform.rotation;

            // Apply a force downwards to keep the player on the surface
            rb.AddForce(surfaceNormal * Physics.gravity.magnitude * -1, ForceMode.Acceleration);
        }



    }


    public int GetHealth()
    {
        return health;
    }
    public int GetMoney()
    {
        return money;
    }



    void OnTriggerEnter(Collider col)
    {   switch(col.tag)
        {
            case "Coin":
                money += 1;
                Destroy(col.gameObject);
                break;
            case "Heart":
                health += 10;
                Destroy(col.gameObject);
                break;
            case "EnemyBullet":
                if(Random.Range(0, 100) > 90)
                {
                    health -= 50;//take critical damage
                }
                else
                {
                    health -= 10;
                }
                Destroy(col.gameObject);
                break;
            default:
                Debug.Log("Tf Dis");
                break;

        }

    }
}
