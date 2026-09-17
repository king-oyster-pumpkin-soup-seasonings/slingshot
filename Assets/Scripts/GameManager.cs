using System;
using System.Collections;
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

        if (attemptCounter == null)
            attemptCounter = GameObject.FindGameObjectWithTag("attemptCounter").GetComponent<TextMeshProUGUI>();
        attemptsLeft = 5;
    }

    private void SuccessLaunchCheck(bool isSuccessful)
    {
        if (!isSuccessful) attemptsLeft--;
        UpdateAttemptHUD();

        if (attemptsLeft <= 0) DeclareResetLevel();
    }

    private void UpdateAttemptHUD()
    {
        attemptCounter.text = attemptsLeft.ToString();
    }

    void DeclareResetLevel()
    {
        SceneManager.LoadScene("GameScene");
    }

    void FirstGame()
    {
        levelNo = 1;
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
    }

    public void DeclareNextLevel()
    {
        levelNo++;
        UpdateHUD();
        OnLevelChange?.Invoke();
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
        cameraHoverGuide.SetActive(true);
    }
}
