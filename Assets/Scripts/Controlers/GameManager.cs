using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour {



    public bool forceLevel = false;
    public bool runGame = true;
    public int levelID;
    public string levelName;

    public int actionCounter = 0;

    // time spent in the current level
    public float timeInLevel = 0f;

    // hints
    public string hintOne = "";
    public string hintTwo = "";


    public List<GameObject> CardList;

    public LayerMask cardLayerMask;
    //public Camera mainCamera;
    //public Camera[] camerasToControl;

    private bool gameIsWon = false;
    private bool gameIsPaused = false;
    [HideInInspector] public bool gameIsInteractable = true;

    private float _rayMaxDistance = 100;
    //private CameraControler _mainCameraControler;

    
    #region Singleton

    public static GameManager instance;


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


    


    // Singleton pattern
    void Awake () {

        CheckSingleton();

        if(!forceLevel)
            levelID = PlayerPrefManager.GetCurrentLevel();
    }

    private void Start()
    {
        Debug.LogWarning("Game started");

        ResetManager();
    }


    private void InitCardList()
    {
        GameObject[] GOArray = GameObject.FindGameObjectsWithTag("Card");
        foreach(GameObject GO in GOArray)
        {
            CardList.Add(GO);
        }
    }



    // Update is called once per frame
    void Update()
    {
        if (!runGame) // if we dont want the game to run
            return;

        // if there is some movement, we'll have to check victory conditions
        bool somethingHappened = false;

        // update time in level
        if(gameIsInteractable && !gameIsPaused && !gameIsWon)
        {
            timeInLevel += Time.deltaTime;
        }

        if (!gameIsWon && Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        if (gameIsInteractable && !gameIsPaused &&  Input.GetButtonDown("Fire1"))
        {
            // TODO
            if (!gameIsWon) // if the game is not won yet
            {
                GameObject touchedObj = GetObjTouched();
                if (touchedObj != null)
                {
                    Card touchedCard = touchedObj.GetComponent<Card>();
                    Debug.Log(gameObject + " mouse button touched; touched object: " + touchedObj + "; touched card: " + touchedCard);
                    if (touchedCard != null && touchedCard.supportSlab.isInteractable) // if we touched a card and it is interactable
                    {
                        somethingHappened = touchedCard.supportSlab.OnTouch();
                    }
                }
                else // if we didn't hit anything
                {
                    // unselect the selected slab is there is one
                    PermutableSlab.ResetSelected();
                }

                if (somethingHappened)
                {
                    //Debug.Log(gameObject + ": something happened: " + somethingHappened);

                    // update action counter
                    actionCounter++;

                    // check if game is won
                    bool victory = CheckVictoryCondition();

                    if (victory) // if game is won
                    {
                        Debug.Log("victory");

                        // trigger zooming
                        CameraControler.instance.TriggerSmoothZooming();

                        // stop floating objects
                        foreach(FloatingObject floatingComp in GameObject.FindObjectsOfType<FloatingObject>())
                        {
                            floatingComp.StopFloating();
                        }

                        // stop timer
                        UIManager.instance.stopTimerUpdate = true;

                        // diplay win text
                        UIManager.instance.NextLevelLayout();

                        // hide the hints
                        UIManager.instance.HideHint();
                        Debug.LogWarning("Game is won ! Wait for a click...");

                        gameIsWon = true; // go wait for a click
                    }
                }
            }
            else // if the game is won and player clicked
            {
                UIManager.instance.ClearNextLevelLayout();
                LoadNextLevel(true);
            }   
        }
    }

    private GameObject GetObjTouched()
    {
        GameObject hitGO = null;

        // cast ray
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // if ray touched
        if (Physics.Raycast(ray, out hit, _rayMaxDistance, cardLayerMask.value))
        {
            hitGO = hit.collider.gameObject;
            Debug.Log(gameObject + ": casting ray; ray successfully hit");
        }
        else
            Debug.Log(gameObject + ": casting ray; ray hit nothing");

        return hitGO;
    }

    // activation checking phase
    // reset all isCheck of circuit cards
    // return true if all cards are activated
    private bool CheckVictoryCondition()
    {
        Debug.Log(gameObject + ": Check victory conditions");
        bool res = true;
        bool circuitBuiltValid = false;

        // update the card connection state
        CircuitCard.UpdateCircuitConnections();



        // build all circuit and check if they are correctly activated
        circuitBuiltValid = Circuit.BuildCircuits();

        // update result
        res = res && circuitBuiltValid;

        return res;
    }

    public void ResetManager()
    {
        timeInLevel = 0;
        actionCounter = 0;
        gameIsWon = false;
        //gameIsPaused = false;

    }

# region GamePause

    public void PauseGame()
    {
        gameIsPaused = true;
        UIManager.instance.pauseMenuPanel.SetActive(true);
    }

    public void ResumeGame()
    {
        if (!gameIsInteractable)
            return;

        UIManager.instance.pauseMenuPanel.SetActive(false);
        gameIsPaused = false;
    }

    public void LoadNextLevel(bool unlockLevel = false)
    {
        if (!gameIsInteractable)
            return;

        if (!gameIsInteractable)
            return;

        // change level id and PlayerPrefs
        levelID++;
        PlayerPrefManager.SetCurrentLevel(levelID);

        if (unlockLevel && PlayerPrefManager.GetLastLevel() < levelID) // if the next level is not unlocked yet and we have the right to unlock it
        {
            PlayerPrefManager.SetLastLevel(levelID);
            PlayerPrefManager.UnlockLevel(levelID);
        }

        // reload scene
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        LevelInitializer.instance.UpdateScene(1);

        ResetManager();

    }

    public void LoadPreviousLevel()
    {

        if (!gameIsInteractable)
            return;

        // change level id and PlayerPrefs
        levelID--;

        if (levelID < 0) // in case we try to load negative levels

        {
            levelID = PlayerPrefManager.GetLastLevel();
            Debug.LogWarning(this + ": try to load negative level. Load last unlocked instead");
        }

        PlayerPrefManager.SetCurrentLevel(levelID);

        // reload scene
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        LevelInitializer.instance.UpdateScene(-1);

        ResetManager();
    }

    #endregion


#region Useful Coroutines

    public IEnumerator MakeGameInteractable(float time)
    {
        yield return new WaitForSeconds(time);
        gameIsInteractable = true;
    }


#endregion

}
