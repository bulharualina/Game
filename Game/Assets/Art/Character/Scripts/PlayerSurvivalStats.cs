using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSurvivalStats : MonoBehaviour
{
    public static PlayerSurvivalStats Instance { get; set; }

    //Health
    public float currentHealth;
    public float maxHealth;
    public bool isDead = false;

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
        currentHealth = maxHealth;
        currentCalories = maxCalories;
        currentHydrationPercent = maxHydrationPercent;
        StartCoroutine(decreaseHydrationHealth());

    }

    IEnumerator decreaseHydrationHealth() {
        while (true) 
        {
            currentHydrationPercent -= 2;
            currentHealth -= 0.5f;
            yield return new WaitForSeconds(15); ;
        }
    
    }

    private void Update()
    {
        if (isDead) return;


        distanceTravelled += Vector3.Distance(player.transform.position,lastPosition);
        lastPosition = player.transform.position;

        if (distanceTravelled >= 10) 
        { 
            distanceTravelled = 0;
            currentCalories -= 1;
        }
        if (currentCalories <= 0)
        {
            TakeDamage(5);
            // Debug.Log("Starving! Taking damage.");
        }
        if (currentHydrationPercent <= 0)
        {
            TakeDamage(10);
            
        }

    }
    public void TakeDamage(float amount) // Using float for consistency, can be int if preferred
    {
        if (isDead) return; // Can't take damage if already dead

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return; // Prevent multiple calls
        isDead = true;

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
    public void setHealth(float newHealth) { 
        currentHealth = newHealth;
    }
    public void setCalories(float newCalories) {
            currentCalories = newCalories;
    }
    public void setHydration(float newHydration) { 
    currentHydrationPercent = newHydration;
    }
}