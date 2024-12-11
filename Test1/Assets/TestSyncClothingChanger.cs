using UnityEngine;

public class GunDisplayManager : MonoBehaviour
{
    public GameObject[] gunDisplays; // Array of gun displays
    public int gunDisplayIndex = 0; // Index of the gun display to show (default is 0)

    void Update()
    {
        // Check for Z key press to show the gun display
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (gunDisplays.Length > gunDisplayIndex)
            {
                gunDisplays[gunDisplayIndex].SetActive(true); // Show the gun display
                Debug.Log("Gun display " + (gunDisplayIndex + 1) + " is now visible.");
            }
        }
    }
}
