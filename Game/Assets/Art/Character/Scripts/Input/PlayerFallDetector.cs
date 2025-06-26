using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class PlayerFallDetector : MonoBehaviour
{
  
    public float fallThresholdY = -10f; 
    private PlayerSurvivalStats playerSurvivalStats;

    [SerializeField] private GameObject deathPanel;

    [Header("Damage/Healing")]
    public float deathDelay = 3f;

   
    private void Start()
    {
        
        playerSurvivalStats = PlayerSurvivalStats.Instance;

        if (playerSurvivalStats == null)
        {
            Debug.LogError("PlayerSurvivalStats instance not found! Make sure it exists in the scene and is initialized before PlayerFallDetector.");
          
            enabled = false;
        }

        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
        }

    }

    void Update()
    {
       
        if (transform.position.y < fallThresholdY)
        {
            Debug.Log("Player fell below threshold!");
            TriggerPlayerDeath();
        }
    }

    
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered death zone trigger!");
            TriggerPlayerDeath();
        }
    }

    private void TriggerPlayerDeath()
    {

        


            StartCoroutine(HandleDeathAndRespawn());
    
    }

    private IEnumerator HandleDeathAndRespawn()
    {

            yield return new WaitForSeconds(deathDelay);

            if (deathPanel != null)
            {
                deathPanel.SetActive(true);
            }

            Time.timeScale = 0f;

            float timer = 0f;
            float waitAfterPanelAppears = 2f;
            while (timer < waitAfterPanelAppears)
            {
                timer += Time.unscaledDeltaTime;
                yield return null;
            }


            Time.timeScale = 1f;


            Debug.Log("Respawning...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}