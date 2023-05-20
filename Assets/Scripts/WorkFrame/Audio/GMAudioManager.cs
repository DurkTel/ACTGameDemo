using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GMAudioManager : SingletonMonoAuto<GMAudioManager>
{
    private AudioMixer m_audioMixer;

    public Dictionary<string, AudioMixerGroup> audioMixerGroups = new Dictionary<string, AudioMixerGroup>(); 

    public Dictionary<string, AudioGroup> audioGroups = new Dictionary<string, AudioGroup>();

    public ObjectPool<AudioObject> audioObject;

    public static void Initialize()
    {
        AudioTrackRegister.AudioParam param;
        foreach (var group in Instance.audioMixerGroups)
        {
            if (AudioTrackRegister.audioTrack.TryGetValue(group.Key, out param))
            {
                GameObject temp = new GameObject(string.Format("[{0}]", group.Value.name));
                temp.transform.SetParent(Instance.transform);
                AudioGroup audioGroup = temp.AddComponent<AudioGroup>();
                audioGroup.isLoop = param.isLoop;
                audioGroup.playMode = param.playMode;
                audioGroup.audioMixerGroup = group.Value;
                Instance.audioGroups.Add(group.Value.name, audioGroup);
            }
        }
    }

    public IEnumerator Init()
    {
        AssetLoader loader = AssetUtility.LoadAssetAsync<AudioMixer>("GameAudioMixer.mixer");

        yield return loader;

        m_audioMixer = loader.rawObject as AudioMixer;

        audioObject = new ObjectPool<AudioObject>((ao) => ao.Init(new GameObject()), (ao) => ao.Release());

        AudioMixerGroup[] audioMixerGroup = m_audioMixer.FindMatchingGroups("Master");
        foreach (var item in audioMixerGroup)
        {
            audioMixerGroups.Add(item.name, item);
        }
    }

    public static bool Play(string audioGroupName, string assetName)
    {
        AudioGroup audioGroup;
        if (Instance.audioGroups.TryGetValue(audioGroupName, out audioGroup))
        {
            audioGroup.Play(assetName);
            return true;
        }

        Debug.LogError("没有AudioGroupName = " + audioGroupName + "assetName = " + assetName + "的音效");
        return false;
    }

    public static bool Play(string audioGroupName, AudioClip audioClip)
    {
        AudioGroup audioGroup;
        if (Instance.audioGroups.TryGetValue(audioGroupName, out audioGroup))
        {
            audioGroup.Play(audioClip);
            return true;
        }

        return false;
    }

    public static bool IsPlaying(string AudioGroupName, string assetName)
    {
        AudioGroup audioGroup;
        if (Instance.audioGroups.TryGetValue(AudioGroupName, out audioGroup))
            return audioGroup.IsPlaying(assetName);

        return false;
    }

    public static bool IsPlaying(string AudioGroupName, AudioClip audioClip)
    {
        AudioGroup audioGroup;
        if (Instance.audioGroups.TryGetValue(AudioGroupName, out audioGroup))
            return audioGroup.IsPlaying(audioClip);

        return false;
    }

    public static bool Delete(string AudioGroupName, AudioClip audioClip)
    {
        AudioGroup audioGroup;
        if (Instance.audioGroups.TryGetValue(AudioGroupName, out audioGroup))
            return audioGroup.Delete(audioClip);

        return false;
    }

    public static bool Delete(string AudioGroupName, string assetName)
    {
        AudioGroup audioGroup;
        if (Instance.audioGroups.TryGetValue(AudioGroupName, out audioGroup))
            return audioGroup.Delete(assetName);

        return false;
    }

    public static void SetTotalAudio(float volume)
    {
        if (Instance.audioMixerGroups.TryGetValue("Master", out AudioMixerGroup group))
        {
            group.audioMixer.SetFloat("Total", volume);
        }
    }

    public static void SetBgAudio(float volume)
    {
        if (Instance.audioMixerGroups.TryGetValue("BGAudio", out AudioMixerGroup group))
        {
            group.audioMixer.SetFloat("BG", volume);
        }
    }

    public static void SetEffectAudio(float volume)
    {
        if (Instance.audioMixerGroups.TryGetValue("EffectAudio", out AudioMixerGroup group))
        {
            group.audioMixer.SetFloat("Effect", volume);
        }
    }

    public static void SetUIAudio(float volume)
    {
        if (Instance.audioMixerGroups.TryGetValue("UiAudio", out AudioMixerGroup group))
        {
            group.audioMixer.SetFloat("UI", volume);
        }
    }
}
