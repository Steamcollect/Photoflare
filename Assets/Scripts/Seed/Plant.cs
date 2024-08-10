using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    [SerializeField] float growUpTime;
    float timer;

    private void Start()
    {
        UIManager.Instance.SetCirclePos(transform.position);
        UIManager.Instance.FadeIn();
    }

    public void GrowUp()
    {
        timer += Time.deltaTime;

        if (timer >= growUpTime)
        {
            //print("Plant grow completly");
        }
    }
}