using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SceneManager : MonoBehaviour
{
    // Prefab for crows
    [SerializeField] private GameObject smallFishPrefab;

    // Prefab for eagles
    [SerializeField] private GameObject swordfishPrefab;

    // List for all birds
    [SerializeField] public List<GameObject> allFish;

    // The camera for the scene
    [SerializeField] private Camera cameraObject;

    // Window bounds
    private float height;
    private float width;

    private Vector3 mousePosition;


    // Start is called before the first frame update
    void Start()
    {
        height = 0.9f * cameraObject.orthographicSize * 2f;

        // Width isn't a value on the camera object
        width = height * cameraObject.aspect;

        // Initiate allBirds
        allFish = new List<GameObject>();

        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        // Get the current mouse position
        mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePosition.z = 0;
    }

    // Spawn creatures
    public void Spawn()
    {
        CleanUp();

        int numFish = Random.Range(15, 30);
        for (int i = 0; i < numFish; i++)
        {
            SpawnSmallFish();
            /* Spawn a lesser but appropiate amount of swordfish
            if (i % 3 == 0)
            {
                SpawnSwordFish();
            }
            */
        }
        // Just spawn 2 swordfish
        SpawnSwordFish();
        SpawnSwordFish();

    }


    // Clean up
    void CleanUp()
    {
        foreach (GameObject c in allFish)
        {
            Destroy(c); // Remove them from the scene
        }
        allFish.Clear(); // Remove the references to them
    }

    // Spawn a crow
    private void SpawnSmallFish()
    {
        // Create one
        allFish.Add(
            Instantiate(
                smallFishPrefab,
                new Vector3(
                    Gaussian(0.0f, width / 8),
                    Gaussian(0.0f, height / 8),
                    0),
                Quaternion.identity)
                );
        allFish[allFish.Count - 1].GetComponent<Agent>().Manager = this;
    }

    // Spawn an eagle
    private void SpawnSwordFish()
    {
        // Create one
        allFish.Add(
            Instantiate(
                swordfishPrefab,
                new Vector3(
                    Gaussian(0.0f, width / 8),
                    Gaussian(0.0f, height / 8),
                    0),
                Quaternion.identity)
                );
        allFish[allFish.Count - 1].GetComponent<Agent>().Manager = this;
    }

    // Method for gaussian curve formula
    // Got from week 6 Random slides in IGME-202:
    // https://docs.google.com/presentation/d/1LWyxK_8rvqkRHd5Xt9S2A5q1VzEi9yveC9jISHYG6tg/edit?usp=sharing
    float Gaussian(float mean, float stdDev)
    {
        float val1 = Random.Range(0f, 1f);
        float val2 = Random.Range(0f, 1f);
        float gaussValue =
                 Mathf.Sqrt(-2.0f * Mathf.Log(val1)) *
                 Mathf.Sin(2.0f * Mathf.PI * val2);
        return mean + stdDev * gaussValue;
    }

    /*
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, mousePosition);
    }
    */
}
