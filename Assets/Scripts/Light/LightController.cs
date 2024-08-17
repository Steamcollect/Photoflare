using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    [Header("Statistics")]
    float maxLightLenght = 50;
    int maxHitPossible = 10;

    [SerializeField] bool lookSeedOnStart;

    [Header("Layers")]
    [SerializeField] string mirrorTag;
    [SerializeField] string plantTag;
    [SerializeField] string portalTag;

    [Header("References")]
    [SerializeField] LineRenderer lightLinePrefab;
    List<LineRenderer> lightLines = new List<LineRenderer>();

    List<List<LightPoint>> rayLights = new List<List<LightPoint>>();

    Transform seed;

    private void Awake()
    {
        seed = GameObject.FindGameObjectWithTag(plantTag).transform;
    }

    private void Start()
    {
        CreateLightLines(15);

        if(lookSeedOnStart) LookSeed();
    }

    private void Update()
    {
        CalculateLight();
        DrawLight();
    }

    void CalculateLight()
    {
        rayLights.Clear();
        rayLights.Add(new List<LightPoint>());
        rayLights[0].Add(new LightPoint(transform.position, transform.up));

        RayLight(0, 0, maxHitPossible);
    }

    void DrawLight()
    {
        if(rayLights.Count > lightLines.Count)
            CreateLightLines(rayLights.Count - lightLines.Count);

        for (int i = 0; i < lightLines.Count; i++)
        {
            if(i < rayLights.Count)
            {
                LineRenderer currentLine = lightLines[i];
                lightLines[i].gameObject.SetActive(true);
                currentLine.positionCount = 0;

                for (int j = 0; j < rayLights[i].Count; j++)
                {
                    currentLine.positionCount++;
                    currentLine.SetPosition(j, rayLights[i][j].position);
                }
            }
            else
            {
                lightLines[i].gameObject.SetActive(false);
            }
        }
    }

    void RayLight(int rayIndex, int pointIndex, int maxReflection)
    {
        Vector2 position = rayLights[rayIndex][pointIndex].position;
        Vector2 direction = rayLights[rayIndex][pointIndex].direction;

        RaycastHit2D hit = Physics2D.Raycast(position, direction, maxLightLenght);
        
        if (hit)
        {
            maxReflection--;

            if (hit.transform.CompareTag(mirrorTag) && maxReflection > 0)
            {
                //print("Mirror touch");
                
                // Vector reflection calculs
                Vector2 newDirection = Vector2.Reflect(direction, hit.normal);
                Vector2 hitPoint = hit.point + newDirection * .01f;
                rayLights[rayIndex].Add(new LightPoint(hitPoint, newDirection));

                // Same ray, but one point up
                // Calculate new point with reflection
                RayLight(rayIndex, pointIndex + 1, maxReflection);
            }
            else if (hit.transform.CompareTag(portalTag)
                && hit.transform.parent.TryGetComponent(out Portal portal))
            {
                // New ray from other portal
                rayLights[rayIndex].Add(new LightPoint(hit.point, hit.normal));

                rayLights.Add(new List<LightPoint>());
                rayIndex++;
                rayLights[rayIndex].Add(portal.TeleportLight(hit.transform));
                RayLight(rayIndex, 0, maxReflection);
            }
            else if (hit.transform.CompareTag(plantTag)
                && hit.transform.TryGetComponent(out Plant seed))
            {
                //print("Plant touch");

                // Grow the plant if touch
                seed.GrowUp();

                // End raycast
                rayLights[rayIndex].Add(new LightPoint(hit.point, hit.normal));
            }
            else
            {
                //print("Nothing touch");

                rayLights[rayIndex].Add(new LightPoint(hit.point, hit.normal));
            }
        }
    }

    void LookSeed()
    {
        Vector2 lookDir = seed.position - transform.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void CreateLightLines(int count)
    {
        for (int i = 0; i < count; i++)
        {
            LineRenderer current = Instantiate(lightLinePrefab, transform).GetComponent<LineRenderer>();
            current.gameObject.SetActive(false);
            lightLines.Add(current);
        }
    }

    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            for (int i = 0; i < rayLights.Count; i++)
            {
                for (int j = 0; j < rayLights[i].Count; j++)
                {
                    if (j < rayLights[i].Count - 1) Gizmos.DrawLine(rayLights[i][j].position, rayLights[i][j + 1].position);
                }
            }
        }
    }
#endif
}

public class LightPoint
{
    public Vector2 position;
    public Vector2 direction;

    public LightPoint(Vector2 position, Vector2 direction)
    {
        this.position = position;
        this.direction = direction;
    }
}