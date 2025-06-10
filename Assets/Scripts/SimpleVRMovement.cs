using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class SimpleVRMovement : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public InputActionProperty moveAction;

    private Vector2 inputAxis;
    private CharacterController characterController;
    private Transform head;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        head = Camera.main.transform;
        moveAction.action.Enable(); // Required
    }

    void Update()
    {
        inputAxis = moveAction.action.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        Vector3 direction = new Vector3(inputAxis.x, 0, inputAxis.y);
        direction = head.TransformDirection(direction);
        direction.y = 0;
        characterController.Move(direction * moveSpeed * Time.fixedDeltaTime);
    }
}
