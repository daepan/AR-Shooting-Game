using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using TMPro;

public class UIManager : MonoBehaviour
{
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

    private LineRenderer lineRenderer;
    private GameObject currentTarget;       // 현재 타겟 참조
    private bool isTargetPlaced = false;    // 과녁 고정 여부
    private int score = 0;                  // 점수
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    public AudioClip fireSound; // 발사 소리 클립
    private AudioSource audioSource; // 오디오 소스 변수

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>(); // 오디오 소스 컴포넌트

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
        // 메인 메뉴에서 게임 모드 선택 화면으로 전환
        mainMenu.SetActive(false);
        gameMode.SetActive(true);
        Debug.Log("게임 시작: 게임 모드 선택 화면으로 이동");
    }


    public void ShowHowtoPlay()
    {
        // How to Play 화면 표시
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
        gameMode.SetActive(false);
        canvasUI.SetActive(true);
        UpdateTargetPositionAndRotation();
    }

    private void UpdateTargetPositionAndRotation()
    {
        if (currentTarget == null) return;

        // 타겟을 카메라 정면으로 이동
        Vector3 targetPosition = arCamera.transform.position + arCamera.transform.forward * targetDistance + offset;
        currentTarget.transform.position = targetPosition;

        // 타겟이 항상 카메라를 바라보도록 회전
        LookAtCamera(currentTarget);
    }

    private void LookAtCamera(GameObject target)
    {
        // 방향 설정
        Vector3 direction = (arCamera.transform.position - target.transform.position).normalized;

        // 회전 설정 (90도 보정 추가)
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

            ReactionModeManager reactionMode = FindObjectOfType<ReactionModeManager>();
            reactionMode?.StartReactionMode();

            Debug.Log("타겟 고정 완료!");
        }
    }

    public void FireGun()
    {
        // 화면 중앙에서 Ray를 발사
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = arCamera.ScreenPointToRay(screenCenter);
        Debug.Log($"Ray Origin: {ray.origin}, Direction: {ray.direction}");

        RaycastHit hit;



        // Raycast 실행 및 시각화
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayer))
        {
            GameObject hitObject = hit.collider.gameObject;


            Debug.Log($"충돌 객체: {hitObject.name}, Layer: {LayerMask.LayerToName(hitObject.layer)}");

            audioSource.PlayOneShot(fireSound);

            // "_on" 상태의 타겟인지 확인
            if (hitObject.name.Contains("_on") && hitObject.activeSelf)
            {
                hitObject.SetActive(false);
                AddScore(10);
                Debug.Log($"명중: {hitObject.name}");
            }
            else
            {
                Debug.Log("유효하지 않은 타겟.");
            }
        }
        else
        {
            Debug.LogWarning("Raycast 충돌 실패: 아무 객체도 맞추지 못했습니다.");
        }
    }

    private void VisualizeRay(Vector3 start, Vector3 end)
    {
        if (lineRenderer == null)
        {
            Debug.LogWarning("LineRenderer가 설정되지 않았습니다.");
            return;
        }

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, start); // Ray 시작점
        lineRenderer.SetPosition(1, end);   // Ray 끝점
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
            currentTarget.SetActive(false);
            currentTarget = targetPrefab;
            currentTarget.transform.localScale = targetScale;
            UpdateTargetPositionAndRotation();
        }

        // UI 상태 리셋
        inGameUI.SetActive(true);
        resultUI.SetActive(false);
        canvasUI.SetActive(true);

        // ReactionModeManager를 찾아서 모드 재시작
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

    public void ReturnToMainMenu()
    {
        // UI 초기화
        mainMenu.SetActive(true);
        gameMode.SetActive(false);
        canvasUI.SetActive(false);
        inGameUI.SetActive(false);
        resultUI.SetActive(false);
        howtoPlay.SetActive(false);

        // 상태 초기화
        if (currentTarget != null) currentTarget.SetActive(false);
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
