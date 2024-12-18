using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class UIManager : MonoBehaviour
{
    public enum GameMode
    {
        Reaction,
        Memory
    }

    [Header("UI Elements")]
    public GameObject mainMenu;             // 메인 메뉴
    public GameObject gameMode;             // 게임 모드 선택 화면
    public GameObject howtoPlay;            // How to Play 화면
    public GameObject canvasUI;             // 타겟 고정 Canvas
    public GameObject inGameUI;             // In-Game UI
    public GameObject resultUI;
    public TMP_Text scoreText;              // 점수 표시 TextMeshPro UI
    public Button gunButton;                // Gun 버튼
    public RectTransform crossHair;         // CrossHair UI
    public ARRaycastManager arRaycastManager;
    public GameObject linePrefab;

    [Header("AR Settings")]
    public Camera arCamera;                 // AR 카메라
    public GameObject targetPrefab;         // 타겟 Prefab (기존 객체 사용)
    public GameObject targetObject;         // 타겟 오브젝트
    public float targetDistance = 10.0f;    // 카메라와 타겟 간 거리
    public Vector3 offset;                  // 추가적인 위치 조정
    public Vector3 targetScale = new Vector3(0.5f, 0.5f, 0.5f);
    public LayerMask targetLayer;

    public Material defaultMaterial;        // 기본 상태 Material
    public Material targetOnMaterial;       // 활성화 상태 Material

    private LineRenderer lineRenderer;
    private GameMode currentMode;           // 현재 선택된 게임 모드
    private GameObject currentTarget;       // 현재 타겟 참조
    private bool isTargetPlaced = false;    // 과녁 고정 여부
    private int score = 0;                  // 점수

    private void Start()
    {
        // 초기 UI 상태 설정
        mainMenu.SetActive(true);
        gameMode.SetActive(false);
        canvasUI.SetActive(false);
        inGameUI.SetActive(false);

        if (targetPrefab != null)
        {
            currentTarget = targetPrefab;
            currentTarget.transform.localScale = targetScale;
        }
        UpdateScoreUI();
        // LineRenderer 생성
        GameObject lineObject = Instantiate(linePrefab);
        lineRenderer = lineObject.GetComponent<LineRenderer>();
        lineRenderer.enabled = false; // 초기에는 비활성화
    }

    public void StartGame()
    {
        mainMenu.SetActive(false);
        gameMode.SetActive(true);
        Debug.Log("게임 시작: 게임 모드 선택 화면으로 이동");
    }

    public void ShowHowtoPlay()
    {
        mainMenu.SetActive(false);
        howtoPlay.SetActive(true);
        Debug.Log("How to Play 화면 표시");
    }

    private void Update()
    {
        if (!isTargetPlaced && currentTarget != null)
        {
            UpdateTargetPositionAndRotation();
        }
    }

    public void SelectReactionMode()
    {
        currentMode = GameMode.Reaction;
        gameMode.SetActive(false);
        canvasUI.SetActive(true);
        UpdateTargetPositionAndRotation();
        Debug.Log("Reaction 모드 선택");
    }

    public void SelectMemoryMode()
    {
        currentMode = GameMode.Memory;
        gameMode.SetActive(false);
        canvasUI.SetActive(true);
        UpdateTargetPositionAndRotation();
        Debug.Log("Memory 모드 선택");
    }
    public void RestartGameMode()
    {
        Debug.Log("다시하기 버튼: 모드를 다시 시작합니다.");

        // 점수 초기화
        score = 0;
        UpdateScoreUI();

        // 타겟 상태 초기화
        isTargetPlaced = false;
        if (currentTarget != null)
        {
            currentTarget = targetPrefab;
            currentTarget.transform.localScale = targetScale;
            UpdateTargetPositionAndRotation();
        }

        // UI 상태 리셋
        inGameUI.SetActive(true);
        resultUI.SetActive(false);
        canvasUI.SetActive(false);

        // 현재 모드에 따라 다시 시작
        if (currentMode == GameMode.Reaction)
        {
            ReactionModeManager reactionMode = FindObjectOfType<ReactionModeManager>();
            if (reactionMode != null)
            {
                reactionMode.StopReactionMode(); // 기존 진행 중인 모드 종료
                reactionMode.StartReactionMode(); // 새로 시작
            }
            else
            {
                Debug.LogError("ReactionModeManager를 찾을 수 없습니다.");
            }
        }
        else if (currentMode == GameMode.Memory)
        {
            MemoryModeManager memoryMode = FindObjectOfType<MemoryModeManager>();
            if (memoryMode != null)
            {
                memoryMode.StartMemoryMode(); // 메모리 모드 다시 시작
            }
            else
            {
                Debug.LogError("MemoryModeManager를 찾을 수 없습니다.");
            }
        }
    }


    private void UpdateTargetPositionAndRotation()
    {
        if (currentTarget == null) return;

        Vector3 targetPosition = arCamera.transform.position + arCamera.transform.forward * targetDistance + offset;
        currentTarget.transform.position = targetPosition;
        LookAtCamera(currentTarget);
    }

    private void LookAtCamera(GameObject target)
    {
        Vector3 direction = (arCamera.transform.position - target.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(-direction);
        target.transform.rotation = targetRotation * Quaternion.Euler(0, 90, 0);
    }

    public void PlaceTarget()
    {
        if (currentTarget != null && !isTargetPlaced)
        {
            isTargetPlaced = true;
            canvasUI.SetActive(false);
            inGameUI.SetActive(true);

            if (currentMode == GameMode.Reaction)
            {
                ReactionModeManager reactionMode = FindObjectOfType<ReactionModeManager>();
                reactionMode?.StartReactionMode();
            }
            else if (currentMode == GameMode.Memory)
            {
                MemoryModeManager memoryMode = FindObjectOfType<MemoryModeManager>();
                memoryMode?.StartMemoryMode();
            }

            Debug.Log("타겟 고정 완료!");
        }
    }

    public void FireGun()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = arCamera.ScreenPointToRay(screenCenter);
        Debug.Log($"Ray Origin: {ray.origin}, Direction: {ray.direction}");

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayer))
        {
            GameObject hitObject = hit.collider.gameObject;
            Renderer renderer = hitObject.GetComponent<Renderer>();

            if (renderer != null && renderer.sharedMaterial != null)
            {
                if (currentMode == GameMode.Reaction)
                {
                    HandleReactionModeHit(renderer);
                }
                else if (currentMode == GameMode.Memory)
                {
                    HandleMemoryModeHit(hitObject);
                }
            }
        }
        else
        {
            Debug.LogWarning("Raycast 충돌 실패: 아무 객체도 맞추지 못했습니다.");
        }
    }

    private void HandleReactionModeHit(Renderer targetRenderer)
    {
        if (targetRenderer.sharedMaterial != null && targetRenderer.sharedMaterial.name == targetOnMaterial.name)
        {
            targetRenderer.sharedMaterial = defaultMaterial; // 기본 상태로 초기화
            AddScore(10);
            Debug.Log("Reaction Mode 명중!");
        }
        else
        {
            Debug.Log("Reaction Mode: 유효하지 않은 타겟.");
        }
    }

    private void HandleMemoryModeHit(GameObject hitObject)
    {
        MemoryModeManager memoryMode = FindObjectOfType<MemoryModeManager>();
        if (memoryMode != null)
        {
            memoryMode.HandleHit(hitObject.transform);
        }
        else
        {
            Debug.LogError("MemoryModeManager를 찾을 수 없습니다.");
        }
    }

    private void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
        else
        {
            Debug.LogError("ScoreText UI가 설정되지 않았습니다.");
        }
    }

    public void ReturnToMainMenu()
    {
        mainMenu.SetActive(true);
        gameMode.SetActive(false);
        canvasUI.SetActive(false);

        inGameUI.SetActive(false);
        resultUI.SetActive(false);
        howtoPlay.SetActive(false);

        isTargetPlaced = false;
        score = 0;
        UpdateScoreUI();

        Debug.Log("메인 메뉴로 돌아갑니다.");
    }

    public void ExitApplication()
    {
        Application.Quit();
    }
}
