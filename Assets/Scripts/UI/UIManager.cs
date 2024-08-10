using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    FadePanel fadePanel;

    public static UIManager Instance;

    private void Awake()
    {
        fadePanel = FindObjectOfType<FadePanel>();

        Instance = this;
    }

    public void ReloadButton()
    {
        LevelManager.ReloadCurrentScene();
    }

    public void NextButton()
    {
        LevelManager.LoadNextScene();
    }

    #region FadePanel
    public void FadeIn()
    {
        fadePanel.FadeIn();
    }
    public void FadeOut()
    {
        fadePanel.FadeOut();
    }

    public void SetCirclePos(Vector2 position)
    {
        fadePanel.SetCirclePos(position);
    }
    #endregion
}
