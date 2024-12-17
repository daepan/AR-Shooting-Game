using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReactionModeManager : MonoBehaviour
{
    public GameObject matrixObject;          // 기존 Matrix 오브젝트
    public GameObject inGameCanvas;          // InGameCanvas 참조
    public GameObject resultCanvas;          // ResultCanvas 참조
    public ScoreManager scoreManager;        // 점수를 관리하는 ScoreManager

    private bool isReactionModeActive = false; // Reaction Mode 활성화 여부

    public void StartReactionMode()
    {
        if (matrixObject == null)
        {
            Debug.LogError("Matrix 오브젝트가 설정되지 않았습니다.");
            return;
        }

        // Matrix 오브젝트 활성화
        matrixObject.SetActive(true);
        isReactionModeActive = true;

        // InGameCanvas 활성화 및 ResultCanvas 비활성화
        inGameCanvas.SetActive(true);
        resultCanvas.SetActive(false);

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

            // 랜덤으로 "_on"이 포함된 자식 오브젝트 활성화
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

        List<Transform> onChildren = new List<Transform>();
        foreach (Transform child in children)
        {
            if (child.name.Contains("_on") && !child.gameObject.activeSelf)
            {
                onChildren.Add(child);
            }
        }

        if (onChildren.Count == 0)
        {
            Debug.LogWarning("활성화 가능한 '_on' 자식 오브젝트가 없습니다.");
            return;
        }

        Transform randomChild = onChildren[Random.Range(0, onChildren.Count)];
        randomChild.gameObject.SetActive(true);

        Debug.Log($"랜덤 활성화: {randomChild.name}");
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
            // 점수를 계산하고 UI 업데이트
            scoreManager.DisplayResults();
            Debug.Log("결과 점수 표시 완료");
        }
        else
        {
            Debug.LogError("ScoreManager가 설정되지 않았습니다.");
        }
    }
}
