using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using Unity.VisualScripting;
using MoreMountains.Feedbacks;
using System.Xml.Linq;
using UnityEngine.UIElements;
using UnityEngine.Audio;

public struct AudioOption
{ 
	public AudioSource aus;
	public string clipName;
	public float volume;
	public float pitch;
	public bool oneShot;
}


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

/// <Notice>
///3D sound
/// 소리가 나야하는 오브젝트에 AudioSource 달아서 쓰셈!!!!!!
/// 없으면 경고 띄워주삼 ㅋㅋ
/// 이제 앵간하면 우리가 세팅을 제대로 하기
/// <Notice>

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

	/// <Notice>
	///3D sound
	/// 소리가 나야하는 오브젝트에 AudioSource 달아서 쓰셈!!!!!!
	/// UI든 뭐든!!!!
	/// 없으면 경고 띄워주고 이제 안만들어 넣어줄꺼임
	/// 이제 앵간하면 우리가 세팅을 제대로 하기
	/// </Notice>
	
	//PlayBGM() : 브금 재생
	//엠비언트 : 해당 오브젝트 AudioSource에서 (코드 사용x)

	//PlaySound() : 앵간한 경우, 대신 이제 소리낼 오브젝트가 AudioSource가지고 있어야함.		
				//없을경우 이제는 재생안시켜주고 오류띄울꺼임 흥
				//Pitch와 Volume이 고정이면 OneShot해도 되지만,
				//가변적이라면 OneShot으로 이전에 재생시킨 사운드도 영향받으니 잘 고려해보셈

	//PlayTemp2DSound() PlayTemp3DSound() : 이건 진짜
										//구조상 사운드를 재생하는 오브젝트를 찾기 힘들거나
										//사운드를 재생하는 오브젝트가 사운드가 끝나기전 사라진다거나
										//가변적인 Pitch, Volume									
										//할때
										//고정된 위치
										//조건 한에서 사용하삼! 
										//OneShot 아님 걍 Play임
										

	AudioPair bgmAus;
	AudioQueuePair tempAusQueue; 
	List<AudioSource> playingTempAus;


	#region About BGM
	public void PlayBGM(string clipName, float volume = 1f)
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


	//#region About Effect Sound
	//public void PlaySound(string clipName)
	//{
	//	AudioClip clip = GetAuidoClip(clipName);
	//	PlayEffectSound(clip, 1f, 1f);
	//}

	//public void PlaySound(string clipName, float volume, float pitch)
	//{
	//	AudioClip clip = GetAuidoClip(clipName);
	//	PlayEffectSound(clip, volume, pitch);
	//}

	//public void PlaySound(string clipName, float volume, float minPitch, float maxPitch)
	//{
	//	float pitch = Random.Range(minPitch, maxPitch);
	//	AudioClip clip = GetAuidoClip(clipName);
	//	PlayEffectSound(clip, volume, pitch);
	//}

	//private void PlayEffectSound(AudioClip clip, float volume, float pitch)
	//{
	//	if (clip == null)
	//	{
	//		return;
	//	}

	//	AudioSource aus = effectAus.aus;
	//	//aus.gameObject.SetActive(true);

	//	aus.clip = clip;
	//	aus.volume = volume;
	//	aus.pitch = pitch;

	//	aus.PlayOneShot(clip);
	//}
	//#endregion

	public void PlaySound(string clipName, GameObject obj, bool oneShot)
	{
		AudioSource aus = obj.GetComponent<AudioSource>();

		if (!aus)
		{
			Debug.LogError(obj.name + "은 오디오 소스 없는디요?");
			return;
			//aus = Funcs.CopyComponent(dimensionSoundSetting, obj);
		}

		AudioClip clip = GetAuidoClip(clipName);

		PlaySound(clip, aus, 1f, 1f, oneShot);
	}

	public void PlaySound(string clipName, GameObject obj, float volume, float pitch, bool oneShot)
	{
		AudioSource aus = obj.GetComponent<AudioSource>();

		if (!aus)
		{
			Debug.LogError(obj.name + "은 오디오 소스 없는디요?");
			return;
			//aus = Funcs.CopyComponent(dimensionSoundSetting, obj);
		}

		AudioClip clip = GetAuidoClip(clipName);

		PlaySound(clip, aus, volume, pitch, oneShot);
	}

	public void PlaySound(string clipName, GameObject obj, float volume, float minPitch, float maxPitch, bool oneShot)
	{
		AudioSource aus = obj.GetComponent<AudioSource>();

		if (!aus)
		{
			Debug.LogError(obj.name + "은 오디오 소스 없는디요?");
			return;
			//aus = Funcs.CopyComponent(dimensionSoundSetting, obj);
		}

		AudioClip clip = GetAuidoClip(clipName);
		float pitch = Random.Range(minPitch, maxPitch);
		PlaySound(clip, aus, volume, pitch, oneShot);
	}

	public void PlaySound(string clipName, AudioSource aus, bool oneShot)
	{
		if (!aus)
		{
			Debug.Log("오디오 소스 없음 ㅅㄱ 디버그 찍어서 보셈");
			return;
		}

		AudioClip clip = GetAuidoClip(clipName);
		PlaySound(clip, aus, 1f, 1f, oneShot);
	}

	public void PlaySound(string clipName, AudioSource aus, float volume, float pitch, bool oneShot)
	{

		if (!aus)
		{
			Debug.Log("오디오 소스 없음 ㅅㄱ 디버그 찍어서 보셈");
			return;
		}

		AudioClip clip = GetAuidoClip(clipName);

		PlaySound(clip, aus, volume, pitch, oneShot);
	}

	public void PlaySound(string clipName, AudioSource aus, float volume, float minPitch, float maxPitch, bool oneShot)
	{
		if (!aus)
		{
			Debug.Log("오디오 소스 없음 ㅅㄱ 디버그 찍어서 보셈");
			return;
		}

		AudioClip clip = GetAuidoClip(clipName);
		float pitch = Random.Range(minPitch, maxPitch);
		PlaySound(clip, aus, volume, pitch, oneShot);
	}

	public void PlayTemp3DSound(string clipName, Vector3 pos, float volume, float pitch)
	{
		AudioClip clip = GetAuidoClip(clipName);

		PlayTempSound(clip, pos, volume, pitch, true);
	}

	public void PlayTemp2DSound(string clipName, Vector3 pos, float volume, float pitch)
	{
		AudioClip clip = GetAuidoClip(clipName);

		PlayTempSound(clip, pos, volume, pitch, false);
	}


	#region Play_Position
	//public void PlaySound(string clipName, Vector3 pos)
	//{
	//	AudioClip clip = GetAuidoClip(clipName);
	//	PlayTempSound(clip, pos, 1f, 1f);
	//}

	//public void PlaySound(string clipName, Vector3 pos, float volume, float pitch)
	//{
	//	AudioClip clip = GetAuidoClip(clipName);
	//	PlayTempSound(clip, pos, volume, pitch);
	//}

	//public void PlaySound(string clipName, Vector3 pos, float volume, float minPitch, float maxPitch)
	//{
	//	AudioClip clip = GetAuidoClip(clipName);
	//	float pitch = Random.Range(minPitch, maxPitch);
	//	PlayTempSound(clip, pos, volume, pitch);
	//}
	#endregion

	private void PlaySound(AudioClip clip, AudioSource aus, float volume, float pitch, bool oneShot)
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

	private void PlayTempSound(AudioClip clip, Vector3 pos, float volume, float pitch, bool spatialize)
	{
		if (tempAusQueue.aus.Count== 0)
		{
			AddAus(ref tempAusQueue, 2);
		}

		AudioSource aus = tempAusQueue.aus.Dequeue();


		aus.spatialize = spatialize;
		aus.clip = clip;
		aus.volume = volume * EffectOffset;
		aus.pitch = pitch;
		aus.gameObject.transform.position = pos;
		
		aus.Play();

		playingTempAus.Add(aus);

		//tempAusQueue.aus.Enqueue(aus);
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
		CheckTempAus();
	}

	private void FixedUpdate()
	{
		
	}

	private void CheckTempAus()
	{
		//var finAus = playingTempAus.Find(aus => aus.isPlaying == false);

		if (playingTempAus.Count == 0)
		{
			return;
		}

		for (int i = 0; i < playingTempAus.Count;)
		{
			AudioSource curAus = playingTempAus[i];

			if (!curAus.isPlaying)
			{
				tempAusQueue.aus.Enqueue(curAus);
				playingTempAus.Remove(curAus);
			}
			else
			{
				++i;
			}
		}
	}

	private void SetupAudioSources()
    {
		bgmAus = CreateAudPair("BGM",true);
		tempAusQueue = CreateAudQueuePair("Temp", 150);
		playingTempAus = new List<AudioSource>();
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

	private AudioQueuePair CreateAudQueuePair(string boxName, int count)
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

			//if (!dimensionSound)
			//{
			//	aus = newObj.AddComponent<AudioSource>();
			//	aus.spatialize = dimensionSound; //3d sound 안하기 위해서
				
			//}
			//else
			//{
				aus = Funcs.CopyComponent<AudioSource>(dimensionSoundSetting, newObj);
				aus.loop = false;
			//}

			queue.Enqueue(aus);
		}

		return new AudioQueuePair(box, queue);

	}

	private void AddAus(ref AudioQueuePair pair, int count)
	{
		string boxName = pair.box.gameObject.name;
		var originQueue = pair.aus;
		var newQueue = new Queue<AudioSource>();

		for (int i = 0; i < count; ++i)
		{
			GameObject newObj = new GameObject(boxName + $"_{originQueue.Count + i}");
			newObj.transform.parent = pair.box;

			AudioSource aus;

			//if (!dimensionSound)
			//{
			//	aus = newObj.AddComponent<AudioSource>();
			//	aus.spatialize = dimensionSound; //3d sound 안하기 위해서
			//	aus.loop = false;
			//}
			//else
			//{
			//	aus = Funcs.CopyComponent<AudioSource>(dimensionSoundSetting, newObj);
			//}

			aus = Funcs.CopyComponent<AudioSource>(dimensionSoundSetting, newObj);
			aus.loop = false;

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
