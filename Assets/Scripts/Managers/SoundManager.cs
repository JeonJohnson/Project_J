using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using Unity.VisualScripting;
using MoreMountains.Feedbacks;
using System.Xml.Linq;
using UnityEngine.UIElements;
using UnityEngine.Audio;


//브금 Bgm
//효과음 Effect : UI등 위치와 상관없이 들려야 하는)
//임시음 3DSound : 위치에 따라서 사운드 차이가 나야하는)

//Play / PlayOneShot 차이
// Play는 해당 AUS에 붙어있는거라서 Stop 등등이 먹히지만
//PlayOneShot은 Stop등등도 안먹히고 
public struct AudioQueuePair
{
	public AudioQueuePair(Transform _box, Queue<AudioSource> _queue)
	{
		box = _box;
		aus = _queue;
	}

	public Transform box;
	public Queue<AudioSource> aus;
}

public struct AudioPair
{
	public AudioPair(Transform _box, bool is3d, bool loop)
	{
		box = _box;
		aus = _box.GetComponent<AudioSource>();
		if (aus == null)
		{
			aus = _box.AddComponent<AudioSource>();
		}
		aus.spatialize = is3d;
		aus.loop = loop;
	}

	public AudioPair(Transform _box, AudioSource _aus)
	{
		box = _box;
		aus = _aus;
	}

	public Transform box;
	public AudioSource aus;
}

public class SoundManager : Singleton<SoundManager>
{

	//추후 게임 옵션에 있는 값 쓸 예졍
	public float BgmOffset = 1.0f;
	public float EffectOffset = 1.0f;
	//추후 게임 옵션에 있는 값 쓸 예졍

	[Space(10f)]
	[SerializedDictionary("Name", "Audio Source")]
	public SerializedDictionary<string, AudioClip> audioClips;

	public AudioSource dimensionSoundSetting;

	//[SerializeField] Transform tempAusBox;

	
	AudioPair bgmAus;
	AudioPair effectAus; //None3D sound
	AudioQueuePair tempAusQueue; //3D sound
								 //웬만하면 해당 오브젝트에 AudioSource 달아주기
								 //없으면 경고 띄워주삼 ㅋㅋ
								 //앵간하면 이제 우리가 세팅을 제대로 하기

	//float volumeValue = 1f;

	//   //브금 -> 여기서
	//   //엠비언트 -> 걍 맵 히어라키에서 세팅
	//   //일시적 물체음 -> Pooling해두고 세팅
	//   //해당 오브젝트에서 날 소리 -> transform으로 ㄴㄴ 오디오 소스 찾아서 넣고 없으면 일시적 물체음으로 ㄱㄱ


	#region About BGM
	public void PlayBGM(string clipName, float volume)
	{
		AudioSource aus = bgmAus.aus;
		aus.clip = GetAuidoClip(clipName);
		aus.volume = BgmOffset * volume;
		aus.Play();
	}
	public void StopBGM()
	{
		AudioSource aus = bgmAus.aus;
		aus.Stop();
	}
	public void PauseBGM()
	{
		AudioSource aus = bgmAus.aus;
		aus.Pause();
	}
	public void ResumeBGM()
	{
		AudioSource aus = bgmAus.aus;
		aus.Play();
	}
	#endregion


	#region About Effect Sound
	public void PlaySound(string clipName)
	{
		AudioClip clip = GetAuidoClip(clipName);
		PlayEffectSound(clip, 1f, 1f);
	}

	public void PlaySound(string clipName, float volume, float pitch)
	{
		AudioClip clip = GetAuidoClip(clipName);
		PlayEffectSound(clip, volume, pitch);
	}

	public void PlaySound(string clipName, float volume, float minPitch, float maxPitch)
	{
		float pitch = Random.Range(minPitch, maxPitch);
		AudioClip clip = GetAuidoClip(clipName);
		PlayEffectSound(clip, 1f, pitch);
	}
	private void PlayEffectSound(AudioClip clip, float volume, float pitch)
	{
		if (clip == null)
		{
			return;
		}

		AudioSource aus = effectAus.aus;
		//aus.gameObject.SetActive(true);

		aus.clip = clip;
		aus.volume = volume;
		aus.pitch = pitch;

		aus.PlayOneShot(clip);
	}
	#endregion

	#region PlaySound_Gameobject
	public void PlaySound(string clipName, GameObject obj, bool oneShot)
	{
		AudioSource aus = obj.GetComponent<AudioSource>();

		if (!aus)
		{
			aus = Funcs.CopyComponent(dimensionSoundSetting, obj);
		}

		AudioClip clip = GetAuidoClip(clipName);

		Play3DSound(clip, aus, 1f, 1f, oneShot);
	}

	public void PlaySound(string clipName, GameObject obj, float volume, float pitch, bool oneShot)
	{
		AudioSource aus = obj.GetComponent<AudioSource>();

		if (!aus)
		{
			aus = Funcs.CopyComponent(dimensionSoundSetting, obj);
		}

		AudioClip clip = GetAuidoClip(clipName);

		Play3DSound(clip, aus, volume, pitch, oneShot);
	}

	public void PlaySound(string clipName, GameObject obj, float volume, float minPitch, float maxPitch, bool oneShot)
	{
		AudioSource aus = obj.GetComponent<AudioSource>();

		if (!aus)
		{
			aus = Funcs.CopyComponent(dimensionSoundSetting, obj);
		}

		AudioClip clip = GetAuidoClip(clipName);
		float pitch = Random.Range(minPitch, maxPitch);
		Play3DSound(clip, aus, volume, pitch, oneShot);
	}
	#endregion

