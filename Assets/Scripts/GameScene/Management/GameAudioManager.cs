using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAudioManager : MonoBehaviour
{
    public static GameAudioManager Instance;
	AudioSource shootSound;
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
		shootSound = GetComponent<AudioSource>();
    }

	public void PlayShootSound()
	{
		shootSound.Play();
	}
}
