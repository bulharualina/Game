using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour
{
    public string animalName;
    public bool playerInRange;
   


    [SerializeField] int _animalHealth;
    [SerializeField] int _animalMaxHealth;
    [SerializeField] private GameObject _huntingUI;

    void Start()
    {
        _animalHealth = _animalMaxHealth;
        _huntingUI.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Animal OnTriggerEnter hit: " + other.gameObject.name);
       
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            _huntingUI.SetActive(true);
            Debug.Log("Player entered Animal range.");
            
        }
        else if (other.CompareTag("AxeHitbox"))
        {

            Weapon weapon = other.GetComponent<Weapon>();
            if (weapon != null)
            {
                TakeDamage(weapon.weaponDamage); // Call TakeDamage with the axe's damage value
                Debug.Log($"Animal hit by axe. Damage: {weapon.weaponDamage}");
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Animal OnTriggerExit hit: " + other.gameObject.name);
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            _huntingUI.SetActive(false);
            Debug.Log("Player exited Animal range.");
        }
       

    }

    internal void TakeDamage(int damage)
    {
        _animalHealth -= damage;
        Debug.Log($" health updated to: {_animalHealth}/{_animalMaxHealth}");
        if (_animalHealth<=0) 
        {
            StartCoroutine(DestroyAnimalDelayed());
           
        }
    }



    private IEnumerator DestroyAnimalDelayed()
    {
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
        _huntingUI.SetActive(false);


        GameObject killedAnimal = Instantiate(Resources.Load<GameObject>("MeatModel"), transform.position, Quaternion.Euler(0, 0, 0));
    }



}
