using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI attemptCounter;
    [SerializeField] private TextMeshProUGUI stageCounter;
    [SerializeField] private TextMeshProUGUI targetCounter;
    [SerializeField] private int attemptsLeft;
    [SerializeField] private int stageNo;
    [SerializeField] public int targetNoLeft;


    public static GameManager Instance { get; private set; }

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
        stageNo = 1;
        UpdateHUD();
        UpdateAttemptHUD();
    }

    void UpdateHUD()
    {
        stageCounter.text = stageNo.ToString();
    }

    void UpdateTargetHUD(int paramTargetStateChange)
    {
        targetNoLeft += paramTargetStateChange;
        targetCounter.text = targetNoLeft.ToString();
    }
}
