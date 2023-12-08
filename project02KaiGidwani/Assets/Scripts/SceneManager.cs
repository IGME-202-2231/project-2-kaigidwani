using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class SceneManager : MonoBehaviour
{
    // Prefab for small fish
    [SerializeField] private GameObject smallFishPrefab;

    // Prefab for swordfish
    [SerializeField] private GameObject swordfishPrefab;

    // Prefab for fish food
    [SerializeField] private GameObject fishFoodPrefab;

    // List for all fish
    [SerializeField] public List<GameObject> allFish;

    // The camera for the scene
    [SerializeField] private Camera cameraObject;

    [SerializeField] private int minFish;

    [SerializeField] private int maxFish;

    // Window bounds
    private float height;
    private float width;

    private Vector3 mousePosition;

    private List<GameObject> allFishFood;
    public List<GameObject> AllFishFood { get { return allFishFood; } }

    // Start is called before the first frame update
    void Start()
    {
        height = 0.9f * cameraObject.orthographicSize * 2f;

        // Width isn't a value on the camera object
        width = height * cameraObject.aspect;

        // Initiate allFish
        allFish = new List<GameObject>();

        // Initiate allFishFood
        allFishFood = new List<GameObject>();

        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        // Get the current mouse position
        mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePosition.z = 0;
    }

    public void SpawnFishFood(InputAction.CallbackContext context)
    {
        Debug.Log("Spawning fish food");
        // Only spawns once per click
        if (context.phase == InputActionPhase.Performed)
        {
            allFishFood.Add(
            Instantiate(
                fishFoodPrefab,
                new Vector3(mousePosition.x, mousePosition.y, 0),
                Quaternion.identity)
                );
        }
    }

    // Spawn creatures
    public void Spawn()
    {
        CleanUp();

        
        int numFish = Random.Range(minFish, maxFish);
        for (int i = 0; i < numFish; i++)
        {
            SpawnFish(smallFishPrefab);
            // Spawn a lesser but appropiate amount of swordfish
            /*
            if (i % 3 == 0)
            {
                SpawnFish(swordfishPrefab);
            }*/
            
        }
        
        // Just spawn 2 swordfish
        SpawnFish(swordfishPrefab, new Vector3(-5, 3, 0));
        SpawnFish(swordfishPrefab, new Vector3(4, 4, 0));
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

    // Spawn a fish
    private void SpawnFish(GameObject fishPrefab)
    {
        // Create one
        allFish.Add(
            Instantiate(
                fishPrefab,
                new Vector3(
                    Gaussian(0.0f, width / 8),
                    Gaussian(0.0f, height / 8),
                    0),
                Quaternion.identity)
                );
        GameObject thisFish = allFish[allFish.Count - 1];
        thisFish.GetComponent<Agent>().Manager = this;
    }
    private void SpawnFish(GameObject fishPrefab, Vector3 position)
    {
        // Create one
        allFish.Add(
            Instantiate(
                fishPrefab,
                position,
                Quaternion.identity)
                );
        GameObject thisFish = allFish[allFish.Count - 1];
        thisFish.GetComponent<Agent>().Manager = this;
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
