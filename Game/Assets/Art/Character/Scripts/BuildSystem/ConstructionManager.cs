using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionManager : MonoBehaviour
{
    public static ConstructionManager Instance { get; set; }
    public List<GameObject> placedBuildings = new List<GameObject>(); 
    public GameObject itemToBeConstructed;
    public bool inConstructionMode = false;
    public GameObject constructionHoldingSpot;

    public bool isValidPlacement;

    public bool selectingAGhost;
    public GameObject selectedGhost;

    // Materials we store as refereces for the ghosts
    public Material ghostSelectedMat;
    public Material ghostSemiTransparentMat;
    public Material ghostFullTransparentMat;

    // We keep a reference to all ghosts currently in our world,
    // so the manager can monitor them for various operations
    public List<GameObject> allGhostsInExistence = new List<GameObject>();
    public GameObject itemToBeDestroyed;
    //[SerializeField] private float placementOffsetY = 0.05f; // Small offset above ground when placing
    //  Parameters for screen positioning of the ghost
    [SerializeField] private float ghostScreenXPosition = 0.75f; // 0.0 is left, 1.0 is right
    [SerializeField] private float ghostScreenYPosition = 0.5f;  // 0.0 is bottom, 1.0 is top
    [SerializeField] private float ghostWorldDistance = 5.0f;

    public GameObject constructionUI;
    public GameObject player;

    public Transform buildingsParent;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
  

    public void ActivateConstructionPlacement(string itemToConstruct)
    {
        GameObject item = Instantiate(Resources.Load<GameObject>(itemToConstruct));

        //change the name of the gameobject so it will not be (clone)
        item.name = itemToConstruct;

        item.transform.SetParent(constructionHoldingSpot.transform, false);
        itemToBeConstructed = item;
        itemToBeConstructed.gameObject.tag = "activeConstructable";

        // Disabling the non-trigger collider so our mouse can cast a ray
        itemToBeConstructed.GetComponent<Constructable>().solidCollider.enabled = false;

        // Actiavting Construction mode
        inConstructionMode = true;
        selectedGhost = null;
        selectingAGhost = false;
    }

    private void GetAllGhosts(GameObject itemToBeConstructed)
    {
        List<GameObject> ghostlist = itemToBeConstructed.gameObject.GetComponent<Constructable>().ghostList;

        foreach (GameObject ghost in ghostlist)
        {
            Debug.Log(ghost);
            allGhostsInExistence.Add(ghost);
        }
    }

    private void PerformGhostDeletionScan()
    {

        foreach (GameObject ghost in allGhostsInExistence)
        {
            if (ghost != null)
            {
                if (ghost.GetComponent<GhostItem>().hasSamePosition == false) // if we did not already add a flag
                {
                    foreach (GameObject ghostX in allGhostsInExistence)
                    {
                        // First we check that it is not the same object
                        if (ghost.gameObject != ghostX.gameObject)
                        {
                            // If its not the same object but they have the same position
                            if (XPositionToAccurateFloat(ghost) == XPositionToAccurateFloat(ghostX) && ZPositionToAccurateFloat(ghost) == ZPositionToAccurateFloat(ghostX))
                            {
                                if (ghost != null && ghostX != null)
                                {
                                    // setting the flag
                                    ghostX.GetComponent<GhostItem>().hasSamePosition = true;
                                    break;
                                }

                            }

                        }

                    }

                }
            }
        }

        foreach (GameObject ghost in allGhostsInExistence)
        {
            if (ghost != null)
            {
                if (ghost.GetComponent<GhostItem>().hasSamePosition)
                {
                    DestroyImmediate(ghost);
                }
            }

        }
    }

    private float XPositionToAccurateFloat(GameObject ghost)
    {
        if (ghost != null)
        {
            // Turning the position to a 2 decimal rounded float
            Vector3 targetPosition = ghost.gameObject.transform.position;
            float pos = targetPosition.x;
            float xFloat = Mathf.Round(pos * 100f) / 100f;
            return xFloat;
        }
        return 0;
    }

    private float ZPositionToAccurateFloat(GameObject ghost)
    {

        if (ghost != null)
        {
            // Turning the position to a 2 decimal rounded float
            Vector3 targetPosition = ghost.gameObject.transform.position;
            float pos = targetPosition.z;
            float zFloat = Mathf.Round(pos * 100f) / 100f;
            return zFloat;

        }
        return 0;
    }

    private void Update()
    {
        if (inConstructionMode)
        {
            constructionUI.SetActive(true);
        }
        else 
        {
            constructionUI.SetActive(false);
        }

        if (itemToBeConstructed != null && inConstructionMode)
        {
            if (itemToBeConstructed.name == "FoundationModel")
            {
                if (CheckValidConstructionPosition())
                {
                    isValidPlacement = true;
                    itemToBeConstructed.GetComponent<Constructable>().SetValidColor();
                }
                else
                {
                    isValidPlacement = false;
                    itemToBeConstructed.GetComponent<Constructable>().SetInvalidColor();
                }
            }


            //Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~0))
            {
                var selectionTransform = hit.transform;
                if (selectionTransform.gameObject.CompareTag("Ghost") && itemToBeConstructed.name == "FoundationModel")
                {
                    itemToBeConstructed.SetActive(false);
                    selectingAGhost = true;
                    selectedGhost = selectionTransform.gameObject;
                }
                else if (selectionTransform.gameObject.CompareTag("wallGhost") && itemToBeConstructed.name == "WallModel")
                {
                    itemToBeConstructed.SetActive(false);
                    selectingAGhost = true;
                    selectedGhost = selectionTransform.gameObject;

                } 
                else if (selectionTransform.gameObject.CompareTag("ceilingGhost") && itemToBeConstructed.name == "CeilingModel")
                {
                    itemToBeConstructed.SetActive(false);
                    selectingAGhost = true;
                    selectedGhost = selectionTransform.gameObject;
                }
                else
                {
                    itemToBeConstructed.SetActive(true);
                    selectedGhost = null;
                    selectingAGhost = false;
                }

                Vector3 viewportPoint = new Vector3(ghostScreenXPosition, ghostScreenYPosition, ghostWorldDistance);
                Vector3 worldPositionForGhost = Camera.main.ViewportToWorldPoint(viewportPoint);
                itemToBeConstructed.transform.position = worldPositionForGhost;

            }
        }

        // Left Mouse Click to Place item
        if (Input.GetMouseButtonDown(0) && inConstructionMode)
        {
            if (isValidPlacement && selectedGhost == false && itemToBeConstructed.name == "FoundationModel") // We don't want the freestyle to be triggered when we select a ghost.
            {
                PlaceItemFreeStyle();
                DestroyItem(itemToBeDestroyed);
            }

            if (selectingAGhost)
            {
                PlaceItemInGhostPosition(selectedGhost);
                DestroyItem(itemToBeDestroyed);
            }
        }
        // Right Mouse Click to Cancel                      
        if (Input.GetKeyDown(KeyCode.X))
        {    
            itemToBeDestroyed.SetActive(true);
            itemToBeDestroyed = null;
            DestroyItem(itemToBeConstructed);
            itemToBeConstructed = null;
            inConstructionMode = false;

        }
    }

    private void PlaceItemInGhostPosition(GameObject copyOfGhost)
    {

        Vector3 ghostPosition = copyOfGhost.transform.position;
        Quaternion ghostRotation = copyOfGhost.transform.rotation;
      

        selectedGhost.gameObject.SetActive(false);

        // Setting the item to be active again (after we disabled it in the ray cast)
        itemToBeConstructed.gameObject.SetActive(true);


        // Setting the parent to be the root of our scene
        itemToBeConstructed.transform.SetParent(buildingsParent, true);
        //var offsetRand = UnityEngine.Random.Range(0.01f,0.03f);

        //itemToBeConstructed.transform.position = new Vector3(ghostPosition.x, ghostPosition.y, ghostPosition.z + offsetRand);
        itemToBeConstructed.transform.position = ghostPosition;
        itemToBeConstructed.transform.rotation = ghostRotation;

       


        // Enabling back the solider collider that we disabled earlier
        itemToBeConstructed.GetComponent<Constructable>().solidCollider.enabled = true;
        itemToBeConstructed.GetComponent<Constructable>().SetDefaultColor();

        

        if (itemToBeConstructed.name == "FoundationModel")
        {
            itemToBeConstructed.GetComponent<Constructable>().ExtractGhostMembers();

            itemToBeConstructed.tag = "placedFoundation";
            GetAllGhosts(itemToBeConstructed);
            PerformGhostDeletionScan();
        }
        else if (itemToBeConstructed.name == "CeilingModel")
        {

            itemToBeConstructed.GetComponent<Constructable>().ExtractGhostMembers();

            itemToBeConstructed.tag = "placedCeiling";
            GetAllGhosts(itemToBeConstructed);
            PerformGhostDeletionScan();
        }
        else
        {
            itemToBeConstructed.tag = "placedWall";
            DestroyItem(selectedGhost);
        }

        

        placedBuildings.Add(itemToBeConstructed);
    

        itemToBeConstructed = null;

        inConstructionMode = false;
    }

    
    private void PlaceItemFreeStyle()
    {
        // Setting the parent to be the root of our scene
        itemToBeConstructed.transform.SetParent(buildingsParent, true);

        // Making the Ghost Children to no longer be children of this item
        itemToBeConstructed.GetComponent<Constructable>().ExtractGhostMembers();
        // Setting the default color/material
        itemToBeConstructed.GetComponent<Constructable>().SetDefaultColor();
        itemToBeConstructed.tag = "placedFoundation";
        itemToBeConstructed.GetComponent<Constructable>().enabled = false;
        // Enabling back the solider collider that we disabled earlier
        itemToBeConstructed.GetComponent<Constructable>().solidCollider.enabled = true;

        //Adding all the ghosts of this item into the manager's ghost bank
        GetAllGhosts(itemToBeConstructed);
        PerformGhostDeletionScan();

        placedBuildings.Add(itemToBeConstructed);
     
        itemToBeConstructed = null;

        inConstructionMode = false;
    }

    void DestroyItem(GameObject item)
    {
        DestroyImmediate(item);
        InventorySystem.Instance.ReCalculateList();
        CraftingSystem.Instance.RefreshNeededItems();
    }
    private bool CheckValidConstructionPosition()
    {
        if (itemToBeConstructed != null)
        {
            return itemToBeConstructed.GetComponent<Constructable>().isValidToBeBuilt;
        }

        return false;
    }
    public void PlaceBuilding(GameObject buildingPrefab, Vector3 position, Quaternion rotation)
    {
        GameObject newBuilding = Instantiate(buildingPrefab, position, rotation);
        newBuilding.name = buildingPrefab.name.Replace("(Clone)", "");
        placedBuildings.Add(newBuilding);
        newBuilding.tag = "PlacedBuilding";
    }

}

