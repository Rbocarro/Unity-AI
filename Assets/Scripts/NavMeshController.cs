using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshController : MonoBehaviour
{
    NavMeshSurface navmeshSurface;
    public GameObject mapGenerator;

    [Header("K-Means Clustering Parameters")]
    public GameObject[] itemPrefab;
    public int numItems = 10;
    public float yOffset = 200.5f;
    public float spawnRadius = 2000f;
    public int numClusters = 5;
    public float cylinderYScale = 10f;
    public float clusterThreshold = 0.1f;//used to determine when the k-means clustering algorithm has converged and should stop iterating
    private List<GameObject> items = new List<GameObject>();
    private List<GameObject> centroids = new List<GameObject>();

    void Awake()
    {
        mapGenerator.GetComponent<MapGenerator>().GenerateMap();
        //navmeshSurface = GetComponent<NavMeshSurface>();
        gameObject.GetComponent<NavMeshSurface>()?.BuildNavMesh();//navmeshSurface.BuildNavMesh();

        MeshCollider meshCollider = GetComponent<MeshCollider>();
        
        Mesh mesh = meshCollider.sharedMesh;// Modify the mesh data of the MeshCollider

        meshCollider.sharedMesh = mesh;// Make changes to the mesh here

        meshCollider.sharedMesh.RecalculateBounds();// Recalculate the bounds of the MeshCollider to ensure proper collision detection


        for (int i = 0; i < numItems; i++)
        {
            
            Vector3 randomPoint = RandomPointOnNavMesh();
            Vector3 spawnPoint = new Vector3(randomPoint.x, randomPoint.y + yOffset, randomPoint.z);
            GameObject Item=Instantiate(itemPrefab[Random.Range(0,itemPrefab.Length)], spawnPoint, Quaternion.identity);
            items.Add(Item);
        }

        // Perform k-means clustering
        List<List<GameObject>> clusters = KMeansClustering(items, numClusters, clusterThreshold);

        // Assign colors to cubes and create cylinders for centroids
        for (int i = 0; i < clusters.Count; i++)
        {
            Color color = new Color(Random.value, Random.value, Random.value);
            foreach (GameObject cube in clusters[i])
            {
                cube.GetComponent<Renderer>().material.color = color;
            }

            GameObject centroid = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            centroid.transform.position = GetCentroid(clusters[i]);
            centroid.transform.localScale = new Vector3(20, 20*cylinderYScale, 20);
            Destroy(centroid.GetComponent<CapsuleCollider>());
            centroid.GetComponent<Renderer>().material.color = color;
            centroids.Add(centroid);
        }

    }
    private Vector3 RandomPointOnNavMesh()
    {
        Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, spawnRadius, -1))
        {
            finalPosition = hit.position;
            
        }
        return finalPosition;
    }

    private List<List<GameObject>> KMeansClustering(List<GameObject> objects, int numClusters, float threshold)
    {
        // Initialize clusters
        List<List<GameObject>> clusters = new List<List<GameObject>>();
        for (int i = 0; i < numClusters; i++)
        {
            clusters.Add(new List<GameObject>());
        }

        // Initialize centroids
        List<Vector3> centroids = new List<Vector3>();
        for (int i = 0; i < numClusters; i++)
        {
            centroids.Add(objects[Random.Range(0, objects.Count)].transform.position);
        }

        // Iterate until the cluster convergence
        while (true)
        {
            // Assign objects to clusters
            for (int i = 0; i < objects.Count; i++)
            {
                int closestClusterIndex = GetClosestClusterIndex(objects[i].transform.position, centroids);
                clusters[closestClusterIndex].Add(objects[i]);
            }

            // Calculate new centroids
            List<Vector3> newCentroids = new List<Vector3>();
            //List<GameObeject> newCentroids = new List<GameObeject>();
            for (int i = 0; i < clusters.Count; i++)
            {
                if (clusters[i].Count > 0)
                {
                    newCentroids.Add(GetCentroid(clusters[i]));
                }
                else
                {
                    newCentroids.Add(centroids[i]);
                }
            }
            // Check convergence
            bool converged = true;
            for (int i = 0; i < centroids.Count; i++)
            {
                if (Vector3.Distance(centroids[i], newCentroids[i]) > threshold)// if abve treshhold then centroid position has not stablised
                {
                    converged = false;
                    break;
                }
            }

            if (converged)
            {
                break;
            }

            centroids = newCentroids;

            // Clear clusters
            for (int i = 0; i < clusters.Count; i++)
            {
                clusters[i].Clear();
            }
        }

        return clusters;
    }

    // Get the index of the closest centroid to a position
    private int GetClosestClusterIndex(Vector3 position, List<Vector3> centroids)
    {
        float closestDistance = float.MaxValue;
        int closestIndex = 0;

        for (int i = 0; i < centroids.Count; i++)
        {
            float distance = Vector3.Distance(position, centroids[i]);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }

    // Get the centroid of a list of objects
    private Vector3 GetCentroid(List<GameObject> objects)
    {
        Vector3 centroid = Vector3.zero;

        foreach (GameObject obj in objects)
        {
            centroid += obj.transform.position;
        }

        if (objects.Count > 0)
        {
            centroid /= objects.Count;
        }

        return centroid;
    }


    //bool IsObjectAccessibleToPlayer(Vector3 objectPosition)
    //{
    //    NavMeshPath path = new NavMeshPath();
    //    if (NavMesh.CalculatePath(playerAgent.transform.position, objectPosition, NavMesh.AllAreas, path))
    //    {
    //        return true;
    //    }

    //    return false;
    //}


}
