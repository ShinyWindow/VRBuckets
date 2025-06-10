using UnityEngine;
using UnityEngine.InputSystem;
using Normal.Realtime;

public class SimpleXRMovement : MonoBehaviour
{
    public InputActionProperty moveInput;
    public InputActionProperty turnInput;
    public float moveSpeed = 1.5f;
    public float snapTurnAngle = 45f;
    public float snapThreshold = 0.7f;

    private RealtimeTransform _realtimeTransform;
    private bool _readyToSnapTurn = true;

    private void Start()
    {
        _realtimeTransform = GetComponent<RealtimeTransform>();
        moveInput.action.Enable();
        turnInput.action.Enable();
    }

    void Update()
    {
        if (!_realtimeTransform.isOwnedLocallySelf) return;

        HandleMovement();
        HandleSnapTurn();
    }

    private void HandleMovement()
    {
        Vector2 input = moveInput.action.ReadValue<Vector2>();
        if (input == Vector2.zero) return;

        var head = Camera.main.transform;
        var forward = new Vector3(head.forward.x, 0, head.forward.z).normalized;
        var right = new Vector3(head.right.x, 0, head.right.z).normalized;

        Vector3 move = (forward * input.y + right * input.x) * moveSpeed * Time.deltaTime;
        transform.position += move;
    }

    private void HandleSnapTurn()
    {
        float turnValue = turnInput.action.ReadValue<Vector2>().x;

        if (_readyToSnapTurn && Mathf.Abs(turnValue) > snapThreshold)
        {
            float angle = snapTurnAngle * Mathf.Sign(turnValue);
            transform.Rotate(0, angle, 0);
            _readyToSnapTurn = false;
        }

        // Wait until user returns stick to center before allowing another snap
        if (!_readyToSnapTurn && Mathf.Abs(turnValue) < 0.2f)
        {
            _readyToSnapTurn = true;
        }
    }
}
