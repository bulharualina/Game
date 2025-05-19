using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;
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
    [Min(1)]
    private float hitRange = 3;

    private RaycastHit hit;

    
    private void Update()
    {
        //Debug.DrawRay(playerCameraTransform.position, playerCameraTransform.forward * hitRange, Color.red);
        if (hit.collider != null) 
        {
            pickUpUI?.SetActive(false);

        }
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out hit, hitRange, pickableLayerMask))
        {

            pickUpUI.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                PickUpItem(hit.collider.gameObject);
            }
        }
        else 
        {
            //pickUpUI.SetActive(false);
        }

    }
    private void PickUpItem(GameObject item)
    {
        PickableItem pickable = item.GetComponent<PickableItem>();

        if (pickable == null)
        {
            Debug.LogError("Picked item doesn't have PickableItem script attached.");
            return;
        }
        if (!InventorySystem.Instance.CheckIfInventoryFull())
        {
            InventorySystem.Instance.AddToInventory(pickable);

            pickUpUI?.SetActive(false);

            Destroy(item);

        }
        else {
            Debug.Log("Inventory is FULL");
        }
    }
}

   
