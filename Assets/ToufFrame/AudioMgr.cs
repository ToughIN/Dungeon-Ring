using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace ToufFrame
{
    public class AudioMgr : SingletonBase<AudioMgr>
    {
        private AudioSource bgmSource = null;
        
        
        private float bgmVolume = 1;
        private float soundVolume = 1;


        private GameObject soundObj = null;
        private List<AudioSource> sourceList = new List<AudioSource>();


        public AudioMgr()
        {
            MonoMgr.Instance.AddUpdateListener(Update);
        }

        private void Update()
        {
            for(int i =sourceList.Count-1;i>=0;i--)
            {
                if (!sourceList[i].isPlaying)
                {
                    GameObject.Destroy(sourceList[i]);
                    sourceList.RemoveAt(i);
                }
            }
        }

        public void PlayBGM(string name)
        {
            if (bgmSource == null)
            {
                GameObject obj = new GameObject("BGM");
                bgmSource = obj.AddComponent<AudioSource>();
            }
            
            ResourceMgr.Instance.LoadAsync<AudioClip>("Music/BGM/"+name,(clip) =>
            {
                bgmSource.clip = clip;
                bgmSource.loop = true;
                bgmSource.volume = bgmVolume;
                bgmSource.Play();
            });
            
        }

        public void ChangeBGMVolumn(float v)
        {
            bgmVolume= v;
            if(bgmSource==null)return;
            bgmSource.volume = v;
        }
        
        public void PauseBGM()
        {
            if(bgmSource==null)return;
            bgmSource.Pause();
        }
        

        public void StopBGM()
        {
            
        }

        public void PlaySound(string name,UnityAction<AudioSource> callBack=null, bool isLoop = false)
        {
            if (soundObj == null)
            {
                soundObj = new GameObject("Sound");
            }

            
            
            ResourceMgr.Instance.LoadAsync<AudioClip>("Sound/"+name,(clip) =>
            {
                AudioSource source =  soundObj.AddComponent<AudioSource>();
                sourceList.Add(source);
                source.clip = clip;
                source.loop=isLoop;
                source.volume = soundVolume;
                source.Play();
                if (callBack != null)
                {
                    callBack(source);
                }
                
            });
        }

        public void StopSound(AudioSource source)
        {
            if(sourceList.Contains(source))
            {
                source.Stop();
                sourceList.Remove(source);
                GameObject.Destroy(source);
            }
        }

        public void ChangeSoundValue(float value)
        {
            soundVolume = value;
            foreach (var source in sourceList)
            {
                source.volume = value;
            }
        }
        
        
        
    }
}