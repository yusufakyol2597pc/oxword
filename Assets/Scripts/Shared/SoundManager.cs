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
    AudioSource m_audioData;

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

        m_audioData = GetComponent<AudioSource>();
    }

    public void PlaySound(SoundType type)
    {
        const string METHOD = "SoundManager-PlaySound";
        switch (type)
        {
            case SoundType.GameSuccess:
                m_audioData.clip = m_successClip;
                break;
            case SoundType.GameFailure:
                m_audioData.clip = m_failClip;
                break;
            case SoundType.SelectLetter:
                m_audioData.clip = m_selectLetterClip;
                break;
            case SoundType.DropLetter:
                m_audioData.clip = m_dropLetterClip;
                break;
            case SoundType.CoinCount:
                m_audioData.clip = m_coinCountClip;
                break;
            default:
                Logger.Log(METHOD, "Sound type is not found.");
                return;
        }

        try
        {
            m_audioData.Play(0);
        }
        catch(Exception e)
        {
            Logger.Log(METHOD, "Couldn't play sound: " + e.ToString());
        }
    }
}
