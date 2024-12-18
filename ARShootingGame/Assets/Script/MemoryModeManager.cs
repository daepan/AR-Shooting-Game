using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class MemoryModeManager : MonoBehaviour
{
    public GameObject matrixObject;              // 16개의 타겟이 포함된 부모 오브젝트
    public Material[] materials;                 // 8가지 메테리얼 배열
    public Material blackMaterial;               // 검정색 메테리얼
    public GameObject resultCanvas;              // 결과 화면
    public GameObject inGameCanvas;              // 게임 중 UI
    public TMP_Text scoreText;                   // 점수 표시 텍스트
    public ScoreManager scoreManager;

    private Dictionary<Transform, Material> originalMaterials = new Dictionary<Transform, Material>();
    private Transform firstSelected = null;      // 첫 번째 선택된 타겟
    private Transform secondSelected = null;     // 두 번째 선택된 타겟
    private int matchedPairs = 0;                // 맞춘 짝의 수
    private int score = 0;                       // 현재 점수

    public void StartMemoryMode()
    {
        if (matrixObject == null || materials.Length != 8 || blackMaterial == null)
        {
            Debug.LogError("MemoryModeManager 초기화 실패: 설정을 확인하세요.");
            return;
        }

        resultCanvas.SetActive(false);
        inGameCanvas.SetActive(true);

        InitializeMaterials();
        StartCoroutine(ShowMaterialsTemporarily());

        // 점수 초기화
        score = 0;
        UpdateScoreText();
    }

    private void InitializeMaterials()
    {
        // 메테리얼 배열 섞기
        List<Material> shuffledMaterials = new List<Material>(materials);
        shuffledMaterials.AddRange(materials); // 8개의 메테리얼을 2개씩 추가
        shuffledMaterials = Shuffle(shuffledMaterials);

        Transform[] children = matrixObject.GetComponentsInChildren<Transform>(true);

        // 각 타겟에 메테리얼 지정
        int index = 0;
        foreach (Transform child in children)
        {
            Renderer renderer = child.GetComponent<Renderer>();
            if (renderer != null && index < shuffledMaterials.Count)
            {
                originalMaterials[child] = shuffledMaterials[index];
                renderer.material = shuffledMaterials[index];
                index++;
            }
        }

        if (index < shuffledMaterials.Count)
        {
            Debug.LogWarning("자식 개수가 부족하여 일부 메테리얼이 할당되지 않았습니다.");
        }
    }

    private IEnumerator ShowMaterialsTemporarily()
    {
        // 10초 대기 후 검정색 메테리얼로 변경
        yield return new WaitForSeconds(10f);
        foreach (var pair in originalMaterials)
        {
            Renderer renderer = pair.Key.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = blackMaterial;
            }
        }

        Debug.Log("모든 메테리얼을 검정색으로 변경.");
    }

    public void HandleHit(Transform target)
    {
        if (!originalMaterials.ContainsKey(target)) return;

        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer == null) return;

        // 첫 번째 선택
        if (firstSelected == null)
        {
            firstSelected = target;
            renderer.material = originalMaterials[target];
            Debug.Log($"첫 번째 선택: {target.name}");
            return;
        }

        // 두 번째 선택
        if (secondSelected == null && target != firstSelected)
        {
            secondSelected = target;
            renderer.material = originalMaterials[target];
            Debug.Log($"두 번째 선택: {target.name}");

            // 두 타겟 비교
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(1f); // 1초 대기 후 비교

        Renderer firstRenderer = firstSelected.GetComponent<Renderer>();
        Renderer secondRenderer = secondSelected.GetComponent<Renderer>();

        if (firstRenderer != null && secondRenderer != null)
        {
            if (originalMaterials[firstSelected] == originalMaterials[secondSelected])
            {
                Debug.Log("짝이 맞았습니다!");
                matchedPairs++;
                score += 30; // 점수 추가
                UpdateScoreText();

                // 짝이 맞으면 상태 유지
                firstSelected = null;
                secondSelected = null;

                // 모든 짝을 맞춘 경우 게임 종료
                if (matchedPairs == materials.Length)
                {
                    EndGame();
                }
            }
            else
            {
                Debug.Log("짝이 맞지 않습니다.");
                firstRenderer.material = blackMaterial;
                secondRenderer.material = blackMaterial;
                score -= 10; // 점수 감점
                UpdateScoreText();

                firstSelected = null;
                secondSelected = null;
            }
        }
    }

    private void EndGame()
    {
        Debug.Log("게임 종료: 모든 짝을 맞췄습니다.");
        ShowResultCanvas();
    }

    private List<T> Shuffle<T>(List<T> list)
    {
        System.Random rand = new System.Random();
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = rand.Next(0, i + 1);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
        return list;
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
        else
        {
            Debug.LogError("ScoreText가 설정되지 않았습니다.");
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
