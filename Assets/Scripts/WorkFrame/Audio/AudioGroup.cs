using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioGroup : MonoBehaviour
{
    public enum AuidioPlayMode
    {
        /// <summary>
        /// �Ƿ������������ͬʱ���� true�Ļ��жϵ�ǰ ������һ��
        /// </summary>
        Single,
        /// <summary>
        /// �Ƿ�ֻ����ͬʱ��һ�� true�Ļ� �����ǰ���ڲ� ���Ქ�����
        /// </summary>
        Only,
        /// <summary>
        /// �Ƿ������������ͬʱ���� true�Ļ����Զ����Чͬʱ����
        /// </summary>
        Multiple,
    }

    public AuidioPlayMode playMode = AuidioPlayMode.Multiple;

    public AudioMixerGroup audioMixerGroup;

    public List<AudioObject> activeAudios = new List<AudioObject>();

    public List<AudioObject> unActiveAudios = new List<AudioObject>();

    public bool isLoop;

    private AudioObject Create()
    {
        AudioObject ao = GMAudioManager.Instance.audioObject.Get();
        ao.transform.SetParent(transform);
        ao.mixerGroup = audioMixerGroup;
        ao.audioSource.loop = isLoop;
        return ao;
    }

    private AudioObject GetAudioObjectFromActive()
    {
        AudioObject ao = null;

        if (activeAudios.Count > 0)
        {
            ao = activeAudios[0];
            ao.Release();
        }
        else if (unActiveAudios.Count > 0)
        {
            ao = unActiveAudios[0];
            ao.Release();
            unActiveAudios.RemoveAt(0);
            activeAudios.Add(ao);
        }
        else
        {
            ao = Create();
            activeAudios.Add(ao);
        }
        return ao;
    }

    private AudioObject GetAudioObjectFromUnActive()
    {
        AudioObject ao = null;

        if (unActiveAudios.Count > 0)
        {
            ao = unActiveAudios[0];
            ao.Release();
            unActiveAudios.RemoveAt(0);
            activeAudios.Add(ao);
        }
        else
        {
            ao = Create();
            activeAudios.Add(ao);
        }
        return ao;
    }

    private AudioObject GetAudioObject()
    {
        AudioObject ao = null;

        if (playMode == AuidioPlayMode.Multiple || (playMode == AuidioPlayMode.Only && activeAudios.Count <= 0))
        {
            ao = GetAudioObjectFromUnActive();
        }
        else if (playMode == AuidioPlayMode.Single)
        {
            ao = GetAudioObjectFromActive();
        }


        return ao;
    }

    public void Play(string assetName)
    {
        AudioObject ao = GetAudioObject();
        if (ao == null) return;
        ao.Play(assetName);
    }

    public void Play(AudioClip audioClip)
    {
        AudioObject ao = GetAudioObject();
        if (ao == null) return;
        ao.Play(audioClip);
    }

    public bool IsPlaying(string assetName)
    {
        foreach (var item in activeAudios)
        {
            if (item.assetName == assetName)
                return true;
        }

        return false;
    }

    public bool IsPlaying(AudioClip audioClip)
    {
        foreach (var item in activeAudios)
        {
            if (item.audioSource.clip == audioClip)
                return true;
        }

        return false;
    }

    public bool Delete(string assetName)
    {
        foreach (var item in activeAudios)
        {
            if (item.assetName == assetName)
            {
                item.Delete();
                return true;
            }
        }

        return false;
    }

    public bool Delete(AudioClip audioClip)
    {
        foreach (var item in activeAudios)
        {
            if (item.audioSource.clip == audioClip)
            {
                item.Delete();
                return true;
            }
        }

        return false;
    }

    private void Update()
    {
        if (Time.frameCount % 60 == 0)
        {
            for (int i = 0; i < activeAudios.Count; i++)
            {
                if (!activeAudios[i].audioSource.isPlaying)
                {
                    unActiveAudios.Add(activeAudios[i]);
                    activeAudios.RemoveAt(i);
                }
            }
        }

    }
}