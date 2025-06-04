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

    private void Awake()
    {
        playerAnimation = GetComponent<PlayerAnimation>();
    }

    private void Update()
    {
       

        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out hit, hitRange, pickableLayerMask))
        {
            var selectionTransform = hit.transform;
            ChoppableTree choppableTree = selectionTransform.GetComponent<ChoppableTree>();
            if (choppableTree) 
            {
                if (choppableTree.playerInRange && Input.GetKeyDown(KeyCode.Q)) 
                {
                    playerAnimation.TriggerChopAttack();
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

   
