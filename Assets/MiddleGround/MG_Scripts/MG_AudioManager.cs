using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiddleGround.Audio
{
    public class MG_AudioManager : MonoBehaviour
    {
        public static MG_AudioManager Instance;
        static readonly Dictionary<int, string> dic_type_Path = new Dictionary<int, string>()
        {
            {(int)MG_PlayAudioType.BGM,"MG_AudioClips/MG_AC_BGM" },
            {(int)MG_PlayAudioType.Button,"MG_AudioClips/MG_AC_Button" },
            {(int)MG_PlayAudioType.SpinDice,"MG_AudioClips/MG_AC_SpinDice" },
            {(int)MG_PlayAudioType.SpinSlots,"MG_AudioClips/MG_AC_SpinSlots" },
            {(int)MG_PlayAudioType.Fly,"MG_AudioClips/MG_AC_Fly" },
        };
        readonly Dictionary<int, AudioClip> dic_type_ac = new Dictionary<int, AudioClip>();
        readonly List<AudioSource> as_all = new List<AudioSource>();
        AudioSource as_bgm;
        GameObject go_root;
        public void Init(GameObject asRoot)
        {
            Instance = this;
            go_root = asRoot;
            as_bgm = asRoot.AddComponent<AudioSource>();
            AudioClip ac_bgm = Resources.Load<AudioClip>(dic_type_Path[(int)MG_PlayAudioType.BGM]);
            dic_type_ac.Add((int)MG_PlayAudioType.BGM, ac_bgm);
            as_bgm.clip = ac_bgm;
            as_bgm.loop = true;
            as_bgm.playOnAwake = false;
            as_bgm.mute = !MG_Manager.Instance.Get_Save_MusicOn();
            as_bgm.Play();
        }
        public AudioSource PlayOneShot(MG_PlayAudioType audioType)
        {
            int _typeIndex = (int)audioType;
            if(dic_type_ac.TryGetValue(_typeIndex,out AudioClip tempAC))
            {
                if(tempAC is null)
                {
                    Debug.LogError("Play MG_ShotAudio Error : loadedDic has key , but content is null.");
                    return null;
                }
                bool hasPlayed = false;
                int asCount = as_all.Count;
                for(int i = 0; i < asCount; i++)
                {
                    if (!as_all[i].isPlaying)
                    {
                        AudioSource tempAS = as_all[i];
                        tempAS.clip = tempAC;
                        tempAS.loop = false;
                        tempAS.playOnAwake = false;
                        tempAS.mute = !MG_Manager.Instance.Get_Save_SoundOn();
                        tempAS.Play();
                        hasPlayed = true;
                        return tempAS;
                    }
                }
                if (!hasPlayed)
                {
                    AudioSource tempAS = go_root.AddComponent<AudioSource>();
                    tempAS.clip = tempAC;
                    tempAS.loop = false;
                    tempAS.playOnAwake = false;
                    tempAS.mute = !MG_Manager.Instance.Get_Save_SoundOn();
                    tempAS.Play();
                    as_all.Add(tempAS);
                    return tempAS;
                }
            }
            else
            {
                if(dic_type_Path.TryGetValue(_typeIndex,out string tempPath))
                {
                    if (string.IsNullOrEmpty(tempPath))
                    {
                        Debug.LogError("Play MG_ShotAudio Error : audioPathDic has key , but content is null.");
                        return null;
                    }
                    tempAC = Resources.Load<AudioClip>(tempPath);
                    dic_type_ac.Add(_typeIndex, tempAC);
                    return PlayOneShot(audioType);
                }
                else
                {
                    Debug.LogError("Play MG_ShotAudio Error : audioPathDic not has this audio. type : " + audioType);
                    return null;
                }
            }
            return null;
        }
        public AudioSource PlayLoop(MG_PlayAudioType audioType)
        {
            AudioSource result = PlayOneShot(audioType);
            result.loop = true;
            return result;
        }
        public void SetMusicState(bool state)
        {
            as_bgm.mute = !state;
        }
        public void SetSoundState(bool state)
        {
            foreach(AudioSource temp in as_all)
            {
                temp.mute = !state;
            }
        }
        public void PauseBgm(bool pause)
        {
            if (pause)
                as_bgm.Pause();
            else
                as_bgm.Play();
        }
    }
    public enum MG_PlayAudioType
    {
        BGM,
        Button,
        SpinDice,
        SpinSlots,
        Fly
    }
}
