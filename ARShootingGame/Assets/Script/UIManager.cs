using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject gameMode;
    public GameObject howtoPlay;
    public Camera arCamera; // AR 카메라
    public GameObject targetPrefab; // 과녁 오브젝트 프리팹
    public Button placeButton; // 위치 고정 버튼

    private GameObject currentTarget; // 생성된 과녁 오브젝트
    private bool isTargetPlaced = false; // 과녁이 고정되었는지 여부

    public void StartGame()
    {
        // 메인 메뉴 끄기
        mainMenu.SetActive(false);

        // 게임 모드 켜기
        gameMode.SetActive(true);
    }

    public void SelectGameMode()
    {
        // 게임 모드 끄기
        gameMode.SetActive(false);

        // AR 카메라 켜기
        arCamera.gameObject.SetActive(true);

        // 카메라 중앙에 과녁 생성
        CreateTarget();
    }

    public void StartHowToPlay()
    {
        // 메인 메뉴 끄기
        mainMenu.SetActive(false);

        // How to Play 켜기
        howtoPlay.SetActive(true);
    }

    public void InHowtoReturnToMain()
    {
        // How to Play 끄기
        howtoPlay.SetActive(false);

        // 메인 메뉴 켜기
        mainMenu.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        // 모든 UI 끄기
        mainMenu.SetActive(true);
        gameMode.SetActive(false);
        howtoPlay.SetActive(false);
        arCamera.gameObject.SetActive(false);

        // 생성된 과녁 제거
        if (currentTarget != null)
        {
            Destroy(currentTarget);
        }
    }

    private void CreateTarget()
    {
        if (currentTarget == null)
        {
            // 카메라 앞에 과녁 생성
            currentTarget = Instantiate(targetPrefab, arCamera.transform.position + arCamera.transform.forward * 0.5f, UnityEngine.Quaternion.identity);
            currentTarget.transform.parent = arCamera.transform; // 카메라와 함께 움직이도록 설정

            // 고정 버튼 활성화
            placeButton.gameObject.SetActive(true);
        }
    }

    public void PlaceTarget()
    {
        if (currentTarget != null && !isTargetPlaced)
        {
            // 과녁 고정 (카메라 종속 해제)
            currentTarget.transform.parent = null;
            isTargetPlaced = true;

            // 고정 버튼 비활성화
            placeButton.gameObject.SetActive(false);
            UnityEngine.Debug.Log("과녁 위치가 고정되었습니다!");
        }
    }
}
