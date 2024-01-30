using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using UnityEngine.Rendering;

//브금 Bgm
//효과음 Effect : UI등 위치와 상관없이 들려야 하는)
//임시음 3DSound : 위치에 따라서 사운드 차이가 나야하는)

public struct AudioPair
{
	public AudioPair(Transform _box, Queue<AudioSource> _queue)
	{
		box = _box;
		aus = _queue;
	}

	public Transform box;
	public Queue<AudioSource> aus;
}

public struct AudioPair
{
	public AudioPair(Transform _box)
	{
		box = _box;
		aus = _box.GetComponent<AudioSource>();
	}

	public Transform box;
	public AudioSource aus;
}

public class SoundManager : Singleton<SoundManager>
{
	[SerializedDictionary("Name", "Audio Source")]
	public SerializedDictionary<string, AudioClip> audioClips;

	public AudioSource dimensionSoundSetting;

	//[SerializeField] Transform tempAusBox;

	public float BgmOffset = 1.0f;
	public float EffectOffset = 1.0f;

	AudioPair bgmAus;
	AudioPair effectAusQueue; //None3D sound
	AudioPair dimensionAusQueue; //3D sound


	//float volumeValue = 1f;

	//   //브금 -> 여기서
	//   //엠비언트 -> 걍 맵 히어라키에서 세팅
	//   //일시적 물체음 -> Pooling해두고 세팅
	//   //해당 오브젝트에서 날 소리 -> transform으로 ㄴㄴ 오디오 소스 찾아서 넣고 없으면 일시적 물체음으로 ㄱㄱ


	#region About BGM
	public void PlayBGM(string clipName, float volume)
	{
		AudioSource aus = bgmAus.aus.Peek();
		aus.clip = GetAuidoClip(clipName);
		aus.volume = BgmOffset * volume;
		aus.Play();
	}

	public void StopBGM()
	{
		AudioSource aus = bgmAus.aus.Peek();
		aus.Stop();
	}

	public void PauseBGM()
	{
		AudioSource aus = bgmAus.aus.Peek();
		aus.Pause();
	}

	public void ResumeBGM()
	{
		AudioSource aus = bgmAus.aus.Peek();
		aus.Play();
	}
	#endregion

	public void PlaySound(string clipName)
	{
		PlayEffectSound(clipName, 1f, 1f);
	}

	public void PlaySound(string clipName, float volume, float pitch)
	{
		PlayEffectSound(clipName, volume, pitch);
	}

	public void PlaySound(string clipName,float volume, float minPitch, float maxPitch)
	{
		float pitch = Random.Range(minPitch, maxPitch);
		PlayEffectSound(clipName, 1f, pitch);
	}

	private void PlayEffectSound(AudioClip clip, float volume, float pitch)
		AudioSource aus = effectAusQueue.aus.Dequeue();
		aus.gameObject.SetActive(true);

		aus.clip = clip;
		aus.volume = volume;
		aus.pitch = pitch;

