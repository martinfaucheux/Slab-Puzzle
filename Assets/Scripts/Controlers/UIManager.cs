using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [Tooltip("Text Component which contains the level id for the main UI")]
    public Text levelText;

    [Tooltip("Text Component which contains the level id for the pause menu")]
    public Text menuLevelText;

    [Tooltip("Text Component which contains the action counter")]
    public Text actionCounterText;

    [Tooltip("Text Component which contains the time")]
    public Text timeText;

    [Tooltip("Text Component which contains the next level message")]
    public Text nextLevelText;

    [Tooltip("Text Component which contains the end game message")]
    public Text endGameText;

    [Tooltip("Text Component which contains the first hint for the first level")]
    public Text hintOne;

    [Tooltip("Text Component which contains the second hint for the first level")]
    public Text hintTwo;
    [Tooltip("Text Component which contains the second hint for the first level")]




    public GameObject pauseMenuPanel;
    public Button nextLevelButton;
    public Button previousLevelButton;




    public bool stopTimerUpdate = false;



    #region Singleton

    public static UIManager instance;

    private void CheckSingleton()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    private void Start()
    {
        CheckSingleton();

        if (levelText == null)
            Debug.LogError("No Text component found to display the level");

        if (actionCounterText == null)
            Debug.LogError("No Text component found to display the counter");

        if (timeText == null)
            Debug.LogError("No Text component found to display the time");

        if (nextLevelText == null)
            Debug.LogError("No Text component found to display the next level message");

        if (endGameText == null)
            Debug.LogError("No Text component found to display the end game message");

        if (hintOne == null)
            Debug.LogError("No Text component found to display the level first hint");

        if (hintTwo == null)
            Debug.LogError("No Text component found to display the level second hint");

        InitUI();

    }

    public void InitUI()
    {
        levelText.text = "Level " + GameManager.instance.levelID;
        menuLevelText.text = "Level " + GameManager.instance.levelID;
        hintOne.text = GameManager.instance.hintOne;
        hintTwo.text = GameManager.instance.hintTwo;

        //pauseMenuPanel.SetActive(false);

        int levelID = GameManager.instance.levelID;
        stopTimerUpdate = false;

        // show hints
        TriggerHint();

        // check if there is a previous level
        previousLevelButton.interactable = ((levelID == 0) ? false : true);

        // check if there is a next level
        if (levelID == PlayerPrefManager.GetLastLevel())
        {
            nextLevelButton.interactable = false;
        }
        else
        {
            nextLevelButton.interactable = true;
        }
    }


    private void Update()
    {
        if (timeText != null && !stopTimerUpdate)
            timeText.text = FormatTime(GameManager.instance.timeInLevel);

        actionCounterText.text = GameManager.instance.actionCounter.ToString();
    }






    private static string FormatTime(float timer)
    {
        string str = "";

        string minutes = Mathf.Floor(timer / 60).ToString("00");
        string seconds = (timer % 60).ToString("00");

        str = minutes + ":" + seconds;

        return str;
    }

    public void NextLevelLayout()
    {
        Debug.Log("UI Manager: changing layout to next level");
        this.nextLevelText.enabled = true;

        Debug.Log("next level text is active: " + nextLevelText.IsActive().ToString());
        nextLevelText.GetComponent<Animator>().SetTrigger("appear");
    }

    public void ClearNextLevelLayout()
    {
        Debug.Log(this + ": clearing UI");
        Debug.Log(this + ": start fading out animation");
        nextLevelText.GetComponent<Animator>().SetTrigger("disappear");
        Invoke("EnableNextLevelText", 2.5f);


    }

    // to disable the text component with delay
    private void EnableNextLevelText()
    {
        this.nextLevelText.enabled = false;
    }

    // to disable the text component with delay
    private void DisableHints()
    {
        this.hintOne.enabled = false;
        this.hintTwo.enabled = false;
    }

    public void EndGameLayout()
    {
        Debug.Log("UI Manager: changing layout to end game");
        this.timeText.enabled = false;
        this.actionCounterText.enabled = false;
        this.levelText.enabled = false;
        this.nextLevelText.enabled = false;
        this.endGameText.enabled = true;

        Debug.Log("end game text is active: " + endGameText.IsActive().ToString());
        endGameText.GetComponent<Animator>().SetTrigger("endGame");

    }

    // use to displays hints for the first level
    // need to appear / disappear the same way than nextLevelMessage
    public void TriggerHint()
    {
        Debug.Log("UI Manager: showing hint for level 1");
        this.hintOne.enabled = true;
        this.hintTwo.enabled = true;

        hintOne.GetComponent<Animator>().SetTrigger("appear");
        hintTwo.GetComponent<Animator>().SetTrigger("appear");
    }

    // use to hide hints for the first level
    // need to appear / disappear the same way than nextLevelMessage
    public void HideHint()
    {
        Debug.Log("UI Manager: showing hint for level 1");
        this.hintOne.enabled = false;
        this.hintTwo.enabled = false;

        hintOne.GetComponent<Animator>().SetTrigger("disappear");
        hintTwo.GetComponent<Animator>().SetTrigger("disappear");
    }
}
