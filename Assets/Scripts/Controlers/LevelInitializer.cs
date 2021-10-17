using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInitializer : MonoBehaviour
{

    [Tooltip("when active, doesn't trigger the end game animation and keep an empty scene")]
    public bool testMode = true;

    [Tooltip("Make the blocks slightly float")]
    public bool floatingBlocks = true;

    public string dataPath = "/LevelData/Levels.xml";
    public Transform previousLevelContainer;
    public Transform currentLevelContainer;
    public Transform nextLevelContainer;

    [Tooltip("instanciate the specified level")]
    public bool instanciateLevel;

    [Tooltip("size of one unit in xml in unity environment")]
    public int gridSize = 1;

    [Tooltip("Time it takes to slide the levels when they chage")]
    public float LevelChangingSpeed = 1f;

    [System.Serializable]
    public class PrefabContainer
    {
        [Header("Slab Prefabs")]
        public GameObject PermutableSlabPrefab;
        public GameObject RotativeSlabPrefab;
        public GameObject StaticSlabPrefab;

        [Space(10)]
        [Header("Circuit Slab Prefabs")]
        public GameObject EmptyCircuitSlabPrefab;
        public GameObject EndCircuitSlabPrefab;
        public GameObject LineCircuitSlabPrefab;
        public GameObject TurnCircuitSlabPrefab;
        public GameObject TCircuitSlabPrefab;
        public GameObject CrossCircuitSlabPrefab;

        [Space(10)]
        [Header("Circuit Slab Prefabs without colored edges")]
        public GameObject EmptyCircuitSlabNoEdgesPrefab;
        public GameObject EndCircuitSlabNoEdgesPrefab;
        public GameObject LineCircuitSlabNoEdgesPrefab;
        public GameObject TurnCircuitSlabNoEdgesPrefab;
        public GameObject TCircuitSlabNoEdgesPrefab;
        public GameObject CrossCircuitSlabNoEdgesPrefab;

        [Space(10)]
        [Header("Other Slab Prefabs")]
        public GameObject StaticCardPrefab;

        [Space(10)]
        [Header("Decorations lab Prefabs")]
        public GameObject SquareDecorationPrefab;
    }

    public PrefabContainer objectPrefabs;
    public LayerMask decorationLayer;


    private LevelContainerXML _levelContainer;
    private int _levelID;
    private LevelXML _currentLevel;

    #region Singleton

    public static LevelInitializer instance;
    
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

    private void Awake()
    {
        CheckSingleton();
    }

    // Use this for initialization
    void Start()
    {

        _levelID = GameManager.instance.levelID;
        Debug.LogWarning(this + ": initializing the level "+_levelID);

        // change floor color
        FloorBackground.instance.ChangeMaterial();
        
        _levelContainer = LevelContainerXML.LoadFromResource();
        _currentLevel = _levelContainer.GetLevel(_levelID);

        // if instanciate level is false, we don't instanciate the level
        if (!instanciateLevel) 
        {
            Debug.Log(this + ": we don't instanciate anything");
            return;
        }
        else if (_currentLevel == null) // if we couldn't find a level at this index
        {
            if (!testMode) // if we are not in test mode trigger end game
            {
                Debug.LogWarning("no level found for id = " + _levelID + ": trigger end game");
                EndGame();
            }
            else // if we are in test mode
            {
                Debug.LogWarning("no level found for id = " + _levelID + ")");
            }
            return; // end of initilisation anyway
        }
        else
        {
            InitCameras(_currentLevel); // camera initialization
            GameManager.instance.levelName = _currentLevel.LevelID; // update the name of the level
            Debug.Log(this + ": loading level " + _levelID + " \"" + _currentLevel.LevelID + "\"");

            CreateLevel(_currentLevel,currentLevelContainer);

            //LevelXML previousLevel = _levelContainer.GetLevel(_levelID - 1);
            //LevelXML nextLevel = _levelContainer.GetLevel(_levelID + 1);
            //if (previousLevel != null)
            //{
            //    GameObject newLevelObj = CreateLevel(previousLevel, previousLevelContainer);
            //    newLevelObj.SetActive(false);
            //}
            //if (nextLevel != null)
            //{
            //    GameObject newLevelObj = CreateLevel(nextLevel, nextLevelContainer);
            //    newLevelObj.SetActive(false);
            //}

        }

        //AddExtraDecorations();
    }

    public void UpdateScene(int levelDifference)
    {
        _levelID = GameManager.instance.levelID;
        Debug.LogWarning(this + ": updating the level to level " + _levelID);


        GameManager.instance.gameIsInteractable = false;
        StartCoroutine(GameManager.instance.MakeGameInteractable(LevelChangingSpeed + 0.05f));

        // update UI
        UIManager.instance.InitUI();
        

        if(_levelID < 0)
        {
            Debug.LogError(this + ": there is no negative level !");
            return;
        }

        // change floor color
        FloorBackground.instance.ChangeMaterial();

        _currentLevel = _levelContainer.GetLevel(_levelID);

        // if instanciate level is false, we don't instanciate the level
        if (!instanciateLevel)
        {
            Debug.Log(this + ": we don't instanciate anything");
            return;
        }
        else if (_currentLevel == null) // if we couldn't find a level at this index
        {
            if (!testMode) // if we are not in test mode
            {
                Debug.LogWarning("no level found for id = " + _levelID + ": trigger end game");
                EndGame();
            }
            else // if we are in test mode
            {
                Debug.LogWarning("no level found for id = " + _levelID + ")");
            }
            return; // end of initilisation anyway
        }
        else
        {
            InitCameras(_currentLevel); // camera initialization
            GameManager.instance.levelName = _currentLevel.LevelID; // update the name of the level
            Debug.Log(this + ": update to level " + _levelID + " \"" + _currentLevel.LevelID + "\"");

            if(levelDifference == 1)
            {
                
                // treat the former current level
                foreach (Transform currentLevel in currentLevelContainer)
                {
                    // change parenting
                    currentLevel.parent = previousLevelContainer;

                    // slide away
                    StartCoroutine(SlideToPosition(currentLevel, previousLevelContainer));

                    // destroy the level after it is out of the screen
                    Destroy(currentLevel.gameObject, LevelChangingSpeed + 0.5f);

                }

                // create the new current level
                LevelXML newLevelInfo = _levelContainer.GetLevel(_levelID);
                if (newLevelInfo != null)
                {
                    // create new obj at the offser position
                    GameObject newLevelObj = CreateLevel(newLevelInfo, nextLevelContainer);

                    // set up parent
                    newLevelObj.transform.parent = currentLevelContainer;

                    // slide the level to the view
                    StartCoroutine(SlideToPosition(newLevelObj.transform, currentLevelContainer));
                }
                else 
                {
                    Debug.LogError(this + ": unexpected null level data");
                    return;
                }
                
            }
            else if(levelDifference == -1)
            {
                // treat the former current level
                foreach (Transform currentLevel in currentLevelContainer)
                {
                    // change parenting
                    currentLevel.parent = nextLevelContainer;

                    // slide away
                    StartCoroutine(SlideToPosition(currentLevel, nextLevelContainer));

                    // destroy the level after it is out of the screen
                    Destroy(currentLevel.gameObject, LevelChangingSpeed + 0.5f);

                }

                // create the new current level
                LevelXML newLevelInfo = _levelContainer.GetLevel(_levelID);
                if (newLevelInfo != null)
                {
                    // create new obj at the offser position
                    GameObject newLevelObj = CreateLevel(newLevelInfo, previousLevelContainer);

                    // set up parent
                    newLevelObj.transform.parent = currentLevelContainer;

                    // slide the level to the view
                    StartCoroutine(SlideToPosition(newLevelObj.transform, currentLevelContainer));
                }
                else
                {
                    Debug.LogError(this + ": unexpected null level data");
                    return;
                }
            }
            else
            {
                Debug.LogError(this + ": Unexpected level difference " + levelDifference);
            }


            
        }
    }



    

    public GameObject CreateLevel(LevelXML levelInfo, Transform levelContainerTransform)
    {
        Debug.Log(this + ": " + _currentLevel.Blocks.Count + " blocks found for this level");

        // create level ton contain the blocks
        GameObject newLevel = new GameObject("Level " + levelInfo.LevelID);
        newLevel.transform.parent = levelContainerTransform;
        newLevel.transform.position = levelContainerTransform.position;

        for (int blockNum = 0; blockNum < _currentLevel.Blocks.Count; blockNum++)
        {
            BlockXML block = levelInfo.Blocks[blockNum];
            Debug.Log(this + ": block " + blockNum + ": " + block.Slabs.Count + " slabs found");

            // create the block object to contain the slabs
            GameObject newBlockObj = new GameObject("Block " + blockNum);
            newBlockObj.transform.parent = newLevel.transform;
            newBlockObj.transform.position = newLevel.transform.position;

            if (floatingBlocks)
            {
                newBlockObj.AddComponent<FloatingObject>();
            }

            foreach (SlabXML slabData in block.Slabs)
            {
                GameObject newSlab = CreateSlab(slabData, newBlockObj.transform);
                CreateCard(slabData, newSlab.transform);
            }
        }
        // fill the level hints
        GameManager.instance.hintOne = levelInfo.FirstHint;
        GameManager.instance.hintTwo = levelInfo.SecondHint;

        return newLevel;
    }



    public GameObject CreateSlab(string slabType, Transform levelContainerTransform, Vector3 position, int orientation, int level = 0)
    {
        GameObject model;
        GameObject instanciatedSlab;
        Slab slabComp;

        int id = Slab.GetNewId();

        switch (slabType)
        {
            case "permutable":  // permutable slab

                model = objectPrefabs.PermutableSlabPrefab;
                break;

            case "rotative": // rotative slab
                model = objectPrefabs.RotativeSlabPrefab;
                break;

            case "static": // static slab
                model = objectPrefabs.StaticSlabPrefab;
                break;

            default:
                model = null;
                break;
        }

        if (model == null)
        {
            Debug.LogError("Error on instanciation : no model found for type \"" + slabType + "\"");
            return null;
        }

        if (currentLevelContainer != null)
        {
            instanciatedSlab = Instantiate(model, position + levelContainerTransform.transform.position, Quaternion.identity);
            instanciatedSlab.transform.SetParent(levelContainerTransform);
        }
        else
        {
            instanciatedSlab = Instantiate(model, position, Quaternion.identity);
        }

        //definition of its rotation
        switch (orientation)
        {
            case 0:
                instanciatedSlab.transform.Rotate(-90f, 0f, -90f);
                break;
            case 1:
                break;
            case 2:
                instanciatedSlab.transform.Rotate(0f, 90f, 90f);
                break;
            default:
                Debug.LogError("Error on instanciation : unkown orientation " + orientation);
                return null;
        }

        // get slab comp
        slabComp = instanciatedSlab.GetComponent<Slab>();

        // set id
        slabComp.id = id;

        // update orientation 
        slabComp.orientation = orientation;

        // set up slab level for permutable slab
        if (slabType == "permutable")
        {
            instanciatedSlab.GetComponent<PermutableSlab>().level = level;
        }

        /*instanciatedCard = */
        //CreateCard(slabInfo.Card, slabInfo, instanciatedSlab);

        Debug.Log("new Slab created: " + instanciatedSlab);
        return instanciatedSlab;
    }

    private GameObject CreateSlab(SlabXML slabInfo, Transform levelContainerTransform)
    {
        Vector3 position = new Vector3(slabInfo.PositionX, slabInfo.PositionY, slabInfo.PositionZ);
        return CreateSlab(slabInfo.SlabType, levelContainerTransform, position, slabInfo.Orientation, slabInfo.SlabLevel);
    }

    // DEPRECATED
    private GameObject CreateSlab_old(SlabXML slabInfo)
    {
        GameObject model;
        Vector3 slabPosition;
        //Quaternion slabRotation;
        GameObject instanciatedSlab;
        //GameObject instanciatedCard;
        Slab slabComp;

        switch (slabInfo.SlabType)
        {
            case "permutable":  // permutable slab

                model = objectPrefabs.PermutableSlabPrefab;
                break;

            case "rotative": // rotative slab
                model = objectPrefabs.RotativeSlabPrefab;
                break;

            case "static": // static slab
                model = objectPrefabs.StaticSlabPrefab;
                break;

            default:
                model = null;
                break;
        }

        if (model == null)
        {
            Debug.LogError("Error on instanciation : no model found for type \"" + slabInfo.SlabType + "\"");
            return null;
        }

        // definition of the slabPosition
        slabPosition = gridSize * new Vector3(slabInfo.PositionX, slabInfo.PositionY, slabInfo.PositionZ);

        if (currentLevelContainer != null)
        {
            instanciatedSlab = Instantiate(model, slabPosition + currentLevelContainer.transform.position, Quaternion.identity);
            instanciatedSlab.transform.SetParent(currentLevelContainer);
        }
        else
        {
            instanciatedSlab = Instantiate(model, slabPosition, Quaternion.identity);
        }

        //definition of its rotation
        switch (slabInfo.Orientation)
        {
            case 0:
                instanciatedSlab.transform.Rotate(-90f, 0f, -90f);
                break;
            case 1:
                break;
            case 2:
                instanciatedSlab.transform.Rotate(0f, 90f, 90f);
                break;
            default:
                Debug.LogError("Error on instanciation : unkown orientation " + slabInfo.Orientation);
                return null;
        }

        // get slab comp
        slabComp = instanciatedSlab.GetComponent<Slab>();

        // update orientation 
        slabComp.orientation = slabInfo.Orientation;

        // set up slab level for permutable slab
        if (slabInfo.SlabType == "permutable")
        {
            instanciatedSlab.GetComponent<PermutableSlab>().level = slabInfo.SlabLevel;
        }

        /*instanciatedCard = */
        //CreateCard(slabInfo.Card, slabInfo, instanciatedSlab);

        Debug.Log("new Slab created: " + slabInfo);
        return instanciatedSlab;
    }

    public GameObject CreateCard(string slabType, Transform parentSlab, string cardType, int cardRotation, CircuitCard.CircuitType circuitType = CircuitCard.CircuitType.Empty)
    {
        //CardXML cardInfo = slabInfo.Card;
        GameObject model;
        Slab parentSlabComp = parentSlab.GetComponent<Slab>();
        Card cardComp;
        //CircuitCard circuitCardComp;

        // get a 
        int id = Card.GetNewId();

        // rotation bias induced by the texture model
        //int rotationBias = 0;

        if (parentSlabComp == null)
            Debug.LogError("Error on card instanciation : wrong parent slab GameObject" + parentSlab);

        // select the proper model
        // update the rotation bias
        switch (cardType)
        {
            case "circuit":  // permutable slab

                if (slabType== "rotative")
                {
                    switch (circuitType)
                    {
                        case CircuitCard.CircuitType.Empty:
                            model = objectPrefabs.EmptyCircuitSlabNoEdgesPrefab;
                            break;
                        case CircuitCard.CircuitType.Line:
                            //rotationBias = -1;
                            model = objectPrefabs.LineCircuitSlabNoEdgesPrefab;
                            break;
                        case CircuitCard.CircuitType.End:
                            //rotationBias = 0;
                            model = objectPrefabs.EndCircuitSlabNoEdgesPrefab;
                            break;
                        case CircuitCard.CircuitType.Turn:
                            //rotationBias = 2;
                            model = objectPrefabs.TurnCircuitSlabNoEdgesPrefab;
                            break;
                        case CircuitCard.CircuitType.T:
                            //rotationBias = -1;
                            model = objectPrefabs.TCircuitSlabNoEdgesPrefab;
                            break;
                        case CircuitCard.CircuitType.Cross:
                            //rotationBias = -1;
                            model = objectPrefabs.CrossCircuitSlabNoEdgesPrefab;
                            break;
                        default:
                            Debug.LogError("Error on card instanciation : no model found for this circuit card type");
                            return null;
                    }
                }
                else
                {
                    switch (circuitType)
                    {
                        case CircuitCard.CircuitType.Empty:
                            model = objectPrefabs.EmptyCircuitSlabPrefab;
                            break;
                        case CircuitCard.CircuitType.Line:
                            //rotationBias = -1;
                            model = objectPrefabs.LineCircuitSlabPrefab;
                            break;
                        case CircuitCard.CircuitType.End:
                            //rotationBias = 0;
                            model = objectPrefabs.EndCircuitSlabPrefab;
                            break;
                        case CircuitCard.CircuitType.Turn:
                            //rotationBias = 2;
                            model = objectPrefabs.TurnCircuitSlabPrefab;
                            break;
                        case CircuitCard.CircuitType.T:
                            //rotationBias = -1;
                            model = objectPrefabs.TCircuitSlabPrefab;
                            break;
                        case CircuitCard.CircuitType.Cross:
                            //rotationBias = -1;
                            model = objectPrefabs.CrossCircuitSlabPrefab;
                            break;
                        default:
                            Debug.LogError("Error on card instanciation : no model found for this circuit card type");
                            return null;
                    }
                }


                break;

            case "static":
                model = objectPrefabs.StaticCardPrefab;
                break;

            default:
                model = null;
                break;
        }

        if (model == null)
        {
            Debug.LogWarning("Ccard instanciation : no model found for card type \"" + cardType + "\"");
            return null;
        }


        GameObject instanciatedCard = Instantiate(model, parentSlab);
        cardComp = instanciatedCard.GetComponent<Card>();
        //circuitCardComp = instanciatedCard.GetComponent<CircuitCard>();

        // set up the id
        cardComp.id = id;

        // rotate the card according to its rotation state and texture bias
        cardComp.rotation = cardRotation;

        float rotationAngle = -90f * cardRotation;
        //float rotationAngle = 0f;

        instanciatedCard.transform.Rotate(Vector3.up, rotationAngle);

        cardComp.supportSlab = parentSlabComp;
        parentSlabComp.card = cardComp;

        return instanciatedCard;
    }

    public GameObject CreateCard(SlabXML slabInfo, Transform parentSlab)
    {
        CircuitCard.CircuitType circuitType = CircuitCard.StringToType(slabInfo.Card.CircuitType);
        return CreateCard(slabInfo.SlabType, parentSlab, slabInfo.Card.CardType, slabInfo.Card.Rotation, circuitType);
    }

    // DEPRECATED
    private GameObject CreateCard_old(SlabXML slabInfo, GameObject parentSlabGO)
    {
        CardXML cardInfo = slabInfo.Card;
        GameObject model;
        Slab parentSlabComp = parentSlabGO.GetComponent<Slab>();
        Card cardComp;
        //CircuitCard circuitCardComp;

        // get a 
        int id = Card.GetNewId();

        // rotation bias induced by the texture model
        //int rotationBias = 0;

        if (parentSlabComp == null)
            Debug.LogError("Error on card instanciation : wrong parent slab GameObject" + parentSlabGO);

        // select the proper model
        // update the rotation bias
        switch (cardInfo.CardType)
        {
            case "circuit":  // permutable slab

                if (slabInfo.SlabType == "rotative")
                {
                    switch (cardInfo.CircuitType)
                    {
                        case "Empty":
                            model = objectPrefabs.EmptyCircuitSlabNoEdgesPrefab;
                            break;
                        case "Line":
                            //rotationBias = -1;
                            model = objectPrefabs.LineCircuitSlabNoEdgesPrefab;
                            break;
                        case "End":
                            //rotationBias = 0;
                            model = objectPrefabs.EndCircuitSlabNoEdgesPrefab;
                            break;
                        case "Turn":
                            //rotationBias = 2;
                            model = objectPrefabs.TurnCircuitSlabNoEdgesPrefab;
                            break;
                        case "T":
                            //rotationBias = -1;
                            model = objectPrefabs.TCircuitSlabNoEdgesPrefab;
                            break;
                        case "Cross":
                            //rotationBias = -1;
                            model = objectPrefabs.CrossCircuitSlabNoEdgesPrefab;
                            break;
                        default:
                            Debug.LogError("Error on card instanciation : no model found for circuit card type \"" + cardInfo.CircuitType + "\"");
                            return null;
                    }
                }
                else
                {
                    switch (cardInfo.CircuitType)
                    {
                        case "Empty":
                            model = objectPrefabs.EmptyCircuitSlabPrefab;
                            break;
                        case "Line":
                            //rotationBias = -1;
                            model = objectPrefabs.LineCircuitSlabPrefab;
                            break;
                        case "End":
                            //rotationBias = 0;
                            model = objectPrefabs.EndCircuitSlabPrefab;
                            break;
                        case "Turn":
                            //rotationBias = 2;
                            model = objectPrefabs.TurnCircuitSlabPrefab;
                            break;
                        case "T":
                            //rotationBias = -1;
                            model = objectPrefabs.TCircuitSlabPrefab;
                            break;
                        case "Cross":
                            //rotationBias = -1;
                            model = objectPrefabs.CrossCircuitSlabPrefab;
                            break;
                        default:
                            Debug.LogError("Error on card instanciation : no model found for circuit card type \"" + cardInfo.CircuitType + "\"");
                            return null;
                    }
                }


                break;

            case "static":
                model = objectPrefabs.StaticCardPrefab;
                break;

            default:
                model = null;
                break;
        }

        if (model == null)
        {
            Debug.LogWarning("Ccard instanciation : no model found for card type \"" + cardInfo.CardType + "\"");
            return null;
        }


        GameObject instanciatedCard = Instantiate(model, parentSlabGO.transform);
        cardComp = instanciatedCard.GetComponent<Card>();
        //circuitCardComp = instanciatedCard.GetComponent<CircuitCard>();

        // set up the id
        cardComp.id = id;

        // rotate the card according to its rotation state and texture bias
        cardComp.rotation = cardInfo.Rotation;

        float rotationAngle = -90f * cardInfo.Rotation;
        //float rotationAngle = 0f;

        instanciatedCard.transform.Rotate(Vector3.up, rotationAngle);

        cardComp.supportSlab = parentSlabComp;
        parentSlabComp.card = cardComp;



        return null;
    }

    private void AddExtraDecorations()
    {
        float size = 2f;
        float decorationSize = 0.5f;
        float hitDistance = size * 3 / 2; // size * 3 / 4;
        RaycastHit hit;
        Transform decorationContainer;
        GameObject[] decorations = GameObject.FindGameObjectsWithTag("Decoration");


        foreach (GameObject decoration in decorations)
        {
            decorationContainer = decoration.transform.parent;
            if (decorationContainer == null)
            {
                Debug.LogWarning(decoration + ": this slab doesn't have a decoration container, creating one");
                GameObject newContainer = new GameObject("Decorations");
                newContainer.transform.position = decoration.transform.position;
                newContainer.transform.rotation = decoration.transform.rotation;
                newContainer.transform.SetParent(decoration.transform);
                decorationContainer = newContainer.transform;
            }

            for (int dir = 0; dir < 4; dir++)
            {
                Vector3 direction;
                Collider collider = decoration.GetComponent<Collider>();
                switch (dir)
                {
                    case 0:
                        direction = new Vector3(1f, 0f, 1f);
                        break;
                    case 1:
                        direction = new Vector3(1f, 0f, -1f);
                        break;
                    case 2:
                        direction = new Vector3(-1f, 0f, -1f);
                        break;
                    case 3:
                        direction = new Vector3(-1f, 0f, 1f);
                        break;
                    default:
                        direction = Vector3.up;
                        break;
                }

                collider.enabled = false;
                bool isHit = Physics.Raycast(decoration.transform.position, decoration.transform.TransformDirection(direction), out hit, 3f, decorationLayer);
                Debug.DrawRay(transform.position, direction.normalized * hit.distance, Color.green, 5);
                collider.enabled = true;

                if (isHit)
                {
                    Debug.Log(gameObject + ": has hit: " + hit.collider.gameObject + "; create extra deco");
                    Vector3 offset = 0.5f * (size + decorationSize) * Vector3.right * (dir < 2 ? 1 : -1);


                    GameObject newDecoration = Instantiate(objectPrefabs.SquareDecorationPrefab, decoration.transform.position, decoration.transform.rotation) as GameObject;
                    newDecoration.transform.Translate(offset);
                    newDecoration.transform.SetParent(decorationContainer);
                }

            }


        }
    }



    private static void InitCameras(LevelXML levelInfo)
    {
        CameraControler.instance.SetParameters(levelInfo.XCameraOffset, levelInfo.YCameraOffset, levelInfo.CameraSize,1f);
    }

    private void EndGame()
    {
        UIManager.instance.EndGameLayout();
    }

#region Useful Coroutines

    private IEnumerator SlideToPosition(Transform objectToMove, Transform destination)
    {
        float transitionTime = LevelChangingSpeed;
        float startTime = 0f;
        Vector3 initialPosition = objectToMove.position;

        while ((objectToMove.position - destination.position).magnitude > 0)
        {
            startTime += Time.deltaTime;
            objectToMove.position = Vector3.Lerp(initialPosition, destination.position, Mathf.Clamp(startTime / transitionTime, 0f, 1f));
            yield return null;
        }
    }

    private IEnumerator SetInactiveAfterTime(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        obj.SetActive(false);
    }


    #endregion

}
