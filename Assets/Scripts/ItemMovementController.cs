using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMovementController : MonoBehaviour
{
    public float rotationSpeed = 30f; // Speed of rotation in degrees per second
    public float bobSpeed = 0.5f; // Speed of bobbing motion in units per second
    public float bobHeight = 0.5f; // Height of bobbing motion in units
    public float offset;

    private float startingY; // Starting Y position of the object

    void Start()
    {
        offset = Random.Range(0.01f, 359f);
        transform.Rotate(0f,offset,0f);
        startingY = transform.position.y; // Record the starting Y position of the object
    }

    void Update()
    {
        // Spin the object around its Y axis
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);

        // Bob the object up and down
        float newY = startingY + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
