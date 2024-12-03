using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject gameMode;

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
}
