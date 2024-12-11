using UnityEngine;

public class ToggleMultipleObjects : MonoBehaviour
{
    public GameObject[] objectsToAppear;
    public GameObject[] objectsToDisappear;

    public void ToggleObjectsVisibility()
    {
        // appear
        foreach (GameObject obj in objectsToAppear)
        {
            obj.SetActive(true);
        }

        // disappear
        foreach (GameObject obj in objectsToDisappear)
        {
            obj.SetActive(false);
        }
    }
}
