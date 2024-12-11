using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public GameObject[] inventorySlots; // 4 hotbar slots
    public GameObject[] items; // 4 items corresponding to Y-P keys
    public GameObject[] gunDisplays; // 4 corresponding 3D gun display prefabs
    public GameObject[] clothingImages; // Existing array for clothing images (first set)
    public GameObject[] bodyTypeImages; // New array for body type images
    public Image[] slotBorders; // UI images to show selected slot
    public GameObject deleteConfirmationPanel; // The confirmation panel for item deletion
    public int startingItem; // Index of the item the player starts with (set in Inspector)

    private int[] itemIndicesInSlots = new int[4]; // Tracks which item is in each slot (-1 means empty)
    private int selectedSlot = 0; // Keeps track of currently selected slot, starts with 0 for first slot
    private bool isDeletePanelVisible = false; // Tracks if the delete confirmation panel is visible
    private Alteruna.Avatar _avatar; // For multiplayer player identification

    // Add an array for the clothing buttons to match up with clothing images
    public Button[] clothingButtons; // UI buttons to trigger clothing display
    public Button[] bodyTypeButtons; // UI buttons to trigger body type display

    private void Start()
    {
        _avatar = GetComponent<Alteruna.Avatar>();

        // Only initialize if this is the local player
        if (!_avatar.IsMe)
            return;

        // Initialize hotbar slots to be empty (-1)
        for (int i = 0; i < itemIndicesInSlots.Length; i++)
        {
            itemIndicesInSlots[i] = -1;
        }

        // Spawn the starting item
        if (startingItem >= 0 && startingItem < items.Length)
        {
            SpawnItem(startingItem);
        }

        // Hide all gun displays and clothing images initially
        for (int i = 0; i < gunDisplays.Length; i++)
        {
            gunDisplays[i].SetActive(false);
        }
        for (int i = 0; i < clothingImages.Length; i++)
        {
            clothingImages[i].SetActive(false);
        }
        for (int i = 0; i < bodyTypeImages.Length; i++)
        {
            bodyTypeImages[i].SetActive(false);
        }

        // Set up button listeners for clothing selection
        for (int i = 0; i < clothingButtons.Length; i++)
        {
            int index = i;  // Local copy of the index for the button listener
            clothingButtons[i].onClick.AddListener(() => ShowClothing(index));
        }

        // Set up button listeners for body type selection
        for (int i = 0; i < bodyTypeButtons.Length; i++)
        {
            int index = i;  // Local copy of the index for the button listener
            bodyTypeButtons[i].onClick.AddListener(() => ShowBodyType(index));
        }

        // Automatically select the first slot at the start
        SelectSlot(0); // Select slot 0 (the first slot)

        // Hide the delete confirmation panel initially
        deleteConfirmationPanel.SetActive(false);
    }

    private void Update()
    {
        // Only the local player can use the inventory
        if (!_avatar.IsMe)
            return;

        // Handle slot selection (1-4) and other inputs
        if (!isDeletePanelVisible)
        {
            // Deselect all slots when "F" is pressed
            if (Input.GetKeyDown(KeyCode.F))
            {
                DeselectAllSlots();
            }

            // Slot selection (1-4) resumes normal behavior after "F"
            for (int i = 0; i < 4; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    SelectSlot(i);
                    break;
                }
            }

            // Handle item spawning with Y, U, I, O
            if (Input.GetKeyDown(KeyCode.Y)) SpawnItem(0);
            else if (Input.GetKeyDown(KeyCode.U)) SpawnItem(1);
            else if (Input.GetKeyDown(KeyCode.I)) SpawnItem(2);
            else if (Input.GetKeyDown(KeyCode.O)) SpawnItem(3);

            // Show delete confirmation panel when "Q" is pressed
            if (Input.GetKeyDown(KeyCode.Q) && selectedSlot != -1 && itemIndicesInSlots[selectedSlot] != -1)
            {
                ShowDeleteConfirmation();
            }
        }
        else
        {
            // Handle panel interaction: "Q" to confirm deletion, other keys to cancel
            if (Input.GetKeyDown(KeyCode.Q))
            {
                DeleteSelectedItem();
            }
            else if (Input.anyKeyDown)
            {
                HideDeleteConfirmation();
            }
        }
    }

    // Show the delete confirmation panel
    private void ShowDeleteConfirmation()
    {
        deleteConfirmationPanel.SetActive(true);
        isDeletePanelVisible = true;
    }

    // Hide the delete confirmation panel
    private void HideDeleteConfirmation()
    {
        deleteConfirmationPanel.SetActive(false);
        isDeletePanelVisible = false;
    }

    // Delete the selected item
    private void DeleteSelectedItem()
    {
        if (selectedSlot != -1 && itemIndicesInSlots[selectedSlot] != -1)
        {
            // Get the item index from the selected slot
            int itemIndex = itemIndicesInSlots[selectedSlot];

            // Destroy the item in the selected slot
            Destroy(inventorySlots[selectedSlot].transform.GetChild(0).gameObject);

            // Hide the corresponding gun display
            gunDisplays[itemIndex].SetActive(false);

            // Mark the slot as empty
            itemIndicesInSlots[selectedSlot] = -1;

            // Hide the delete confirmation panel
            HideDeleteConfirmation();

            Debug.Log("Item deleted from slot " + (selectedSlot + 1));
        }
    }

    // Deselect all slots and hide clothing items
    private void DeselectAllSlots()
    {
        // Disable all slot borders and deselect the current slot
        for (int i = 0; i < slotBorders.Length; i++)
        {
            slotBorders[i].enabled = false;
        }
        selectedSlot = -1;

        // Hide all gun displays and clothing images
        HideAllGunDisplays();
        HideAllClothingImages();

        Debug.Log("All slots deselected.");
    }

    // Hide all gun displays
    private void HideAllGunDisplays()
    {
        foreach (var gunDisplay in gunDisplays)
        {
            gunDisplay.SetActive(false);
        }
    }

    // Hide all clothing images
    private void HideAllClothingImages()
    {
        foreach (var clothingImage in clothingImages)
        {
            clothingImage.SetActive(false);
        }

        // Hide the body type images as well
        foreach (var bodyTypeImage in bodyTypeImages)
        {
            bodyTypeImage.SetActive(false);
        }
    }

    // Spawn an item into the inventory
    private void SpawnItem(int itemIndex)
    {
        if (!_avatar.IsMe)
            return;

        // Check if the item is already in a slot
        for (int i = 0; i < itemIndicesInSlots.Length; i++)
        {
            if (itemIndicesInSlots[i] == itemIndex)
            {
                Debug.Log($"Item {itemIndex + 1} is already in slot {i + 1}.");
                return; // Item already exists, no need to spawn
            }
        }

        // Find the first empty slot
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].transform.childCount == 0)
            {
                // Instantiate the item in the slot
                GameObject spawnedItem = Instantiate(items[itemIndex], inventorySlots[i].transform);
                spawnedItem.transform.localPosition = Vector3.zero;

                itemIndicesInSlots[i] = itemIndex; // Track which item was placed in the slot

                Debug.Log($"Item {itemIndex + 1} spawned in slot {i + 1}.");

                // If the spawned item is in the currently selected slot, show its gun display
                if (i == selectedSlot)
                {
                    gunDisplays[itemIndex].SetActive(true); // Show the corresponding gun display
                }

                return;
            }
        }

        Debug.Log("All slots are filled.");
    }

    // Select a specific slot
    private void SelectSlot(int slotIndex)
    {
        if (!_avatar.IsMe)
            return;

        if (slotIndex < 0 || slotIndex >= slotBorders.Length)
        {
            Debug.LogError("Slot index out of range.");
            return;
        }

        // Disable all slot borders
        for (int i = 0; i < slotBorders.Length; i++)
        {
            slotBorders[i].enabled = false;

            // Hide the gun display for each unselected slot
            if (i != slotIndex && itemIndicesInSlots[i] != -1)
            {
                gunDisplays[itemIndicesInSlots[i]].SetActive(false);
            }
        }

        // Enable the border for the selected slot
        slotBorders[slotIndex].enabled = true;
        selectedSlot = slotIndex;

        // Check if the selected slot has an item
        int itemIndex = itemIndicesInSlots[slotIndex];

        // Show the gun display for the selected slot if the item exists
        if (itemIndex != -1)
        {
            gunDisplays[itemIndex].SetActive(true); // Activate the corresponding gun display
            Debug.Log($"Slot {slotIndex + 1} selected. Showing gun display for item {itemIndex + 1}.");
        }
        else
        {
            Debug.Log($"Slot {slotIndex + 1} selected, but no item is present.");
        }
    }

    // Show a specific clothing item based on button press
    private void ShowClothing(int index)
    {
        // Hide all clothing items first
        HideAllClothingImages();

        // If the index is valid, show the clothing image
        if (index >= 0 && index < clothingImages.Length)
        {
            clothingImages[index].SetActive(true);
            Debug.Log($"Clothing {index + 1} is now visible.");
        }
    }

    // Show a specific body type image based on button press
    private void ShowBodyType(int index)
    {
        // Hide all body type images first
        HideAllClothingImages();

        // If the index is valid, show the body type image
        if (index >= 0 && index < bodyTypeImages.Length)
        {
            bodyTypeImages[index].SetActive(true);
            Debug.Log($"Body type {index + 1} is now visible.");
        }
    }

    public int GetSelectedItemIndex()
    {
        if (!_avatar.IsMe)
            return -1; // Prevent access to non-local players

        if (selectedSlot != -1 && itemIndicesInSlots[selectedSlot] != -1)
        {
            return itemIndicesInSlots[selectedSlot]; // Return the index of the selected item
        }
        return -1; // No item selected
    }
}
