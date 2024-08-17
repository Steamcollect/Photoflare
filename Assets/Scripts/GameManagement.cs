using UnityEngine;

public class GameManagement : MonoBehaviour
{
    [SerializeField] LevelConfig level;

    [Header("System References")]
    [SerializeField] GameObject managementPrefabs;

    private void Awake()
    {
        SetupDontDestroyOnLoad();
    }

    private void Start()
    {
        UIManager.Instance.FadeIn();
        AudioManager.Instance.SetMusicsGO(level.playlist);

        Destroy(gameObject);
    }

    void SetupDontDestroyOnLoad()
    {
        GameObject management = GameObject.FindGameObjectWithTag("GameManagement");

        if (!management)
        {
            DontDestroyOnLoad(Instantiate(managementPrefabs));
        }
    }
}