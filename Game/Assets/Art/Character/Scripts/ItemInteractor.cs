using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField]
    private TextMeshProUGUI pickUpUIText;
    private PlayerAnimation playerAnimation;


    [SerializeField]
    [Min(1)]
    private float hitRange = 3;

    private RaycastHit hit;
   

    [SerializeField] private float axeActiveDuration = 0.2f; // How long the axe stays active after Q press
    private bool axeIsActive = false;
    private void Awake()
    {
        playerAnimation = GetComponent<PlayerAnimation>();
        if (pickUpUIText == null && pickUpUI != null)
        {
            pickUpUIText = pickUpUI.GetComponentInChildren<TextMeshProUGUI>();
            if (pickUpUIText == null)
            {
                Debug.LogError("ItemInteractor: pickUpUIText not assigned and could not be found in children of PickUpUI!");
            }
        }
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

        if (pickUpUI != null)
        {
            pickUpUI.SetActive(false);
        }
    }
    private void Update()
    {
     
        bool qKeyPressedThisFrame = Input.GetKeyDown(KeyCode.Q);
        bool raycastHitSomething = Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out hit, hitRange, pickableLayerMask);
       
        
        if (raycastHitSomething)
        {
            var selectionTransform = hit.transform;
            ChoppableTree choppableTree = selectionTransform.GetComponent<ChoppableTree>();
            PickableItem pickableItem = selectionTransform.GetComponent<PickableItem>();
            Animal animal = selectionTransform.GetComponent<Animal>();
            
            bool interacted = false; // Flag to know if we handled an interaction

            if (choppableTree != null) // If we hit a tree
            {
                // Update UI text for chopping
                if (pickUpUIText != null)
                {
                    pickUpUIText.text = "Press [Q] to chop tree";
                }
                pickUpUI.SetActive(true); // Show UI
                choppableTree.canBeChopped = true; // Ensure canBeChopped is true when looking at it.

                if (qKeyPressedThisFrame)
                {
                    if (choppableTree.playerInRange && !axeIsActive) // playerInRange handled by ChoppableTree's own OnTriggerEnter
                    {
                        playerAnimation.TriggerChopAttack();
                        EnableAxeColliderManual();
                        interacted = true;
                    }
                }
            }
            else if (pickableItem != null) // If we hit a pickable item
            {
                // Update UI text for picking up
                if (pickUpUIText != null)
                {
                    pickUpUIText.text = "Press [E] to pick up";
                }
                pickUpUI.SetActive(true); // Show UI

                if (Input.GetKeyDown(KeyCode.E))
                {
                    PickUpItem(hit.collider.gameObject);
                    interacted = true;
                }
            }
            else if (animal != null) // This is the block we care about
            {
                Debug.Log("Raycast hit an Animal!"); // ADD THIS
                if (pickUpUIText != null)
                {
                    pickUpUIText.text = "Press [Q] to kill the " + animal.animalName;
                }
                pickUpUI.SetActive(true);

                if (qKeyPressedThisFrame)
                {
                    Debug.Log("Q pressed and raycast hit Animal!"); // ADD THIS
                    if (animal.playerInRange && !axeIsActive)
                    {
                        Debug.Log("Conditions met: Player in range and axe not active. Dealing damage!"); // ADD THIS
                                                                                                          // playerAnimation.TriggerChopAttack(); // UNCOMMENT THIS IF YOU WANT ANIMATION
                        StartCoroutine(DealDamageTo(animal, 0.3f, EquipSystem.Instance.GetWeaponDamage()));
                        interacted = true;
                    }
                    else
                    {
                        Debug.Log($"Conditions NOT met. playerInRange: {animal.playerInRange}, axeIsActive: {axeIsActive}"); // ADD THIS
                    }
                }
            }
            else // Hit something on pickableLayerMask but it's neither a tree nor a pickable item
            {
                // Hide UI if it's not a recognized interactive object
                if (pickUpUI != null && pickUpUI.activeSelf)
                {
                    pickUpUI.SetActive(false);
                }
            }

            // If we interacted this frame, or if the UI was shown, keep it active
            // This ensures the UI stays visible for a moment even if no interaction happens yet
            if (pickUpUI != null && !interacted && (choppableTree != null || pickableItem != null))
            {
                pickUpUI.SetActive(true);
            }
            else if (pickUpUI != null && !interacted && choppableTree == null && pickableItem == null)
            {
                pickUpUI.SetActive(false); // Hide if nothing interactable is hit
            }

        }
        else // No raycast hit anything on the pickable layer mask
        {
            if (pickUpUI != null && pickUpUI.activeSelf)
            {
                pickUpUI.SetActive(false);
            }
        }


    }

    private IEnumerator DealDamageTo(Animal animal, float delay, int damage)
    {
        yield return new WaitForSeconds(delay);
        animal.TakeDamage(damage);
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

   
