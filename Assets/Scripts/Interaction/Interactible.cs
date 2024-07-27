using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Interactible : MonoBehaviour
{
    [Header("Movement type")]
    [SerializeField] bool horizontal;
    [SerializeField] bool vertical;

    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (!horizontal && !vertical) rb.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
        else if (horizontal && !vertical) rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        else if (!horizontal && vertical) rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        else rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void Move(Vector2 direction, float moveTime)
    {
        if (!horizontal) direction.x = 0;
        if(!vertical) direction.y = 0;

        rb.velocity = direction / moveTime;
    }
}