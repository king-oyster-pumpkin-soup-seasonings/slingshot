using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI attemptCounter;
    [SerializeField] private int attemptsLeft;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        Slingshot.ObjectLaunchedGotSuccess += SuccessLaunchCheck;
    }

    private void OnDisable()
    {
        Slingshot.ObjectLaunchedGotSuccess -= SuccessLaunchCheck;
    }

    void Start()
    {
        if (attemptCounter == null)
            attemptCounter = GameObject.FindGameObjectWithTag("attemptCounter").GetComponent<TextMeshProUGUI>();
        attemptsLeft = 5;
        UpdateAttemptHUD();
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
}
