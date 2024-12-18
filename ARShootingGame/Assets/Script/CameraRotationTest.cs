using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotationTest : MonoBehaviour
{
    public float rotationSpeed = 50f; // 회전 속도 (도/초)
    private Vector2 inputRotation;   // 마우스 또는 키보드 입력

    void Update()
    {
        // 입력 값을 가져오기
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            inputRotation.y -= rotationSpeed * Time.deltaTime;
        }
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            inputRotation.y += rotationSpeed * Time.deltaTime;
        }
        if (Keyboard.current.upArrowKey.isPressed)
        {
            inputRotation.x -= rotationSpeed * Time.deltaTime;
        }
        if (Keyboard.current.downArrowKey.isPressed)
        {
            inputRotation.x += rotationSpeed * Time.deltaTime;
        }

        // 회전 적용
        transform.rotation = Quaternion.Euler(inputRotation.x, inputRotation.y, 0f);
    }
}
