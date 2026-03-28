using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectDestroyChecker : MonoBehaviour
{
    public string redName = "Enemy_Red";
    public string greenName = "Enemy_Green";
    public string yellowName = "Enemy_Yellow";
    public string playerName = "Player_Blue";

    private bool redDestroyed = false;
    private bool greenDestroyed = false;
    private bool yellowDestroyed = false;
    private bool playerDestroyed = false;

    public GameObject winCanvas;
    public GameObject loseCanvas;

    private void Start()
    {
        if (winCanvas != null) winCanvas.SetActive(false);
        if (loseCanvas != null) loseCanvas.SetActive(false);
    }

    public void NotifyDestroyed(string objName)
    {
        if (objName == redName) redDestroyed = true;
        if (objName == greenName) greenDestroyed = true;
        if (objName == yellowName) yellowDestroyed = true;
        if (objName == playerName) playerDestroyed = true;

        CheckConditions();
    }

    private void CheckConditions()
    {
        if (playerDestroyed && !(redDestroyed && greenDestroyed && yellowDestroyed))
        {
            if (loseCanvas != null)
            {
                loseCanvas.SetActive(true);
            }
            return;
        }

        if (redDestroyed && greenDestroyed && yellowDestroyed && !playerDestroyed)
        {
            if (winCanvas != null)
            {
                winCanvas.SetActive(true);
            }
        }
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadScene("menu");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

