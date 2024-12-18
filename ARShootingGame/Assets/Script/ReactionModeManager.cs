using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReactionModeManager : MonoBehaviour
{
    public GameObject matrixObject;          // 기존 Matrix 오브젝트
    public GameObject inGameCanvas;          // InGameCanvas 참조
    public GameObject resultCanvas;          // ResultCanvas 참조
    public ScoreManager scoreManager;        // 점수를 관리하는 ScoreManager
    public Material targetOnMaterial;        // 타겟 활성화 상태의 메테리얼
    public Material defaultMaterial;         // 타겟 기본 상태의 메테리얼

    private bool isReactionModeActive = false; // Reaction Mode 활성화 여부

    public void StartReactionMode()
    {
        if (matrixObject == null || targetOnMaterial == null || defaultMaterial == null)
        {
            Debug.LogError("Matrix 오브젝트 또는 메테리얼이 설정되지 않았습니다.");
            return;
        }

        // Matrix 오브젝트 활성화
        matrixObject.SetActive(true);
        isReactionModeActive = true;

        // InGameCanvas 활성화 및 ResultCanvas 비활성화
        inGameCanvas.SetActive(true);
        resultCanvas.SetActive(false);

        // 모든 타겟 초기화
        InitializeTargets();

        // 랜덤 활성화 코루틴 시작
        StartCoroutine(ActivateTargetsRoutine());

        Debug.Log("Reaction Mode 시작: Matrix 활성화 완료");
    }

    public void StopReactionMode()
    {
        if (matrixObject != null)
        {
            // Matrix 오브젝트 비활성화
            matrixObject.SetActive(false);
        }

        isReactionModeActive = false;

        // 모든 코루틴 정지
        StopAllCoroutines();

        Debug.Log("Reaction Mode 종료: Matrix 비활성화 완료");

        // 결과 화면을 보여줌
        ShowResultCanvas();
    }

    private void InitializeTargets()
    {
        // 모든 타겟을 기본 메테리얼로 초기화
        Transform[] children = matrixObject.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            if (child.name.Contains("Base"))
            {
                continue; // "Base"라는 이름을 가진 타겟 제외
            }

            Renderer renderer = child.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = defaultMaterial;
                child.gameObject.SetActive(true); // 활성화
            }
        }
        Debug.Log("모든 타겟이 기본 상태로 초기화되었습니다.");
    }

    private IEnumerator ActivateTargetsRoutine()
    {
        float elapsedTime = 0f; // 경과 시간
        float interval = 3f;    // 초기 간격

        while (isReactionModeActive)
        {
            // 경과 시간에 따라 간격 조정
            if (elapsedTime > 30f)
            {
                interval = 2f; // 30초 이후에는 2초 간격
            }

            // 랜덤으로 타겟 활성화
            ActivateRandomTarget();

            // 대기 후 경과 시간 증가
            yield return new WaitForSeconds(interval);
            elapsedTime += interval;

            // 게임이 60초 동안 실행되면 종료
            if (elapsedTime >= 60f)
            {
                StopReactionMode();
            }
        }
    }

    private void ActivateRandomTarget()
    {
        Transform[] children = matrixObject.GetComponentsInChildren<Transform>(true);
        if (children.Length == 0) return;

        List<Transform> inactiveTargets = new List<Transform>();
        foreach (Transform child in children)
        {
            if (child.name.Contains("Base"))
            {
                continue; // "Base"라는 이름을 가진 타겟 제외
            }

            Renderer renderer = child.GetComponent<Renderer>();
            if (renderer != null && renderer.material != targetOnMaterial)
            {
                inactiveTargets.Add(child);
            }
        }

        if (inactiveTargets.Count == 0)
        {
            Debug.LogWarning("활성화 가능한 타겟이 없습니다.");
            return;
        }

        Transform randomTarget = inactiveTargets[Random.Range(0, inactiveTargets.Count)];
        Renderer targetRenderer = randomTarget.GetComponent<Renderer>();

        if (targetRenderer != null)
        {
            targetRenderer.material = targetOnMaterial;
            Debug.Log($"랜덤 활성화: {randomTarget.name}");

            // 3초 후 비활성화 코루틴 시작
            StartCoroutine(DeactivateTargetAfterDelay(randomTarget));
        }
    }

    private IEnumerator DeactivateTargetAfterDelay(Transform target)
    {
        Debug.Log($"3초 뒤 타겟 비활성화 예정: {target.name}");
        yield return new WaitForSeconds(3f); // 3초 대기

        if (target != null) // 타겟이 여전히 존재하는지 확인
        {
            Renderer renderer = target.GetComponent<Renderer>();
            if (renderer != null)
            {
                // 현재 메테리얼 정보를 디버그로 출력
                Debug.Log($"타겟 {target.name} 현재 메테리얼: {renderer.sharedMaterial.name}");

                // 메테리얼이 targetOnMaterial인지 확인 (sharedMaterial 사용)
                if (renderer.sharedMaterial == targetOnMaterial)
                {
                    renderer.sharedMaterial = defaultMaterial;
                    Debug.Log($"타겟 {target.name} 자동 비활성화 완료. 새 메테리얼: {renderer.sharedMaterial.name}");
                }
                else
                {
                    Debug.LogWarning($"타겟 {target.name}의 메테리얼이 이미 변경되었습니다. 현재 메테리얼: {renderer.sharedMaterial.name}");
                }
            }
            else
            {
                Debug.LogWarning($"타겟 {target.name}에 Renderer가 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("타겟이 null입니다.");
        }
    }



    private void ShowResultCanvas()
    {
        // InGameCanvas 비활성화
        inGameCanvas.SetActive(false);

        // ResultCanvas 활성화
        resultCanvas.SetActive(true);

        // 결과 점수 표시
        if (scoreManager != null)
        {
            scoreManager.DisplayResults();
            Debug.Log("결과 점수 표시 완료");
        }
        else
        {
            Debug.LogError("ScoreManager가 설정되지 않았습니다.");
        }
    }
}