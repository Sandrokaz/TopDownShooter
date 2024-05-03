using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomRoom : MonoBehaviour
{
    public static CustomRoom Instance;
    [SerializeField] private List<Transform> spawnPoints;
    public List<Transform> SpawnPoints { get => spawnPoints; set => spawnPoints = value; }
	public float HalfXScale { get => halfXScale; set => halfXScale = value; }
	public float HalfZScale { get => halfZScale; set => halfZScale = value; }
	private float halfXScale = 50;
	private float halfZScale = 37.5f;

	private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
}
