using UnityEngine;

public class CameraZoomAim : MonoBehaviour
{
    public GameObject sniperGun;  // Reference to the Sniper Gun object
    public GameObject sniperUI;   // Reference to the UI/GameObject to show when Sniper Gun is zooming
    public float zoomedFOV = 30f;
    public float normalFOV = 60f;
    public float zoomSpeed = 5f;
    public float sniperZoomBonus = 10f;  // Extra zoom for Sniper Gun

    private bool isZooming = false;
    private Camera mainCamera;

    void Start()
    {
        // Find the main camera dynamically
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (mainCamera == null) return;  // Make sure we have a camera

        // Check if the Sniper Gun is visible
        if (sniperGun.activeInHierarchy)
        {
            if (Input.GetMouseButtonDown(1))  // Right mouse button down
            {
                isZooming = true;
            }
            if (Input.GetMouseButtonUp(1))  // Right mouse button released
            {
                isZooming = false;
            }

            // Show UI only while zooming and Sniper Gun is visible
            if (isZooming)
            {
                sniperUI.SetActive(true);

                // Add extra zoom if Sniper Gun is visible
                float sniperZoomedFOV = zoomedFOV - sniperZoomBonus;
                mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, sniperZoomedFOV, Time.deltaTime * zoomSpeed);
            }
            else
            {
                sniperUI.SetActive(false);
                mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, normalFOV, Time.deltaTime * zoomSpeed);
            }
        }
        else
        {
            // If Sniper Gun is not visible, hide the UI/GameObject and revert to normal zoom behavior
            sniperUI.SetActive(false);

            if (Input.GetMouseButtonDown(1))  // Right mouse button down
            {
                isZooming = true;
            }
            if (Input.GetMouseButtonUp(1))  // Right mouse button released
            {
                isZooming = false;
            }

            // Normal zoom behavior without the sniper bonus
            if (isZooming)
            {
                mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, zoomedFOV, Time.deltaTime * zoomSpeed);
            }
            else
            {
                mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, normalFOV, Time.deltaTime * zoomSpeed);
            }
        }
    }
}
