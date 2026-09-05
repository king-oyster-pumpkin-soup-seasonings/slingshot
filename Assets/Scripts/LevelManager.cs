using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private string sceneName;

    public void ChangeScene()
    {
        if (sceneName is null) return;

        SceneManager.LoadScene(sceneName);
    }
}
