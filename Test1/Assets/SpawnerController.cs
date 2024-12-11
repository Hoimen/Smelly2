using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Alteruna;

public class CubeSpawner : MonoBehaviour
{
    public Alteruna.Avatar avatar;
    private Spawner _spawner;
    private InventoryManager inventoryManager; // Reference to the InventoryManager script

    [SerializeField] private float[] despawnTimes = new float[5];
    [SerializeField] private TextMeshProUGUI[] bulletCounters;
    [SerializeField] private int[] startingBulletCounts = new int[5];
    [SerializeField] private float[] cooldownTimes = new float[5];

    [SerializeField] private List<GameObject> settingsPanels; // List to hold multiple SettingsPanels

    private int[] bulletCounts;
    private float[] lastShotTime;

    [SerializeField] private GameObject objectToSpawnInFrontOf; // Object to spawn in front of
    [SerializeField] private float spawnDistance = 2f; // Distance behind the object to spawn the cube
    [SerializeField] private float spawnHeight = 1f; // Height variation for the spawn position (positive is above, negative is below)

    private void Awake()
    {
        _spawner = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<Spawner>();
        inventoryManager = FindObjectOfType<InventoryManager>(); // Find the InventoryManager in the scene

        bulletCounts = new int[startingBulletCounts.Length];
        lastShotTime = new float[cooldownTimes.Length];

        for (int i = 0; i < bulletCounts.Length; i++)
        {
            bulletCounts[i] = startingBulletCounts[i];
            lastShotTime[i] = -cooldownTimes[i];
            UpdateCounterText(i);
        }

        // Ensure all settings panels start as invisible
        foreach (GameObject panel in settingsPanels)
        {
            if (panel != null)
            {
                panel.SetActive(false); // Disable all panels at the start
            }
        }
    }

    private void Update()
    {
        if (!avatar.IsMe) return;

        // Prevent shooting if any SettingsPanel is visible
        foreach (GameObject panel in settingsPanels)
        {
            if (panel != null && panel.activeSelf)
            {
                return; // If any panel is visible, prevent shooting
            }
        }

        // Get the selected item's index from the InventoryManager
        int selectedItemIndex = inventoryManager.GetSelectedItemIndex();

        if (selectedItemIndex != -1 && Input.GetKeyDown(KeyCode.Mouse0)) // Mouse click to shoot
        {
            if (bulletCounts[selectedItemIndex] > 0 && Time.time >= lastShotTime[selectedItemIndex] + cooldownTimes[selectedItemIndex])
            {
                SpawnCube(selectedItemIndex);
                bulletCounts[selectedItemIndex]--;
                lastShotTime[selectedItemIndex] = Time.time; // Record the time of the shot
                UpdateCounterText(selectedItemIndex);
            }
        }
    }

    private void SpawnCube(int indexToSpawn)
    {
        if (objectToSpawnInFrontOf == null)
        {
            Debug.LogWarning("No object assigned to spawn in front of.");
            return;
        }

        // Calculate the spawn position behind the object
        Vector3 spawnPosition = objectToSpawnInFrontOf.transform.position
                               - objectToSpawnInFrontOf.transform.forward * spawnDistance
                               + new Vector3(0, spawnHeight, 0); // Adjust height

        // Spawn the cube at the calculated position with the object's rotation
        GameObject cube = _spawner.Spawn(indexToSpawn, spawnPosition, objectToSpawnInFrontOf.transform.rotation, Vector3.one);
        StartCoroutine(DespawnCubeAfterTime(cube, despawnTimes[indexToSpawn]));
    }

    private IEnumerator DespawnCubeAfterTime(GameObject cube, float time)
    {
        yield return new WaitForSeconds(time);
        _spawner.Despawn(cube);
    }

    private void UpdateCounterText(int index)
    {
        if (index >= 0 && index < bulletCounters.Length)
        {
            bulletCounters[index].text = "" + bulletCounts[index];
        }
    }
}
