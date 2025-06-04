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
        // Initial check to confirm axeHitbox reference and initial state
        if (playerAnimation != null && playerAnimation.axeHitbox != null)
        {
            //Debug.Log($"ItemInteractor Start: Axe Hitbox assigned. Is enabled by default? {playerAnimation.axeHitbox.enabled}");
            // Ensure it's off at start, this is a redundant check if you disable in Inspector, but good for debugging
            if (playerAnimation.axeHitbox.enabled)
            {
                playerAnimation.axeHitbox.enabled = false;
                //Debug.LogWarning("ItemInteractor Start: Axe Hitbox was ON by default, forcing OFF.");
            }
        }
        else
        {
            Debug.LogError("ItemInteractor Start: playerAnimation or axeHitbox reference is NULL. Check PlayerAnimation Inspector assignment!");
        }
    }
    private void Update()
    {
        Debug.DrawRay(playerCameraTransform.position, playerCameraTransform.forward * hitRange, Color.red); // Make sure this line is here

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
            Debug.Log($"ItemInteractor Update: Raycast successfully hit a ChoppableTree: {choppableTree.name}"); // NEW IMPORTANT LOG
            if (qKeyPressedThisFrame) 
            {
                Debug.Log($"ItemInteractor Update: Q pressed while raycasting a ChoppableTree.");
                Debug.Log($"   Conditions: playerInRange={choppableTree.playerInRange}, canBeChopped={choppableTree.canBeChopped}, axeIsActive={axeIsActive}");
                if (choppableTree.playerInRange && choppableTree.canBeChopped && !axeIsActive) 
                {
                    Debug.Log("ItemInteractor Update: All conditions met for chopping! Triggering attack and enabling axe.");

                    playerAnimation.TriggerChopAttack();
                   
                    EnableAxeColliderManual(); // Call the new method to activate
                    
                }
                else if (axeIsActive)
                {
                    Debug.Log("ItemInteractor Update: Attack skipped, axe already active.");
                }
                else if (!choppableTree.playerInRange)
                {
                    Debug.Log("ItemInteractor Update: Attack skipped, player not in range.");
                }
                else if (!choppableTree.canBeChopped)
                {
                    Debug.Log("ItemInteractor Update: Attack skipped, tree cannot be chopped (canBeChopped is false).");
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
        Debug.Log("EnableAxeColliderManual: Attempting to enable collider...");
        if (playerAnimation != null && playerAnimation.axeHitbox != null)
        {
            playerAnimation.axeHitbox.enabled = true;
            axeIsActive = true;
            Debug.Log("Axe collider MANUALLY ENABLED by Q press.");
            // Start a coroutine to disable it after a short duration
            StartCoroutine(DisableAxeColliderAfterDelay(axeActiveDuration));
        }
        else
        {
            Debug.LogError("ItemInteractor: Cannot enable axe collider. playerAnimation or playerAnimation.axeHitbox is null.");
        }
    }

    private IEnumerator DisableAxeColliderAfterDelay(float delay)
    {
        Debug.Log($"DisableAxeColliderAfterDelay: Waiting for {delay} seconds...");
        yield return new WaitForSeconds(delay);
        if (playerAnimation != null && playerAnimation.axeHitbox != null)
        {
            playerAnimation.axeHitbox.enabled = false;
            axeIsActive = false;
            Debug.Log("Axe collider MANUALLY DISABLED after delay.");
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

   
