using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour
{
    public string animalName;
    public bool playerInRange;
   


    [SerializeField] int animalHealth;
    [SerializeField] int animalMaxHealth;

    void Start()
    {
        animalHealth = animalMaxHealth;
     
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Animal OnTriggerEnter hit: " + other.gameObject.name);
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
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
            Debug.Log("Player exited Animal range.");
        }
       

    }

    internal void TakeDamage(int damage)
    {
        animalHealth -= damage;
        Debug.Log($" health updated to: {animalHealth}/{animalMaxHealth}");
        if (animalHealth<=0) 
        {
            StartCoroutine(DestroyAnimalDelayed());
           
        }
    }



    private IEnumerator DestroyAnimalDelayed()
    {
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
        

        GameObject killedAnimal = Instantiate(Resources.Load<GameObject>("MeatModel"), transform.position, Quaternion.Euler(0, 0, 0));
    }



}
