using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    [Header("Base setup")]
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float lookSpeed = 2.0f; // i dont think this dose anything
    public float lookXLimit = 45.0f;

    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private float rotationY = 0;

    [HideInInspector]
    public bool canMove = true;
    public bool canRotate = false;

    [SerializeField]
    private float cameraYOffset = 0.4f;
    [SerializeField]
    private float cameraXOffset = 0.0f;
    [SerializeField]
    private float cameraZOffset = -2.0f;

    private Camera playerCamera;

    [Header("Camera for Bobbing")]
    public Camera bobbingCamera;

    private Alteruna.Avatar _avatar;

    public Button lockCursorButton;
    public GameObject[] settingsPanels;

    public float bobbingAmount = 0.05f;
    public float bobbingSpeed = 14.0f;
    private float bobbingTimer = 0.0f;
    private Vector3 initialBobbingCameraPosition;
    private float currentBobbingOffset = 0.0f;
    private float bobbingLerpSpeed = 5.0f;

    [Header("Object Visibility")]
    public GameObject objectToHide;
    public GameObject objectToShow;

    public int counter = 0;
    public TextMeshProUGUI counterText;

    [Header("Camera Rotation")]
    [SerializeField]
    private Transform centralObject;
    [SerializeField]
    private float rotationSpeed = 3.0f;

    [Header("Camera Zoom")]
    public float zoomSpeed = 2.0f;
    public float startFOV = 60f;

    [Header("Visual Rotation")]
    public GameObject visualPlayer;
    public float visualRotationSpeed = 5f;

    private Vector3 lastMoveDirection;

    private float targetFOV;

    void Start()
    {
        _avatar = GetComponent<Alteruna.Avatar>();

        if (!_avatar.IsMe)
            return;

        if (objectToHide != null)
        {
            objectToHide.SetActive(false);
        }

        if (objectToShow != null)
        {
            objectToShow.SetActive(true);
        }

        characterController = GetComponent<CharacterController>();
        playerCamera = Camera.main;

        if (bobbingCamera == null)
        {
            bobbingCamera = playerCamera;
        }

        playerCamera.transform.position = new Vector3(transform.position.x + cameraXOffset, transform.position.y + cameraYOffset, transform.position.z + cameraZOffset);
        playerCamera.transform.SetParent(transform);

        initialBobbingCameraPosition = bobbingCamera.transform.localPosition;

        UnlockCursor();

        if (lockCursorButton != null)
        {
            lockCursorButton.onClick.AddListener(UnlockCursor);
        }

        UpdateCounterText();

        lastMoveDirection = Vector3.zero;

        // Set the FOV to the starting value
        targetFOV = startFOV;
        ApplyFOVToAllCameras();
    }

    void Update()
    {
        if (!_avatar.IsMe)
            return;

        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        bool anyPanelActive = false;
        foreach (GameObject panel in settingsPanels)
        {
            if (panel != null && panel.activeSelf)
            {
                anyPanelActive = true;
                break;
            }
        }

        if (anyPanelActive)
        {
            UnlockCursor();
        }
        else if (Input.GetMouseButton(1))
        {
            LockCursor();
        }
        else
        {
            UnlockCursor();
        }

        HandleMovement(isRunning);

        if (canRotate && Input.GetMouseButton(1))
        {
            HandleCameraRotation();
        }

        HandleCameraZoom();

        TrackMovementDirection();
        HandleVisualRotation();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnlockCursor();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!_avatar.IsMe)
            return;

        if (other.CompareTag("RatOil"))
        {
            counter++;
            UpdateCounterText();
        }
    }

    void UpdateCounterText()
    {
        if (counterText != null)
        {
            counterText.text = "x" + counter;
        }
    }

    private void HandleMovement(bool isRunning)
    {
        if (!canMove)
            return;

        Vector3 forward = centralObject.forward;
        Vector3 right = centralObject.right;

        float curSpeedX = (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical");
        float curSpeedY = (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal");
        float movementDirectionY = moveDirection.y;

        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (characterController.isGrounded && Input.GetButton("Jump"))
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (characterController.velocity.magnitude > 0 && characterController.isGrounded)
        {
            bobbingTimer += Time.deltaTime * bobbingSpeed;
            currentBobbingOffset = Mathf.Sin(bobbingTimer) * bobbingAmount;
        }
        else
        {
            currentBobbingOffset = Mathf.Lerp(currentBobbingOffset, 0.0f, Time.deltaTime * bobbingLerpSpeed);
        }

        bobbingCamera.transform.localPosition = new Vector3(initialBobbingCameraPosition.x, initialBobbingCameraPosition.y + currentBobbingOffset, initialBobbingCameraPosition.z);
    }

    private void HandleCameraRotation()
    {
        if (playerCamera == null || centralObject == null)
            return;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotationY += mouseX * rotationSpeed;
        rotationX -= mouseY * rotationSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        centralObject.rotation = Quaternion.Euler(rotationX, rotationY, 0);
    }

    private void HandleCameraZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0)
        {
            targetFOV -= Mathf.RoundToInt(scrollInput * zoomSpeed * 10f);
            targetFOV = Mathf.Clamp(targetFOV, 12, 80);
            ApplyFOVToAllCameras();
        }
    }

    private void ApplyFOVToAllCameras()
    {
        Camera[] cameras = GameObject.FindGameObjectsWithTag("MainCamera").Select(go => go.GetComponent<Camera>()).ToArray();
        foreach (Camera cam in cameras)
        {
            if (cam != null)
            {
                cam.fieldOfView = targetFOV;
            }
        }
    }

    private void TrackMovementDirection()
    {
        Quaternion centralRotation = Quaternion.Euler(0, centralObject.transform.eulerAngles.y, 0);
        Vector3 localMoveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 worldMoveDirection = centralRotation * localMoveDirection;

        if (worldMoveDirection.magnitude > 0.1f)
        {
            lastMoveDirection = worldMoveDirection.normalized;
        }
    }

    private void HandleVisualRotation()
    {
        if (visualPlayer == null || lastMoveDirection == Vector3.zero)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(-lastMoveDirection); // Flipped rotation
        visualPlayer.transform.rotation = Quaternion.Slerp(visualPlayer.transform.rotation, targetRotation, Time.deltaTime * visualRotationSpeed);
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        canMove = true;
        canRotate = true;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        canMove = true;
        canRotate = false;
    }
}
