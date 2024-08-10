using UnityEngine;

public class GameManagement : MonoBehaviour
{
    [SerializeField] LevelConfig level;

    [SerializeField] GameObject managementPrefabs;
    UIManager uiManager;

    private void Awake()
    {
        SetupDontDestroyOnLoad();

        uiManager.FadeIn();

        Destroy(gameObject);
    }

    void SetupDontDestroyOnLoad()
    {
        GameObject management = GameObject.FindGameObjectWithTag("GameManagement");

        if (!management)
        {
            DontDestroyOnLoad(Instantiate(managementPrefabs));
        }

        uiManager = management.GetComponentInChildren<UIManager>();
    }
}