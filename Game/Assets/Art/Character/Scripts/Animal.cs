using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Animal : MonoBehaviour
{
    public string animalName;
    public bool playerInRange;
    public bool canBeKilled;



    public float animalHealth;
    public float animalMaxHealth;


    [SerializeField] private GameObject _huntingUI;
    [SerializeField] private GameObject _animalHolderUI;

    private PlayerAnimation playerAnimation;

    [SerializeField] private float hitDelay = 0.6f;

    public float caloriesSpentHunting = 30;

    private void Awake()
    {
        if (playerAnimation == null)
        {

            playerAnimation = FindObjectOfType<PlayerAnimation>();

        }
    }

    void Start()
    {
        animalHealth = animalMaxHealth;

        if (_animalHolderUI != null)
        {
            _animalHolderUI.SetActive(false);
        }


        if (_huntingUI != null)
        {
            _huntingUI.SetActive(false);
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        Weapon weapon = other.GetComponent<Weapon>();
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (_huntingUI != null)
            {
                _huntingUI.SetActive(true);
            }

            if (_animalHolderUI != null)
            {
                _animalHolderUI.SetActive(true);
            }
           
            if (GlobalState.Instance != null)
            {
                GlobalState.Instance.resourceHealth = animalHealth;
                GlobalState.Instance.resourceHealthMax = animalMaxHealth;
            }

        }
        else if (other.CompareTag("AxeHitbox"))
        {

            if (playerInRange && canBeKilled && playerAnimation != null && playerAnimation.TryRegisterHit() && weapon != null)
            {
                TakeDamage(weapon.weaponDamage);
            }
           
           
           
        }

    }

    internal void TakeDamage(int damage)
    {
        if (animalHealth > 0)
        {
            if (PlayerSurvivalStats.Instance != null)
            {
                PlayerSurvivalStats.Instance.currentCalories -= caloriesSpentHunting;
            }

            animalHealth -= damage;


            if (GlobalState.Instance != null && playerInRange) // Ensure player is still looking at this tree
            {
                GlobalState.Instance.resourceHealth = animalHealth;
            }

            if (animalHealth <= 0)
            {
                StartCoroutine(DestroyAnimalDelayed());


                if (_huntingUI != null)
                {
                    _huntingUI.SetActive(false);
                }
                if (_animalHolderUI != null)
                {
                    _animalHolderUI.SetActive(false);
                }

                if (GlobalState.Instance != null)
                {
                    GlobalState.Instance.resourceHealth = 0;
                    GlobalState.Instance.resourceHealthMax = 0;

                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (_huntingUI != null)
            {
                _huntingUI.SetActive(false);
            }

            if (_animalHolderUI != null)
            {
                _animalHolderUI.SetActive(false);
            }

            if (GlobalState.Instance != null)
            {
                GlobalState.Instance.resourceHealth = 0; 
                GlobalState.Instance.resourceHealthMax = 0; 
                
            }
            Debug.Log("Player exited Animal range.");
        }
       

    }


 


  
    private IEnumerator DestroyAnimalDelayed()
    {
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
     
        GameObject killedAnimal = Instantiate(Resources.Load<GameObject>("MeatModel"), transform.position, Quaternion.Euler(0, 0, 0));
    }



}
