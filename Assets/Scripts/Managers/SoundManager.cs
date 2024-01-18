using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class SoundManager : Singleton<SoundManager>
{
    [SerializedDictionary("Name", "Audio Source")]
    public SerializedDictionary<string, AudioClip> audioClips;

    [SerializeField] Transform tempAusBox;
    
    public float BgmOffset = 1.0f;
    public float SeOffset = 1.0f;

    public AudioSource bgmAus;
    public Queue<AudioSource> tempAusQueue;

    float volumeValue = 1f;


    //브금 -> 여기서
    //엠비언트 -> 걍 맵 히어라키에서 세팅
    //일시적 물체음 -> Pooling해두고 세팅
    //해당 오브젝트에서 날 소리 -> transform으로 ㄴㄴ 오디오 소스 찾아서 넣고 없으면 일시적 물체음으로 ㄱㄱ
    public IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;
            yield return null;
        }
        audioSource.Stop();
    }

    public IEnumerator FadeIn(AudioSource audioSource, float FadeTime)
    {
        audioSource.Play();
        volumeValue = 0f;
        while (volumeValue < 1)
        {
            volumeValue += Time.deltaTime / FadeTime;
            bgmAus.volume = BgmOffset * volumeValue;
            yield return null;
        }
    }

    public void PlayBgm(string clipName, float volume = 1f)
    { //이건 한무반복
        if (!bgmAus)
        {
            CreateBgmAudioSource();
        }
        bgmAus.clip = GetAuidoClip(clipName);
        bgmAus.volume = BgmOffset * volume;
        bgmAus.Play();
    }

    public void PlayBgmFade(string clipName)
    {
        if (!bgmAus)
        {
            CreateBgmAudioSource();
        }
        bgmAus.clip = GetAuidoClip(clipName);
        StartCoroutine(FadeIn(bgmAus, 5f));
    }

    public void StopBgm()
    {
        if (bgmAus)
        {
            bgmAus.Stop();
        }

    }

    public void StopAllSE()
    {
        AudioSource[] allAS = FindObjectsOfType<AudioSource>();

        foreach (AudioSource audio in allAS)
        {
            if (audio == bgmAus)
            {
                continue;
            }
            audio.Stop();
        }
    }
    public void StopAllSound()
    {
        AudioSource[] allAS = FindObjectsOfType<AudioSource>();

        foreach (AudioSource audio in allAS)
        {
            audio.Stop();
        }
    }

    public void PauseAllSound()
    {
        AudioSource[] allAS = FindObjectsOfType<AudioSource>();

        foreach (AudioSource audio in allAS)
        {
            audio.Pause();
        }
    }

    public void ResumeAllSound()
    {
        AudioSource[] allAS = FindObjectsOfType<AudioSource>();

        foreach (AudioSource audio in allAS)
        {
            audio.Play();
        }
    }


	public void PlayTempSound(string clipName, Vector3 pos, float volume = 1f, float minPitch = 1f, float maxPitch = 1f)
    {
        //특정 위치에 잠시 틀어지거나 안움직일때 씀.
        AudioClip clip = GetAuidoClip(clipName);

		if (clip != null)
		{
			AudioSource temp = tempAusQueue.Dequeue();

			if (!temp.isPlaying)
			{
                temp.gameObject.SetActive(true);
                temp.pitch = Random.Range(minPitch, maxPitch);
                temp.volume = volume * SeOffset;
				temp.transform.position = pos;
				temp.PlayOneShot(clip);
				tempAusQueue.Enqueue(temp);
			}
			else
			{
                tempAusQueue.Enqueue(temp);

                GameObject newObj = new GameObject();
                newObj.name = "tempAus_" + tempAusQueue.Count.ToString();
                newObj.transform.parent = tempAusBox.transform;
                AudioSource newAus = newObj.AddComponent<AudioSource>();
                newAus.spatialBlend = 1f;
                
                newObj.AddComponent<TempAudioSource>();

                newAus.volume = volume * SeOffset;
                newAus.transform.position = pos;
                newAus.pitch = Random.Range(minPitch, maxPitch);
                newAus.PlayOneShot(clip);
                tempAusQueue.Enqueue(newAus);
            }
		}
    }

    public void PlaySound(string clipName, GameObject obj, float volume= 1f, float minPitch = 1f, float maxPitch = 1f)
    {
        //움직이는 오브젝트들에 사용. (소리가 따라 움직여야 할때)
        //그 오브젝트 안에 AuidoSource 컴포넌트 찾아보고 없으면 PlayTempSound로 돌릴꺼임 
        AudioClip clip = GetAuidoClip(clipName);

        if (clip)
        {
            AudioSource aus = obj.transform.root.GetComponent<AudioSource>();

            if (aus)
            {
                aus.volume = volume * SeOffset;
                aus.pitch = Random.Range(minPitch, maxPitch);
                aus.PlayOneShot(clip);
            }
            else
            { PlayTempSound(clipName, obj.transform.position, volume); }
        }

    }




    //void SearchAllAudClips()
    //{
    //    AudioClip[] clips = Resources.LoadAll<AudioClip>(audioFolderPath);

    //    audioClips = new Dictionary<string, AudioClip>();
    //    foreach (AudioClip clip in clips)
    //    {
    //        string clipName = clip.name;
    //        audioClips.Add(clipName, clip);
    //    }
    //}

    void CreateTempAudioSource(int count)
    {
        if (tempAusBox != null)
        {
            string boxName = "TempAusBox";
            Transform tr = transform.Find(boxName);
            if (tr == null)
            {
                GameObject newObj = new GameObject(boxName);
                newObj.transform.SetParent(gameObject.transform);
                tr = newObj.transform;
            }
            tempAusBox = tr;
        }

        if (tempAusQueue == null)
        {
            tempAusQueue = new Queue<AudioSource>();
        }

        for (int i = 0; i < count; ++i)
        {
            GameObject newObj = new GameObject();
            newObj.name = $"tempAus_{i}";
            newObj.transform.parent = tempAusBox.transform;

            AudioSource aus = newObj.AddComponent<AudioSource>();
            aus.spatialBlend = 1f;

            newObj.AddComponent<TempAudioSource>();

            newObj.SetActive(false);
            tempAusQueue.Enqueue(aus);
        }
    }

    void CreateBgmAudioSource()
    {
        if (bgmAus == null)
        {
            string bgmAusName = "BgmAus";
            Transform tr = gameObject.transform.Find(bgmAusName);

            if (tr == null)
            {
                GameObject newObj = new GameObject(bgmAusName);
                newObj.transform.SetParent(transform);
                tr = newObj.transform;
            }

            AudioSource aus = tr.GetComponent<AudioSource>();

            if (aus == null)
            {
                aus = tr.gameObject.AddComponent<AudioSource>();

            }

            aus.spatialBlend = 0f;
            //3d sound 안하기 위해서
            aus.loop = true;
            bgmAus = aus;
        }
    }

    AudioClip GetAuidoClip(string clipName)
    {
        AudioClip audioClip = null;
        if (!audioClips.TryGetValue(clipName, out audioClip))
        {
            Debug.LogError($"{clipName} is empty");
        }
        return audioClip;
    }

    public void Awake()
	{
        //SearchAllAudClips();
        CreateTempAudioSource(50);
        CreateBgmAudioSource();
    }

	void Start()
    {
        //SearchAllAudClips();
        //CreateTempAudioSource(50);
        //CreateBgmAudioSource();
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    PlayTempSound("SoundTest", Vector3.zero);
        //}
    }
}
