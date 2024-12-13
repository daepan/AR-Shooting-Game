using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject gameMode;
    public GameObject howtoPlay;
    public Camera arCamera; // AR 카메라
    public GameObject targetObject; // TargetObject 프리팹 (TargetCube와 Canvas 포함)
    public Vector3 offset;
    private GameObject currentTarget; // 생성된 TargetObject

    private bool isTargetPlaced = false; // 타겟이 고정되었는지 여부

    public void StartGame()
    {
        // 메인 메뉴 끄기
        mainMenu.SetActive(false);

        // 게임 모드 켜기
        gameMode.SetActive(true);
    }

    public void SelectGameMode()
    {
        UnityEngine.Debug.Log("게임 모드가 선택되었습니다.");
        // 게임 모드 끄기
        gameMode.SetActive(false);

        // AR 카메라 켜기
        arCamera.gameObject.SetActive(true);
        targetObject.SetActive(true);
        // 타겟 생성
        CreateTargetObject();
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
        // 메인 메뉴 켜기
        mainMenu.SetActive(true);

        // 다른 UI 끄기
        gameMode.SetActive(false);
        howtoPlay.SetActive(false);
        arCamera.gameObject.SetActive(false);

        // 생성된 타겟 제거
        if (currentTarget != null)
        {
            Destroy(currentTarget);
        }

        isTargetPlaced = false;
    }

    private void Start()
    {
        // 3D 오브젝트를 카메라의 자식으로 설정
        targetObject.transform.SetParent(arCamera.transform);
        // 초기 위치 오프셋 설정
        targetObject.transform.localPosition = offset;
    }

    private void Update()
    {
        // 카메라의 위치에 따라 오브젝트의 위치를 업데이트
        targetObject.transform.position = arCamera.transform.position + offset;
        targetObject.transform.LookAt(arCamera.transform); // 카메라를 바라보도록 회전
    }

    private void CreateTargetObject()
    {
        if (currentTarget == null)
        {
            // 타겟 생성 및 카메라 앞에 초기 배치
            Vector3 targetPosition = arCamera.transform.position + arCamera.transform.forward * 0.5f;
            currentTarget = Instantiate(targetObject, targetPosition, Quaternion.identity);

            // Canvas 안의 버튼 활성화
            Button placeButton = currentTarget.GetComponentInChildren<Button>();
            if (placeButton != null)
            {
                placeButton.onClick.AddListener(PlaceTarget); // 버튼 클릭 시 PlaceTarget 메서드 호출
            }

            UnityEngine.Debug.Log("타겟 오브젝트 생성 완료!");
        }
    }

    public void PlaceTarget()
    {
        UnityEngine.Debug.Log("타겟 오브젝트 위치 고정 이벤트 활성!");
        if (currentTarget != null && !isTargetPlaced)
        {
            // 타겟 고정
            isTargetPlaced = true;

            // Canvas 안의 버튼 비활성화
            Button placeButton = currentTarget.GetComponentInChildren<Button>();
            if (placeButton != null)
            {
                placeButton.gameObject.SetActive(false); // 버튼 비활성화
            }

            UnityEngine.Debug.Log("타겟 오브젝트 위치 고정!");
        }
    }

    public void ExitApplication()
    {
        // 빌드된 애플리케이션에서 종료
        Application.Quit();
    }
}
