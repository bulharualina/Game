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
            if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out hit, hitRange,pickableLayerMask))
            {

                pickUpUI.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    PickUpItem(hit.collider.gameObject);
                }
            }

    }
    private void PickUpItem(GameObject item)
    {

        if (!InventorySystem.Instance.CheckIfInventoryFull())
        {
            string cleanName = item.name.Replace("(Clone)", "").Trim();
            InventorySystem.Instance.AddToInventory(cleanName);

            pickUpUI?.SetActive(false);

            Destroy(item);

        }
        else {
            Debug.Log("Inventory is FULL");
        }
    }
}

   
