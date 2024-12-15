using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReactionModeManager : MonoBehaviour
{
    public GameObject matrixObject; // 기존 Matrix 오브젝트
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
        }
    }

    private void ActivateRandomTarget()
    {
        // 자식 오브젝트 중 "_on"이 포함된 이름의 오브젝트만 선택
        Transform[] children = matrixObject.GetComponentsInChildren<Transform>(true);
        if (children.Length == 0) return;

        // "_on"이 포함된 자식 오브젝트 필터링
        List<Transform> onChildren = new List<Transform>();
        foreach (Transform child in children)
        {
            if (child.name.Contains("_on") && !child.gameObject.activeSelf)
            {
                onChildren.Add(child);
            }
        }

        // "_on" 자식이 없으면 반환
        if (onChildren.Count == 0)
        {
            Debug.LogWarning("활성화 가능한 '_on' 자식 오브젝트가 없습니다.");
            return;
        }

        // 랜덤으로 하나 선택하여 활성화
        Transform randomChild = onChildren[Random.Range(0, onChildren.Count)];
        randomChild.gameObject.SetActive(true);

        // Renderer 활성화
        Renderer renderer = randomChild.GetComponent<Renderer>();
        if (renderer != null && !renderer.enabled)
        {
            renderer.enabled = true;
        }

        Debug.Log($"랜덤 활성화: {randomChild.name}");
    }
}
