using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI attemptCounter;
    [SerializeField] private TextMeshProUGUI stageCounter;
    [SerializeField] private TextMeshProUGUI targetCounter;
    [SerializeField] private TextMeshProUGUI messageToast;
    [SerializeField] private int attemptsLeft;
    [SerializeField] public int levelNo;
    [SerializeField] public int targetNoLeft;
    [SerializeField] public bool playerCanNowMove;
    [SerializeField] private GameObject slingshotGuide;
    [SerializeField] private GameObject cameraHoverGuide;
    [SerializeField] private GameObject levelGameObject;
    [SerializeField] private List<GameObject> levelList;
    [SerializeField] private TextMeshProUGUI cameraXEndEarlyToast;

    public static GameManager Instance { get; private set; }

    public static Action OnLevelChange;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        Slingshot.ObjectLaunchedGotSuccess += SuccessLaunchCheck;
        TargetObjectScript.OnTargetStateChange += UpdateTargetHUD;
    }

    private void OnDisable()
    {
        Slingshot.ObjectLaunchedGotSuccess -= SuccessLaunchCheck;
        TargetObjectScript.OnTargetStateChange -= UpdateTargetHUD;
    }

    void Start()
    {
        FirstGame();
        cameraXEndEarlyToast = cameraHoverGuide.GetComponentInChildren<TextMeshProUGUI>();

        if (attemptCounter == null)
            attemptCounter = GameObject.FindGameObjectWithTag("attemptCounter").GetComponent<TextMeshProUGUI>();
        attemptsLeft = 5;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("W pressed: Declaring next level");
            DeclareNextLevel();
        }
    }

    private void SuccessLaunchCheck(bool isSuccessful)
    {
        if (!isSuccessful) attemptsLeft--;
        UpdateAttemptHUD();
    }

    private void UpdateAttemptHUD()
    {
        attemptCounter.text = attemptsLeft.ToString();

        if (attemptsLeft <= 0)
        {
            if (levelNo != 1) SceneManager.LoadScene("GameOverScene");
            else SceneManager.LoadScene("GameScene");
        }
    }

    void FirstGame()
    {
        levelNo = 1;
        for (int i = 0; i < levelList.Count; i++)
        {
            levelList[i].SetActive(false);
        }

        levelList[levelNo - 1].SetActive(true);
        levelGameObject = Instantiate(levelList[levelNo - 1], Vector3.zero, Quaternion.identity);
        playerCanNowMove = false;
        UpdateHUD();
        UpdateAttemptHUD();
        slingshotGuide.SetActive(false);
        cameraHoverGuide.SetActive(false);
        StartCoroutine(DisplayMessageCurrentLevelCoroutine());
    }

    void UpdateHUD()
    {
        stageCounter.text = levelNo.ToString();
    }

    void UpdateTargetHUD(int paramTargetStateChange)
    {
        targetNoLeft += paramTargetStateChange;
        targetCounter.text = targetNoLeft.ToString();

        if (targetNoLeft <= 0)
        {
            StartCoroutine(DisplayMessageLevelCompleteCoroutine());
            playerCanNowMove = false;
        }
    }

    public void DeclareNextLevel()
    {
        Destroy(levelGameObject);
        levelNo++;
        levelGameObject = Instantiate(levelList[levelNo - 1], Vector3.zero, Quaternion.identity);
        Debug.Log("Level Game Object exist?: " + (levelGameObject != null));
        levelGameObject.SetActive(true);
        OnLevelChange?.Invoke();
        attemptsLeft = 5;
        UpdateHUD();
        UpdateAttemptHUD();
        StartCoroutine(DisplayMessageCurrentLevelCoroutine());
    }

    IEnumerator DisplayMessageCurrentLevelCoroutine()
    {
        messageToast.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        messageToast.text = "Level " + levelNo;
        messageToast.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        messageToast.gameObject.SetActive(false);
        playerCanNowMove = true;
        yield return new WaitForSeconds(1f);
        slingshotGuide.SetActive(true);
        cameraXEndEarlyToast.text = "Hover Right";
        cameraHoverGuide.SetActive(true);
    }

    IEnumerator DisplayMessageLevelCompleteCoroutine()
    {
        messageToast.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        messageToast.text = "Level " + levelNo + " Complete!";
        messageToast.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        messageToast.gameObject.SetActive(false);

        if (levelNo == GameManager.Instance.levelList.Count)
            SceneManager.LoadScene("GameCompleteScene");
        else DeclareNextLevel();
    }
}
