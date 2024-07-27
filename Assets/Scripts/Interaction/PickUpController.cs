using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    [SerializeField] float moveTime;
    [SerializeField] LayerMask interactibleLayer;

    [SerializeField] bool moveOnGrid;

    Interactible currentInteractible;

    Vector2 mousePosOffset;

    Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) OnClick();
        if (Input.GetKeyUp(KeyCode.Mouse0)) currentInteractible = null;
    }

    private void FixedUpdate()
    {
        Interact();
    }

    void OnClick()
    {
        RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, interactibleLayer);
        if (hit && hit.transform.TryGetComponent(out Interactible current))
        {
            currentInteractible = current;

            mousePosOffset = currentInteractible.transform.position - cam.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    void Interact()
    {
        if (Input.GetKey(KeyCode.Mouse0) && currentInteractible)
        {
            Vector2 currentMousePos = (Vector2)cam.ScreenToWorldPoint(Input.mousePosition) + mousePosOffset;
            if(moveOnGrid) currentMousePos = Vector2Int.RoundToInt(currentMousePos);

            Vector2 direction = currentMousePos - (Vector2)currentInteractible.transform.position;

            currentInteractible.Move(direction, moveTime);
        }
    }
}