using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour
{
    public string animalName;
    public bool playerInRange;
    //public bool canBeKilled;


    [SerializeField] int animalHealth;
    [SerializeField] int animalMaxHealth;
    //public int damage;
    //[SerializeField] private GameObject animalHealthUI;

    //private PlayerAnimation playerAnimation;

    //[SerializeField] private float hitDelay = 0.6f;
    //private bool isInvincible = false;
   /* private void Awake()
    {
        if (playerAnimation == null)
        {

            playerAnimation = FindObjectOfType<PlayerAnimation>();

        }
    }*/
    // Start is called before the first frame update
    void Start()
    {
        animalHealth = animalMaxHealth;
       /* if (animalHealthUI != null)
        {
            animalHealthUI.SetActive(false);
        }*/
       // damage = EquipSystem.Instance.GetWeaponDamage();
    }
    private void OnTriggerEnter(Collider other)
    {
       // Debug.Log("Animal OnTriggerEnter hit: " + other.gameObject.name);
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
           /* if (animalHealthUI != null)
            {
                animalHealthUI.SetActive(true);
            }
            if (GlobalState.Instance != null)
            {
                GlobalState.Instance.resourceHealth = animalHealth;
                GlobalState.Instance.resourceHealthMax = animalMaxHealth;
                Debug.Log($"Player entered {gameObject.name} range. GlobalState health updated to: {animalHealth}/{animalMaxHealth}");
            }*/
        }
        /*else if (other.CompareTag("AxeHitbox"))
        {
            Debug.Log("Animal hit by AxeHitbox!");
            if (playerInRange && canBeKilled && playerAnimation != null && playerAnimation.TryRegisterHit())
            {
                
                StartCoroutine(ProcessHitDelayed());

            }
        }*/

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
           /* if (animalHealthUI != null)
            {
                animalHealthUI.SetActive(false);

            }
            if (GlobalState.Instance != null)
            {
                GlobalState.Instance.resourceHealth = 0;
                GlobalState.Instance.resourceHealthMax = 0;
                Debug.Log($"Player exited {gameObject.name} range. GlobalState health reset.");
            }*/
        }

    }

    internal void TakeDamage(int damage)
    {
        animalHealth -= damage;
        Debug.Log($" health updated to: {animalHealth}/{animalMaxHealth}");
        if (animalHealth<=0) 
        {
            Destroy(gameObject);
        }
    }


    /*  private IEnumerator ProcessHitDelayed()
      {
          //isInvincible = true;

         yield return new WaitForSeconds(hitDelay);

          if (animalHealth > 0) 
          {
              animalHealth -= 1;
              //PlayerSurvivalStats.Instance.currentCalories -= caloriesSpentChopping;
              if (GlobalState.Instance != null && playerInRange) // Only update if player is still interacting with THIS animal
              {
                  GlobalState.Instance.resourceHealth = animalHealth;
                  GlobalState.Instance.resourceHealthMax = animalMaxHealth;
              }

              if (animalHealth <= 0) 
              {
                  DestroyAnim();

                 / if (animalHealthUI != null)
                  {
                      animalHealthUI.SetActive(false);
                  }
                  if (GlobalState.Instance != null)
                  {
                      GlobalState.Instance.resourceHealth = 0;
                      GlobalState.Instance.resourceHealthMax = 0;
                      Debug.Log($"Animal ({gameObject.name}): Died. GlobalState health reset.");
                  }
              }
          }
          // isInvincible = false;

          yield return new WaitForSeconds(0.1f);

      }


      void DestroyAnim() 
      {
          Vector3 animPos = transform.position;
          Destroy(gameObject);
          canBeKilled = false;

          GameObject meat = Instantiate(Resources.Load<GameObject>("AnimalMeat"), new Vector3(animPos.x, animPos.y + 0.5f, animPos.z), Quaternion.Euler(0, 0, 0));
      }
    */




}
