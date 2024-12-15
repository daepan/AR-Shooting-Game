using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections; // Coroutine에 필요한 네임스페이스


public class ReactionModeManager : MonoBehaviour
{
    public GameObject[] targets;          // 타겟 배열 (B1 ~ B16 오브젝트)
    public Material defaultMaterial;      // 기본 상태 Material
    public Material highlightedMaterial;  // 점등 상태 Material
    public GameObject aimUI;              // 화면 중앙의 에임 UI
    public GameObject gunUI;              // 화면 하단의 권총 UI
    public Camera mainCamera;             // AR 카메라
    private bool isAiming = false;        // 에임 활성화 여부
    private GameObject currentTarget = null; // 현재 점등된 타겟
    private bool isGameActive = false;    // 게임 진행 상태
    private int score = 0;                // 플레이어 점수

    private void Start()
    {
        aimUI.SetActive(false); // 초기 상태에서 에임 비활성화
    }

    public void StartReactionMode()
    {
        Debug.Log("순발력 모드 시작!");
        isGameActive = true;
        StartCoroutine(GameRoutine());
    }

    private IEnumerator GameRoutine()
    {
        while (isGameActive)
        {
            ActivateRandomTarget();
            yield return new WaitForSeconds(3f);
            DeactivateTarget();
        }
    }

    private void ActivateRandomTarget()
    {
        // 기존 타겟 점등 해제
        if (currentTarget != null)
        {
            DeactivateTarget();
        }

        // 랜덤으로 타겟 선택
        int randomIndex = Random.Range(0, targets.Length);
        currentTarget = targets[randomIndex];

        // 타겟 점등
        MeshRenderer renderer = currentTarget.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.material = highlightedMaterial; // 점등 상태 Material 적용
        }

        Debug.Log($"{currentTarget.name} 점등됨!");
    }

    private void DeactivateTarget()
    {
        if (currentTarget != null)
        {
            // 기본 상태로 변경
            MeshRenderer renderer = currentTarget.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material = defaultMaterial; // 기본 상태 Material 적용
            }

            Debug.Log($"{currentTarget.name} 비활성화됨!");
            currentTarget = null;
        }
    }

    public void OnGunPressed()
    {
        // 권총 터치 시 에임 활성화
        isAiming = true;
        aimUI.SetActive(true);
        Debug.Log("에임 활성화됨!");
    }

    public void OnScreenTouched(Vector2 touchPosition)
    {
        if (!isAiming || !isGameActive) return;

        // 터치 위치를 기준으로 Raycast 수행
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(touchPosition.x, touchPosition.y, 0));
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject == currentTarget)
            {
                // 타겟 명중
                Debug.Log("타겟 명중!");
                score += 1; // 점수 증가
                DeactivateTarget();
            }
            else
            {
                Debug.Log("타겟을 맞추지 못했습니다.");
            }
        }

        // 에임 비활성화
        isAiming = false;
        aimUI.SetActive(false);
    }
}
