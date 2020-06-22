using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    AudioSource bgm;
    List<AudioSource> allAudio = new List<AudioSource>();
    Dictionary<string, AudioClip> allClipDic = new Dictionary<string, AudioClip>();
    GameObject audioParent;
    private void Awake()
    {
        Instance = this;
    }
    public void Init(bool musicOn)
    {
        audioParent = new GameObject("Audios");
        bgm = audioParent.AddComponent<AudioSource>();
        bgm.clip = Resources.Load<AudioClip>("AudioClips/DiceBgm");
        bgm.loop = true;
        bgm.Play();
        bgm.mute = !musicOn;
    }
    public void PlayerSound(string name)
    {
        AudioClip tempClip;
        if (allClipDic.ContainsKey(name))
        {
            tempClip = allClipDic[name];
        }
        else
        {
            tempClip = Resources.Load<AudioClip>("AudioClips/" + name);
            allClipDic.Add(name, tempClip);
        }
        bool hasPlay = false;
        for (int i = 0; i < allAudio.Count; i++)
        {
            AudioSource tempAS = allAudio[i];
            if (!tempAS.isPlaying)
            {
                tempAS.clip =tempClip;
                tempAS.loop = false;
                tempAS.volume = 1;
                tempAS.pitch = 1;
                tempAS.Play();
                tempAS.mute = !GameManager.Instance.GetSoundOn();
                hasPlay = true;
                break;
            }
        }
        if (!hasPlay)
        {
            AudioSource temp = audioParent.AddComponent<AudioSource>();
            temp.playOnAwake = false;
            temp.volume = 1;
            temp.pitch = 1;
            temp.loop = false;
            temp.mute = !GameManager.Instance.GetSoundOn();
            allAudio.Add(temp);
            temp.clip = tempClip;
            temp.Play();
        }
    }
    public AudioSource PlayerSoundLoop(string name)
    {
        AudioClip tempClip;
        AudioSource tempSource = null;
        if (allClipDic.ContainsKey(name))
        {
            tempClip = allClipDic[name];
        }
        else
        {
            tempClip = Resources.Load<AudioClip>("AudioClips/" + name);
            allClipDic.Add(name, tempClip);
        }
        bool hasPlay = false;
        for (int i = 0; i < allAudio.Count; i++)
        {
            if (!allAudio[i].isPlaying)
            {
                tempSource = allAudio[i];
                tempSource.clip = tempClip;
                tempSource.loop = true;
                tempSource.volume = 1;
                tempSource.pitch = 1;
                tempSource.mute = !GameManager.Instance.GetSoundOn();
                tempSource.Play();
                hasPlay = true;
                break;
            }
        }
        if (!hasPlay)
        {
            tempSource = audioParent.AddComponent<AudioSource>();
            tempSource.playOnAwake = false;
            tempSource.volume = 1;
            tempSource.pitch = 1;
            tempSource.loop = true;
            tempSource.mute = !GameManager.Instance.GetSoundOn();
            allAudio.Add(tempSource);
            tempSource.clip = tempClip;
            tempSource.Play();
        }
        return tempSource;
    }
    public void ChangeLoopAudio(AudioSource audioSource,string clipName)
    {
        AudioClip tempClip = null;
        if (allClipDic.TryGetValue(clipName,out tempClip)&&tempClip is object)
        {
            audioSource.Stop();
            audioSource.clip = tempClip;
            audioSource.Play();
        }
        else
        {
            tempClip = Resources.Load<AudioClip>("AudioClips/" + clipName);
            if(tempClip is object)
            {
                allClipDic.Add(clipName, tempClip);
                audioSource.Stop();
                audioSource.clip = tempClip;
                audioSource.Play();
            }
            else
            {
                Debug.LogError("更换循环音乐错误，不存在该音乐片段：" + clipName);
            }
        }
    }
    public void SetMusic(bool musicOn)
    {
        bgm.mute = !musicOn;
    }
    public void SetSound(bool soundOn)
    {
        foreach(AudioSource a in allAudio)
        {
            a.mute = !soundOn;
        }
    }
    public void PauseBgm(bool value)
    {
        if (value)
            bgm.Pause();
        else
            bgm.Play();
    }
}
