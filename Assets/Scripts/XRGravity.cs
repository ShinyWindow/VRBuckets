using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class XRGravity : MonoBehaviour
{
    public float gravity = -9.81f;
    public float maxFallSpeed = -5f;
    public float fallingSpeed = 0f;

    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (characterController.isGrounded)
        {
            if (fallingSpeed < 0f)
                fallingSpeed = 0f;
        }
        else
        {
            fallingSpeed += gravity * Time.deltaTime;
            if (fallingSpeed < maxFallSpeed)
                fallingSpeed = maxFallSpeed;
        }

        characterController.Move(Vector3.up * fallingSpeed * Time.deltaTime);
    }
}
