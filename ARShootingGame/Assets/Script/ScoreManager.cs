using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text scoreText;            // 현재 점수를 표시하는 Text (예: "Score: 100")
    public TMP_Text hitsCountText;        // 맞힌 갯수 Text
    public TMP_Text bonusScoreText;       // 보너스 점수 Text
    public TMP_Text totalScoreText;       // 최종 점수 Text

    private int hitsCount = 0;            // 맞힌 갯수
    private float bonusScore = 0f;        // 보너스 점수
    private int totalScore = 0;           // 최종 점수

    private int currentScore = 0;         // 현재 점수 (내부 변수로 저장)

    /// <summary>
    /// 결과를 표시하는 메서드
    /// </summary>
    public void DisplayResults()
    {
        CalculateScores();
        UpdateUI();
        Debug.Log("결과가 성공적으로 표시되었습니다.");
    }

    private void CalculateScores()
    {
        // 점수 텍스트에서 현재 점수를 가져옴
        currentScore = GetScoreFromText();

        // 맞힌 갯수 = 현재 점수 / 10 (1개당 10점)
        hitsCount = currentScore / 10;

        // 보너스 점수 = 맞힌 갯수 x 0.1
        bonusScore = hitsCount * 0.1f;

        // 최종 점수 = 현재 점수 + 보너스 점수 (올림처리)
        totalScore = currentScore + Mathf.CeilToInt(bonusScore);
    }

    private int GetScoreFromText()
    {
        // ScoreText의 텍스트에서 숫자만 추출
        string scoreString = scoreText.text.Replace("Score: ", "");
        if (int.TryParse(scoreString, out int parsedScore))
        {
            return parsedScore;
        }

        Debug.LogWarning("ScoreText에서 점수를 추출하는데 실패했습니다.");
        return 0; // 실패 시 0 반환
    }

    private void UpdateUI()
    {
        // UI에 계산된 값 표시
        hitsCountText.text = $"맞힌 갯수: 10 x {hitsCount}";
        bonusScoreText.text = $"추가 점수: 0.1 x {bonusScore:F1}"; // 소수점 1자리까지 표시
        totalScoreText.text = $"최종 점수: {totalScore}";
    }
}
