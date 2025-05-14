using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class InputManager : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] private BoxCollider2D cameraBoundingBox;
    [SerializeField] private float cameraMoveSpeed = 5f;
    private Vector3 cameraMoveDirection = Vector3.zero;

    [SerializeField] private float cardRotationSpeed = 120f;
    private bool rotatingCard = false;

    private float maxX;
    private float minX;
    private float maxY;
    private float minY;

    private void Awake()
    {
        mainCamera = Camera.main;

        float cameraHalfHeight = mainCamera.orthographicSize;
        float cameraHalfWidth = cameraHalfHeight * mainCamera.aspect;

        maxX = cameraBoundingBox.bounds.max.x - cameraHalfWidth;
        minX = cameraBoundingBox.bounds.min.x + cameraHalfWidth;
        maxY = cameraBoundingBox.bounds.max.y - cameraHalfHeight;
        minY = cameraBoundingBox.bounds.min.y + cameraHalfHeight;
    }
    private void Update()
    {
        mainCamera.transform.position += cameraMoveDirection * cameraMoveSpeed * Time.deltaTime;
        Vector3 clampedPosition = mainCamera.transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);
        mainCamera.transform.position = clampedPosition;

        var selectedCard = CombatManager.Instance.SelectedCard;
        if (selectedCard == null) return;

        selectedCard.UpdatePosition(Input.mousePosition);

        if (rotatingCard)
        {
            selectedCard.DeploymentPreviewObject.transform.Rotate(0, 0, cardRotationSpeed * Time.deltaTime);
        }
    }

    private void OnLook(InputValue value)
    {
        cameraMoveDirection = value.Get<Vector2>();
    }

    private void OnRotateCard(InputValue value)
    {
        rotatingCard = value.isPressed;
    }
}
