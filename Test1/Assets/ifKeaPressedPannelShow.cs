using UnityEngine;

public class IfKeaPressedPannelShow : MonoBehaviour
{
    public GameObject[] objectsToActivate; // Array of GameObjects to activate
    public GameObject[] objectsToHide; // Array of GameObjects to hide
    public GameObject[] objectsToToggle; // Array of GameObjects to toggle on/off
    public KeyCode[] activationKeys; // Array of keys to press for activation

    void Update()
    {
        foreach (KeyCode key in activationKeys)
        {
            if (Input.GetKeyDown(key))
            {
                // Activate objects
                foreach (GameObject obj in objectsToActivate)
                {
                    obj.SetActive(true);
                }

                // Hide objects
                foreach (GameObject obj in objectsToHide)
                {
                    obj.SetActive(false);
                }

                // Toggle objects
                foreach (GameObject obj in objectsToToggle)
                {
                    obj.SetActive(!obj.activeSelf);
                }

                // Exit the loop after the first key press to avoid triggering multiple times
                break;
            }
        }
    }
}
