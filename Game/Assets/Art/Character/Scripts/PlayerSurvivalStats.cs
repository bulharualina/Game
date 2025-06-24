using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSurvivalStats : MonoBehaviour
{
    public static PlayerSurvivalStats Instance { get; set; }

   

    //Calories
    public float currentCalories;
    public float maxCalories;
    public float damagePerSecondFromStarvation = 5f;

    float distanceTravelled = 0;
    Vector3 lastPosition;

    public GameObject player;

    //Hydration
    public float currentHydrationPercent;
    public float maxHydrationPercent;
    public float damagePerSecondFromDehydration = 8f;
    public bool isHydrationActive;

    
    [Header("Damage/Healing")]
    public float deathDelay = 3f;


    [SerializeField] private Animator playerAnimator;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        
        currentCalories = maxCalories;
        currentHydrationPercent = maxHydrationPercent;
        StartCoroutine(decreaseHydrationHealth());

    }

    IEnumerator decreaseHydrationHealth() {
        while (true) 
        {
            currentHydrationPercent -= 2;
           
            yield return new WaitForSeconds(15); ;
        }
    
    }

    private void Update()
    {
        


        distanceTravelled += Vector3.Distance(player.transform.position,lastPosition);
        lastPosition = player.transform.position;

        if (distanceTravelled >= 10) 
        { 
            distanceTravelled = 0;
            currentCalories -= 1;
        }
       

    }
 

    private void Die()
    {
       

        Debug.Log("Player has died!");

     
         if (playerAnimator != null) playerAnimator.SetTrigger("Die");

        
        StartCoroutine(HandleDeathAndRespawn());
    }
    IEnumerator HandleDeathAndRespawn()
    {
        // Optional: Fade screen to black, show "You Died" message
        Debug.Log("Waiting for death delay...");
        yield return new WaitForSeconds(deathDelay); // Wait for the specified delay

       
        Debug.Log("Respawning...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reloads the current scene
    }
   
    public void setCalories(float newCalories) {
            currentCalories = newCalories;
    }
    public void setHydration(float newHydration) { 
    currentHydrationPercent = newHydration;
    }
}