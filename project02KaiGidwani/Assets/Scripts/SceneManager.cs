using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.InputSystem;

public class SceneManager : MonoBehaviour
{
    [SerializeField] List<PhysicsObject> physicsObjects;
    [SerializeField] private GameObject crowPrefab;
    [SerializeField] private GameObject eaglePrefab;
    private List<GameObject> allBirds = new List<GameObject>();

    // The camera for the scene
    [SerializeField] private Camera cameraObject;

    // Window bounds
    private float height;
    private float width;

    //private Vector3 mousePosition;


    // Start is called before the first frame update
    void Start()
    {
        height = 0.9f * cameraObject.orthographicSize * 2f;

        // Width isn't a value on the camera object
        width = height * cameraObject.aspect;

        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        /** InputSystem isn't working for me and it is not showing up in the settings to change it.
         * I dont need it immediatley at the moment so im jut temporarily disabling it.
         * 
        mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePosition.z = 0;

        foreach(PhysicsObject physicsObject in physicsObjects)
        {
            physicsObject.ApplyForce(mousePosition - physicsObject.transform.position);
        }
        */
    }

    // Spawn creatures
    public void Spawn()
    {
        CleanUp();

        int numBirds = Random.Range(5, 15);
        for (int i = 0; i < numBirds; i++)
        {
            SpawnCrow();
            if (i % 3 == 0)
            {
                SpawnEagle();
            }
        }
    }


    // Clean up
    void CleanUp()
    {
        foreach (GameObject c in allBirds)
        {
            Destroy(c); // Remove them from the scene
        }
        allBirds.Clear(); // Remove the references to them
    }

    // Spawn a crow
    private void SpawnCrow()
    {
        // Create one
        allBirds.Add(
            Instantiate(
                crowPrefab,
                new Vector3(

                    // ***Rewrite to have gaussian spread***
                    Gaussian(0.0f, width / 8),
                    Gaussian(0.0f, height / 8),
                    0),
                Quaternion.identity)
                );
    }

    // Spawn an eagle
    private void SpawnEagle()
    {
        // Create one
        allBirds.Add(
            Instantiate(
                eaglePrefab,
                new Vector3(

                    // ***Rewrite to have gaussian spread***
                    Gaussian(0.0f, width / 8),
                    Gaussian(0.0f, height / 8),
                    0),
                Quaternion.identity)
                );
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

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawLine(transform.position, mousePosition);
    }
}
