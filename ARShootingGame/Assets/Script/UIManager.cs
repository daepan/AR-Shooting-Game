using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject mainMenu;           // 메인 메뉴
    public GameObject gameMode;           // 게임 모드 선택 화면
    public GameObject howtoPlay;          // How to Play 화면
    public Camera arCamera;               // AR 카메라
    public GameObject targetPrefab;       // 타겟 생성용 Prefab
    public GameObject targetObject;       // 타겟 오브젝트
    public GameObject canvasUI;           // 과녁 고정 버튼이 있는 Canvas UI
    public GameObject inGameUI;
    public float targetDistance = 10.0f;  // 카메라와 타겟 간 거리
    public Vector3 offset;                // 추가적인 위치 조정
    public Vector3 targetScale = new Vector3(0.5f, 0.5f, 0.5f); // 타겟 크기

    private GameObject currentTarget;     // 현재 활성화된 타겟 인스턴스
    private bool isTargetPlaced = false;  // 과녁 고정 여부
    private string selectedMode = "";     // 선택된 모드

    private void Start()
    {
        canvasUI.SetActive(false); // Canvas UI 숨김
        arCamera.nearClipPlane = 0.01f;
        arCamera.farClipPlane = 100f;     // 클리핑 제한
    }

    public void StartGame()
    {
        mainMenu.SetActive(false);
        gameMode.SetActive(true);
    }

    public void SelectMemoryMode()
    {
        selectedMode = "Memory";
        gameMode.SetActive(false);
        ActivateTargetAndUI();
    }

    public void SelectReactionMode()
    {
        selectedMode = "Reaction";
        gameMode.SetActive(false);
        ActivateTargetAndUI();
    }

    private void ActivateTargetAndUI()
    {
        if (currentTarget != null)
            Destroy(currentTarget);

        // Prefab에서 새로운 타겟 생성
        currentTarget = targetPrefab;
        currentTarget.transform.localScale = targetScale;

        targetObject.SetActive(true);
        UpdateTargetPositionAndRotation();
        canvasUI.SetActive(true);
        isTargetPlaced = false;
    }

    private void Update()
    {
        if (!isTargetPlaced && currentTarget != null)
        {
            UpdateTargetPositionAndRotation();
        }
    }

    private void UpdateTargetPositionAndRotation()
    {
        if (currentTarget == null) return;

        // 부모 객체(Matrix)의 위치를 카메라 정면으로 이동
        Vector3 targetPosition = arCamera.transform.position + arCamera.transform.forward * targetDistance + offset;
        currentTarget.transform.position = targetPosition;

        // 부모 객체가 카메라를 바라보도록 회전
        LookAtCamera(currentTarget);
    }

    private void LookAtCamera(GameObject target)
    {
        // 방향 설정: 카메라를 향하는 방향 계산
        Vector3 direction = (arCamera.transform.position - target.transform.position).normalized;

        // 타겟(부모)의 회전 설정
        Quaternion targetRotation = Quaternion.LookRotation(-direction);

        // 회전 보정 (필요시 추가)
        target.transform.rotation = targetRotation * Quaternion.Euler(0, 90, 0);
    }

    public void PlaceTarget()
    {
        if (currentTarget != null && !isTargetPlaced)
        {
            isTargetPlaced = true;       // 타겟 고정
            canvasUI.SetActive(false);   // Canvas UI 숨김
            inGameUI.SetActive(true);
            Debug.Log($"타겟 고정 완료! 위치: {currentTarget.transform.position}");
            StartSelectedMode();
        }
    }

    private void StartSelectedMode()
    {
        if (selectedMode == "Memory")
        {
            Debug.Log("기억력 모드 시작!");
            MemoryModeManager memoryMode = FindObjectOfType<MemoryModeManager>();
            memoryMode?.StartMemoryMode();
        }
        else if (selectedMode == "Reaction")
        {
            Debug.Log("순발력 모드 시작!");
            ReactionModeManager reactionMode = FindObjectOfType<ReactionModeManager>();
            reactionMode?.StartReactionMode();
        }
    }

    public void CheckHit(string targetName)
    {
        GameObject targetOn = currentTarget.transform.Find(targetName + "_on")?.gameObject;

        if (targetOn != null)
        {
            // `_on` 상태 활성화
            targetOn.SetActive(true);
            Debug.Log($"{targetName}_on 활성화됨!");
        }
        else
        {
            Debug.LogError($"타겟 {targetName}_on을 찾을 수 없습니다.");
        }
    }

    public void ReturnToMainMenu()
    {
        mainMenu.SetActive(true);
        gameMode.SetActive(false);
        howtoPlay.SetActive(false);
        inGameUI.SetActive(false);

        if (currentTarget != null)
            Destroy(currentTarget);

        canvasUI.SetActive(false);
        isTargetPlaced = false;
    }

    public void ExitApplication()
    {
        Application.Quit();
    }
}
