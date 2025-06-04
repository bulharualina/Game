using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemInteractor : MonoBehaviour
{
    [SerializeField]
    private LayerMask pickableLayerMask;

    [SerializeField]
    private Transform playerCameraTransform;

    [SerializeField]
    private GameObject pickUpUI;

    private PlayerAnimation playerAnimation;


    [SerializeField]
    [Min(1)]
    private float hitRange = 3;

    private RaycastHit hit;
    public GameObject chopHolder;

    [SerializeField] private float axeActiveDuration = 0.2f; // How long the axe stays active after Q press
    private bool axeIsActive = false;
    private void Awake()
    {
        playerAnimation = GetComponent<PlayerAnimation>();
    
    }
    private void Start()
    {
        
        if (playerAnimation != null && playerAnimation.axeHitbox != null)
        {
            
            if (playerAnimation.axeHitbox.enabled)
            {
                playerAnimation.axeHitbox.enabled = false;
               
            }
        }
        else
        {
            Debug.LogError("ItemInteractor Start: playerAnimation or axeHitbox reference is NULL. Check PlayerAnimation Inspector assignment!");
        }
    }
    private void Update()
    {
     
        bool qKeyPressedThisFrame = Input.GetKeyDown(KeyCode.Q);

        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out hit, hitRange, pickableLayerMask))
        {
            var selectionTransform = hit.transform;
            ChoppableTree choppableTree = selectionTransform.GetComponent<ChoppableTree>();

            if (choppableTree == null)
            {
                Debug.LogWarning($"ItemInteractor Update: Raycast hit '{selectionTransform.name}' but no ChoppableTree component found on it!");
                // Continue only if ChoppableTree exists
                return; // Exit this part of the Update if no ChoppableTree
            }

            
            if (qKeyPressedThisFrame) 
            {
                
                if (choppableTree.playerInRange && choppableTree.canBeChopped && !axeIsActive) 
                {
                  

                    playerAnimation.TriggerChopAttack();
                   
                    EnableAxeColliderManual(); // Call the new method to activate
                    
                }
                
            }
            
            pickUpUI.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                PickUpItem(hit.collider.gameObject);
            }
        }
        else {
            if (pickUpUI != null && pickUpUI.activeSelf)
            {
                pickUpUI?.SetActive(false);

            }
        }
       

    }

    private void EnableAxeColliderManual()
    {
       
        if (playerAnimation != null && playerAnimation.axeHitbox != null)
        {
            playerAnimation.axeHitbox.enabled = true;
            axeIsActive = true;
           
            StartCoroutine(DisableAxeColliderAfterDelay(axeActiveDuration));
        }
        else
        {
            Debug.LogError("ItemInteractor: Cannot enable axe collider. playerAnimation or playerAnimation.axeHitbox is null.");
        }
    }

    private IEnumerator DisableAxeColliderAfterDelay(float delay)
    {
        
        yield return new WaitForSeconds(delay);
        if (playerAnimation != null && playerAnimation.axeHitbox != null)
        {
            playerAnimation.axeHitbox.enabled = false;
            axeIsActive = false;
           
        }
    }
    private void PickUpItem(GameObject item)
    {
        PickableItem pickable = item.GetComponent<PickableItem>();

        if (pickable == null)
        {
            Debug.LogError("Picked item '" +item.name+"' doesn't have PickableItem script attached.");
            return;
        }
        if (!InventorySystem.Instance.CheckIfInventoryFull())
        {
            InventorySystem.Instance.AddToInventory(pickable.ItemName);
            InventorySystem.Instance.pickedupItems.Add(pickable.ItemName);
            if (pickUpUI != null)
            {
                pickUpUI?.SetActive(false);
            }
            Destroy(item);

        }
        else {
            Debug.Log("Inventory is FULL");
        }
    }
}

   
