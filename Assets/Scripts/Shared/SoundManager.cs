using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    GameSuccess,
    GameFailure,
    SelectLetter,
    DropLetter,
    CoinCount
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] List<AudioSource> m_audioDataPool;

    // Game sounds
    [SerializeField] AudioClip m_successClip;
    [SerializeField] AudioClip m_failClip;
    [SerializeField] AudioClip m_selectLetterClip;
    [SerializeField] AudioClip m_dropLetterClip;
    [SerializeField] AudioClip m_coinCountClip;

    void Awake()
    {
        if (Instance != null)
            Destroy(Instance);
        else
            Instance = this;
    }

    public void PlaySound(SoundType type)
    {
        const string METHOD = "SoundManager-PlaySound";
        switch (type)
        {
            case SoundType.GameSuccess:
                Play(m_successClip);
                break;
            case SoundType.GameFailure:
                Play(m_failClip);
                break;
            case SoundType.SelectLetter:
                Play(m_selectLetterClip);
                break;
            case SoundType.DropLetter:
                Play(m_dropLetterClip);
                break;
            case SoundType.CoinCount:
                Play(m_coinCountClip);
                break;
            default:
                Logger.Log(METHOD, "Sound type is not found.");
                return;
        }
    }

    void Play(AudioClip audioClip)
    {
        const string METHOD = "SoundManager-Play";

        foreach (AudioSource audioSource in m_audioDataPool)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = audioClip;

                try
                {
                    audioSource.Play(0);
                }
                catch (Exception e)
                {
                    Logger.Log(METHOD, "Couldn't play sound: " + e.ToString());
                }

                return;
            }
        }
    }
}
