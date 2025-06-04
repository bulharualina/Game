using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider))]
public class ChoppableTree : MonoBehaviour
{
    public bool playerInRange;
    public bool canBeChopped;

    public float treeMaxHealth;
    public float treeHealth;

    [SerializeField] private GameObject chopHolderUI;
   

    private PlayerAnimation playerAnimation;
  
    [SerializeField] private float hitDelay = 0.6f;
    private void Awake() 
    {
        if (playerAnimation == null)
        {
           
            playerAnimation = FindObjectOfType<PlayerAnimation>();
            
        }
    }
    private void Start()
    {
       
        treeHealth = treeMaxHealth;
       
        if (chopHolderUI != null)
        {
            chopHolderUI.SetActive(false);
        }
    }

    //private void Update()
    //{
     //   if (canBeChopped) 
     //   { 
     //       GlobalState.Instance.resourceHealth = treeHealth;
     //       GlobalState.Instance.resourceHealthMax = treeMaxHealth;
     //   }
   // }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            playerInRange = true;
            if (chopHolderUI != null) 
            { 
                chopHolderUI.SetActive(true); 
            }
            if (GlobalState.Instance != null)
            {
                GlobalState.Instance.resourceHealth = treeHealth;
                GlobalState.Instance.resourceHealthMax = treeMaxHealth;
                Debug.Log($"Player entered {gameObject.name}. GlobalState health updated to: {treeHealth}/{treeMaxHealth}");
            }

        }
        else if (other.CompareTag("AxeHitbox"))
        {

            if (playerInRange && canBeChopped && playerAnimation != null && playerAnimation.TryRegisterHit())
            { 
                StartCoroutine(ProcessHitDelayed());
               

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (chopHolderUI != null)
            {
                chopHolderUI.SetActive(false);
            }
            if (GlobalState.Instance != null)
            {
                GlobalState.Instance.resourceHealth = 0; // Reset to 0 when no tree is in range
                GlobalState.Instance.resourceHealthMax = 0; // Reset max too
                Debug.Log($"Player exited {gameObject.name}. GlobalState health reset.");
            }
        }
        
    }
    private IEnumerator ProcessHitDelayed()
    {
        

        yield return new WaitForSeconds(hitDelay); // Wait for the specified delay

       
        if (treeHealth > 0)
        {
            treeHealth -= 1;
            Debug.Log($"ChoppableTree ({gameObject.name}): Health decreased to: {treeHealth}");

            if (GlobalState.Instance != null && playerInRange) // Ensure player is still looking at this tree
            {
                GlobalState.Instance.resourceHealth = treeHealth;
               
                Debug.Log($"ChoppableTree ({gameObject.name}): GlobalState health updated after hit to: {treeHealth}");
            }

            if (treeHealth <= 0)
            {
                Debug.Log($"ChoppableTree ({gameObject.name}): Tree destroyed!");

                // Destroy the parent GameObject (Tree000n)
                Destroy(transform.parent.gameObject);

                if (chopHolderUI != null)
                {
                    chopHolderUI.SetActive(false); // Hide UI when tree is destroyed
                }
                
                if (GlobalState.Instance != null)
                {
                    GlobalState.Instance.resourceHealth = 0;
                    GlobalState.Instance.resourceHealthMax = 0;
                    Debug.Log($"ChoppableTree ({gameObject.name}): Destroyed. GlobalState health reset.");
                }
            }
        }
        else
        {
            
            Debug.Log($"ChoppableTree ({gameObject.name}): Hit attempted, but health was already 0 or less.");
        }

        yield return new WaitForSeconds(0.1f); 
       
    }

   
}