		aus.Play();
	}





	public void PlaySound(string clipName, GameObject obj)
	{
		AudioSource aus = obj.GetComponent<AudioSource>();

		if (aus)
		{

		}
		else
		{ 
			
		
		}

	}

	public void PlaySound(string clipName, GameObject obj, float volume, float pitch)
	{

	}

	public void PlaySound(string clipName, GameObject obj, float volume, float minPitch, float maxPitch)
	{

	}

	private void Play3DSound(string clipName, AudioSource aus, float volume, float pitch)
	{ 
			
	}

	private void Play3DTempSound(string clipName, Vector3 pos, float volume, float pitch)
	{

	}


	public void StopAllSound()
	{ 
	
	}


	//   public void PauseAllSound()
	//   {
	//       AudioSource[] allAS = FindObjectsOfType<AudioSource>();

	//       foreach (AudioSource audio in allAS)
	//       {
	//           audio.Pause();
	//       }
	//   }

	//   public void ResumeAllSound()
	//   {
	//       AudioSource[] allAS = FindObjectsOfType<AudioSource>();

	//       foreach (AudioSource audio in allAS)
	//       {
	//           audio.Play();
	//       }
	//   }


	//   public void PlayBgm(string clipName, float volume = 1f)
	//   { //이건 한무반복
	//       if (!bgmAus)
	//       {
	//           CreateBgmAudioSource();
	//       }
	//       bgmAus.clip = GetAuidoClip(clipName);
	//       bgmAus.volume = BgmOffset * volume;
	//       bgmAus.Play();
	//   }

	//   public void PlayBgmFade(string clipName)
	//   {
	//       if (!bgmAus)
	//       {
	//           CreateBgmAudioSource();
	//       }
	//       bgmAus.clip = GetAuidoClip(clipName);
	//       StartCoroutine(FadeIn(bgmAus, 5f));
	//   }

	//   public void StopBgm()
	//   {
	//       if (bgmAus)
	//       {
	//           bgmAus.Stop();
	//       }

	//   }

	//   public void StopAllSE()
	//   {
	//       AudioSource[] allAS = FindObjectsOfType<AudioSource>();

	//       foreach (AudioSource audio in allAS)
	//       {
	//           if (audio == bgmAus)
	//           {
	//               continue;
	//           }
	//           audio.Stop();
	//       }
	//   }
	//   public void StopAllSound()
	//   {
	//       AudioSource[] allAS = FindObjectsOfType<AudioSource>();

	//       foreach (AudioSource audio in allAS)
	//       {
	//           audio.Stop();
	//       }
	//   }

	//   public void PauseAllSound()
	//   {
	//       AudioSource[] allAS = FindObjectsOfType<AudioSource>();

	//       foreach (AudioSource audio in allAS)
	//       {
	//           audio.Pause();
	//       }
	//   }

	//   public void ResumeAllSound()
	//   {
	//       AudioSource[] allAS = FindObjectsOfType<AudioSource>();

	//       foreach (AudioSource audio in allAS)
	//       {
	//           audio.Play();
	//       }
	//   }


	//public void PlayTempSound(string clipName, Vector3 pos, float volume = 1f, float minPitch = 1f, float maxPitch = 1f)
	//   {
	//       //특정 위치에 잠시 틀어지거나 안움직일때 씀.
	//       AudioClip clip = GetAuidoClip(clipName);

	//	if (clip != null)
	//	{
	//		AudioSource temp = tempAusQueue.Dequeue();

	//		if (!temp.isPlaying)
	//		{
	//               temp.gameObject.SetActive(true);
	//               temp.pitch = Random.Range(minPitch, maxPitch);
	//               temp.volume = volume * SeOffset;
	//			temp.transform.position = pos;
	//			temp.PlayOneShot(clip);
	//			tempAusQueue.Enqueue(temp);
	//		}
	//		else
	//		{
	//               tempAusQueue.Enqueue(temp);

	//               GameObject newObj = new GameObject();
	//               newObj.name = "tempAus_" + tempAusQueue.Count.ToString();
	//               newObj.transform.parent = tempAusBox.transform;
	//               AudioSource newAus = newObj.AddComponent<AudioSource>();
	//               newAus.spatialBlend = 1f;

	//               newObj.AddComponent<TempAudioSource>();

	//               newAus.volume = volume * SeOffset;
	//               newAus.transform.position = pos;
	//               newAus.pitch = Random.Range(minPitch, maxPitch);
	//               newAus.PlayOneShot(clip);
	//               tempAusQueue.Enqueue(newAus);
	//           }
	//	}
	//   }

	//   public void PlaySound(string clipName, GameObject obj, float volume= 1f, float minPitch = 1f, float maxPitch = 1f)
	//   {
	//       //움직이는 오브젝트들에 사용. (소리가 따라 움직여야 할때)
	//       //그 오브젝트 안에 AuidoSource 컴포넌트 찾아보고 없으면 PlayTempSound로 돌릴꺼임 
	//       AudioClip clip = GetAuidoClip(clipName);

	//       if (clip)
	//       {
	//           AudioSource aus = obj.transform.root.GetComponent<AudioSource>();

	//           if (aus)
	//           {
	//               aus.volume = volume * SeOffset;
	//               aus.pitch = Random.Range(minPitch, maxPitch);
	//               aus.PlayOneShot(clip);
	//           }
	//           else
	//           { PlayTempSound(clipName, obj.transform.position, volume); }
	//       }

	//   }






	public void Awake()
	{
        Initailize(false);


		SetupAudioSources();

		//CreateTempAudioSource(50);
		//CreateBgmAudioSource();
	}

	void Start()
    {
        
    }

    void Update()
    {
        
    }


    private void SetupAudioSources()
    {
		//bgmAus = CreateAudPair("Bgm", 1, false);
		CreateBgmAS();
		effectAusQueue = CreateAudPair("Effect", 25, false);
		dimensionAusQueue = CreateAudPair("3DSound", 150, true);
	}


	private void CreateBgmAS()
	{
		string bgmAusName = "BgmAus";

		Transform box = gameObject.transform.Find(bgmAusName);

		if (box == null)
		{
			GameObject newObj = new GameObject(bgmAusName);
			newObj.transform.SetParent(transform);
			box = newObj.transform;
		}

		AudioSource aus = box.gameObject.AddComponent<AudioSource>();
		aus.spatialBlend = 0f;//3d sound 안하기 위해서
		aus.loop = true;

		var queue = new Queue<AudioSource>();
		queue.Enqueue(aus);

		bgmAus = new AudioPair(box, queue);
	}

	private AudioPair CreateAudPair(string boxName, int count, bool dimensionSound)
	{
		Transform box = gameObject.transform.Find(boxName);

		if (box == null)
		{
			GameObject newObj = new GameObject(boxName);
			newObj.transform.SetParent(transform);
			box = newObj.transform;
		}


		Queue<AudioSource> queue = new Queue<AudioSource>();

		for (int i = 0; i < count; ++i)
		{
			GameObject newObj = new GameObject(boxName + $"_{i}");
			newObj.transform.parent = box;
			newObj.SetActive(false);

			AudioSource aus;

			if (!dimensionSound)
			{
				aus = newObj.AddComponent<AudioSource>();
				aus.spatialBlend = 0f;//3d sound 안하기 위해서
				aus.loop = false;
			}
			else
			{
				aus = Funcs.CopyComponent<AudioSource>(dimensionSoundSetting, newObj);
			}

			queue.Enqueue(aus);
		}

		return new AudioPair(box, queue);

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

	//private IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
	//{
	//	float startVolume = audioSource.volume;
	//	while (audioSource.volume > 0)
	//	{
	//		audioSource.volume -= startVolume * Time.deltaTime / FadeTime;
	//		yield return null;
	//	}
	//	audioSource.Stop();
	//}

	//private IEnumerator FadeIn(AudioSource audioSource, float FadeTime)
	//{
	//	audioSource.Play();
	//	volumeValue = 0f;
	//	while (volumeValue < 1)
	//	{
	//		volumeValue += Time.deltaTime / FadeTime;
	//		bgmAus.volume = BgmOffset * volumeValue;
	//		yield return null;
	//	}
	//}


}
