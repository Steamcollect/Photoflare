using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Configs", menuName = "Configs/Level")]
public class LevelConfig : ScriptableObject
{
    [SerializeField] string levelName;

    [Header("Musics")]
    public AudioManager.Playlist[] playlist;
}