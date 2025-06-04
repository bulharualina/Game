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
        Debug.Log($"ChoppableTree (Start): Initialized treeHealth to {treeHealth} (from treeMaxHealth: {treeMaxHealth})");
        if (chopHolderUI != null)
        {
            chopHolderUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (canBeChopped) 
        { 
            GlobalState.Instance.resourceHealth = treeHealth;
            GlobalState.Instance.resourceHealthMax = treeMaxHealth;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            playerInRange = true;
            if (chopHolderUI != null) 
            { 
                chopHolderUI.SetActive(true); 
            }
            
        }
        else if (other.CompareTag("AxeHitbox"))
        {

            if (playerInRange && canBeChopped && playerAnimation != null && playerAnimation.TryRegisterHit())
            {
                Debug.Log("Tree hit by Axe Hitbox!");
                GetHit();

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
        }
        
    }

    public void GetHit() 
    {
        if (treeHealth > 0)
        {
            treeHealth -= 1;
            Debug.Log($"Tree health: {treeHealth}");
           
            if (treeHealth <= 0)
            {
                Debug.Log("Tree destroyed!");
                // Add logic for destroying the tree, spawning logs, etc.
                Destroy(gameObject);
                if (chopHolderUI != null)
                {
                    chopHolderUI.SetActive(false); // Hide UI when tree is destroyed
                }
            }
        }
    }
}
