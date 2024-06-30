using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip SkeletonHit;
    [SerializeField]
    private AudioClip Combo_1, Combo_2, Combo_3;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void OnSkeletonHit()
    {
        audioSource.clip = SkeletonHit;
        audioSource.time = 0.2f;
        audioSource.Play();
    }
    public void OnCombo(int combo)
    {
        switch (combo)
        {
            case 1:
                audioSource.clip = Combo_1;
                audioSource.time = 0.1f;
                audioSource.Play();
                break;
            case 2:
                audioSource.clip = Combo_2;
                audioSource.time = 0.1f;
                audioSource.Play();
                break;
            case 3:
                audioSource.clip = Combo_3;
                audioSource.time = 0.1f;
                audioSource.Play();
                break;
            default:
                break;
        }
    }
}
