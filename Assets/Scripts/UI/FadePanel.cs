using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FadePanel : MonoBehaviour
{
    [SerializeField] RectTransform circle;
    [SerializeField] RectTransform background;

    Camera cam;

    public static FadePanel Instance;

    private void Awake()
    {
        cam = Camera.main;

        Instance = this;
    }

    private void Start()
    {
        background.sizeDelta = new Vector2(Screen.width * 1.1f, Screen.height * 1.1f);
    }

    public void SetCirclePos(Vector2 worldPosition)
    {
        circle.position = cam.WorldToScreenPoint(worldPosition);
        background.position = cam.WorldToScreenPoint(new Vector2(-circle.rect.x, -circle.rect.y));
    }

    public void FadeIn()
    {
        circle.gameObject.SetActive(true);

        float screenSize = Screen.width > Screen.height ? Screen.width : Screen.height;
        float targetSize = screenSize / 2.7f;
             
        circle.DOSizeDelta(new Vector2(targetSize,targetSize), .8f).OnComplete(()=>
        {
            targetSize = screenSize / 3f;
            circle.DOSizeDelta(new Vector2(targetSize, targetSize), .8f).OnComplete(() =>
            {
                Vector2 position = new Vector2(Screen.width / 2, Screen.height / 2);
                targetSize = screenSize * 1.5f;
                float time = 1.5f;

                circle.DOMove(position, time);
                background.DOMove(position, time);
                circle.DOSizeDelta(new Vector2(targetSize, targetSize), time)
                    .OnComplete(()=>circle.gameObject.SetActive(false));
            });
        });
    }
    public void FadeOut()
    {
        circle.gameObject.SetActive(true);

        float targetSize = 0;
        circle.DOSizeDelta(new Vector2(targetSize, targetSize), 1.2f);
    }
}