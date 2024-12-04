using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject gameMode;
    public GameObject howtoPlay;

    public void StartGame()
    {
        // main menu scene off
        mainMenu.SetActive(false);

        // game mode scene on
        gameMode.SetActive(true);
    }
    public void ReturnToMainMenu()
    {
        // game mode scene off
        gameMode.SetActive(false);

        // main menu scene on
        mainMenu.SetActive(true);
    }

    public void StartHowToPlay()
    {
        // main menu scene off
        mainMenu.SetActive(false);

        // game mode scene on
        howtoPlay.SetActive(true);
    }

    public void InHowtoReturnToMain()
    {
        howtoPlay.SetActive(false);

        mainMenu.SetActive(true);
    }
}