	#region PlaySound_AudioSource
	public void PlaySound(string clipName, AudioSource aus, bool oneShot)
	{

		AudioClip clip = GetAuidoClip(clipName);

		Play3DSound(clip, aus, 1f, 1f, oneShot);
	}

	public void PlaySound(string clipName, AudioSource aus, float volume, float pitch, bool oneShot)
	{
		AudioClip clip = GetAuidoClip(clipName);

		Play3DSound(clip, aus, volume, pitch, oneShot);
	}

	public void PlaySound(string clipName, AudioSource aus, float volume, float minPitch, float maxPitch, bool oneShot)
	{
		AudioClip clip = GetAuidoClip(clipName);
		float pitch = Random.Range(minPitch, maxPitch);
		Play3DSound(clip, aus, volume, pitch, oneShot);
	}
	#endregion

	#region Play_Position
	public void PlaySound(string clipName, Vector3 pos)
	{
		AudioClip clip = GetAuidoClip(clipName);
		PlayTempSound(clip, pos, 1f, 1f);
	}

	public void PlaySound(string clipName, Vector3 pos, float volume, float pitch)
	{
		AudioClip clip = GetAuidoClip(clipName);
		PlayTempSound(clip, pos, volume, pitch);
	}

	public void PlaySound(string clipName, Vector3 pos, float volume, float minPitch, float maxPitch)
	{
		AudioClip clip = GetAuidoClip(clipName);
		float pitch = Random.Range(minPitch, maxPitch);
		PlayTempSound(clip, pos, volume, pitch);
	}
	#endregion

	private void Play3DSound(AudioClip clip, AudioSource aus, float volume, float pitch, bool oneShot)
	{
		if (clip == null | aus == null)
		{ return; }

		aus.volume = volume * EffectOffset;
		aus.pitch = pitch;

		if (oneShot)
		{
			aus.PlayOneShot(clip);
		}
		else
		{
			aus.clip = clip;
			aus.Play();
		}
	}

	private void PlayTempSound(AudioClip clip, Vector3 pos, float volume, float pitch)
	{
		AudioSource aus = tempAusQueue.aus.Dequeue();

		aus.clip = clip;
		aus.volume = volume * EffectOffset;
		aus.pitch = pitch;
		aus.gameObject.transform.position = pos;
		
		//AudioSource.PlayClipAtPoint(clip, transform.position, volume, dimensionSoundSetting.spatialBlend, dimensionSoundSetting.priority, mixerGroup);
		//를 쓰니 옵션값을 못바꿈. 볼륨까지 밖에,,,

		aus.Play();

		tempAusQueue.aus.Enqueue(aus);
	}
	public void StopAllSound()
	{ 
		
	}

	public void Awake()
	{
        Initailize(false);


		SetupAudioSources();
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
		bgmAus = CreateAudPair("BGM",true);
		effectAus = CreateAudPair("Effect");
		tempAusQueue = CreateAudQueuePair("3DSound", 150, true);
	}


	private AudioPair CreateAudPair(string boxName, bool loop = false, bool spatialize = false)
	{
		Transform box = gameObject.transform.Find(boxName);

		if (box == null)
		{
			GameObject newObj = new GameObject(boxName);
			newObj.transform.SetParent(transform);
			box = newObj.transform;
		}

		return new AudioPair(box, spatialize,loop);
	}

	private AudioQueuePair CreateAudQueuePair(string boxName, int count, bool dimensionSound)
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
			//newObj.SetActive(false);

			AudioSource aus;

			if (!dimensionSound)
			{
				aus = newObj.AddComponent<AudioSource>();
				aus.spatialize = dimensionSound; //3d sound 안하기 위해서
				aus.loop = false;
			}
			else
			{
				aus = Funcs.CopyComponent<AudioSource>(dimensionSoundSetting, newObj);
			}

			queue.Enqueue(aus);
		}

		return new AudioQueuePair(box, queue);

	}

	private void AddAus(ref AudioQueuePair pair, int count, bool dimensionSound)
	{
		string boxName = pair.box.gameObject.name;
		var originQueue = pair.aus;
		var newQueue = new Queue<AudioSource>();

		for (int i = 0; i < count; ++i)
		{
			GameObject newObj = new GameObject(boxName + $"_{originQueue.Count + i}");
			newObj.transform.parent = pair.box;

			AudioSource aus;

			if (!dimensionSound)
			{
				aus = newObj.AddComponent<AudioSource>();
				aus.spatialize = dimensionSound; //3d sound 안하기 위해서
				aus.loop = false;
			}
			else
			{
				aus = Funcs.CopyComponent<AudioSource>(dimensionSoundSetting, newObj);
			}

			newQueue.Enqueue(aus);
		}

		int originQueueCount = originQueue.Count;
		for (int k = 0; k < originQueueCount; ++k)
		{
			newQueue.Enqueue(originQueue.Dequeue());
		}

		pair.aus = newQueue;
	}


	AudioClip GetAuidoClip(string clipName)
	{
		AudioClip audioClip = null;

		if (!audioClips.TryGetValue(clipName, out audioClip))
		{
			Debug.LogError($"'{clipName}' AuidoClip has not exist");
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
