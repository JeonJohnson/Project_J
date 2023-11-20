using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	//제네뤽 클래스
	//C++의 템플릿 클래스 비슷한거임
	private static T instance = null;
	//public bool isDontDestory;
	//스스로 Awake에서 Dont Destroy 설정해주셈

	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				instance = InstantiateManager();
			}
			return instance;
		}
	}

	public static T InstantiateManager(/*bool isDontDestroy*/)
	{
		//1. 하이어라키 창에 있는지 확인.
		//2. 없으면 ManagerPrefabs 폴더안에 같은이름 프리팹 있는지 확인.
		//3. 없으면 새로 만듬.

		//230530 문제점 발견
		//Manager를 만약 씬에 올려 놓은 경우, 초기화가 일어난건지 안일어난건지 불확실함.
		//GameManager의 경우, 씬에 올려놓았을 때, 스크립트 실행순서에 맞춰 가장 먼저 호출된다.
		//GameManager Awake에서 먼저 다른 매니저들 생성하고 다른 스크립트의 Awake를 다 호출한뒤
		//다시 RuntimeInitializeOnLoadMethod 어트리뷰트로 생성하는데
		//instance 자체는 생성하지 않음으로 한무루프에 빠짐.
		//
		//근데 Awake에서 instance를 생성하는거 자체가 InstantiateManager 함수를 호출 하는거라서
		//그것도 한무루프,,,


		if (instance == null)
		{
			string managerName = typeof(T).Name;
			GameObject managerObj = null;
			T tempInstance = FindObjectOfType<T>();

			if (tempInstance)
			{ //이미 하이어라키창에 오브젝트가 올라가져 있을 경우

				//걍 바로 박스 담아주삼
				managerObj = tempInstance.gameObject;
			}
			else
			{ //없는 경우 (새로 게임오브젝트 만들어야함)
			  //1. 프리팹 찾아보기
				GameObject prefab = Resources.Load(Defines.managerPrfabFolderPath + typeof(T).Name) as GameObject;

				if (prefab)
				{
					managerObj = Instantiate(prefab); //여기서 Awake 한번 더 호출함.
					managerObj.name = managerObj.name.Replace("(Clone)", string.Empty);
					tempInstance = managerObj.GetComponent<T>();
				}
				else
				{
					managerObj = new GameObject(managerName);
					tempInstance = managerObj.AddComponent<T>();//여기서 Awake 한번 더 호출함.
				}
			}

			instance = tempInstance;

			GameObject boxObj = FindManagerBoxes(false);
			managerObj.transform.SetParent(boxObj.transform);
		}

		return instance;
	}


	public static void InstantiateManagerByPrefabPath(string prefabFolderPath, bool isDontDestroy = true)
	{
		//프리팹 있는거 확신 할 때.
		//앵간하면 InstantiateManager로 퉁치삼

		Debug.LogWarning("U should be use \"InstantiateManager\" Funcs instead of \"InstantiateManagerByPrefabPath\".");

		if (instance == null)
		{
			GameObject managerObj = GameObject.Find(typeof(T).Name);

			if (managerObj == null)
			{
				GameObject prefab = Resources.Load(prefabFolderPath + typeof(T).Name) as GameObject;

				if (prefab == null)
				{
					Debug.LogError(typeof(T).Name + "'s prefab is not exist");

					managerObj = new GameObject(typeof(T).Name);
					instance = managerObj.AddComponent<T>();
				}
				else
				{
					managerObj = Instantiate(prefab);
					instance = managerObj.GetComponent<T>();

					if (instance == null)
					{
						Debug.LogError(typeof(T).Name + "'s prefab don't include" + typeof(T).Name + " Script!!");
					}
				}

				managerObj.name = managerObj.name.Replace("(Clone)", string.Empty);
			}

			GameObject boxObj = FindManagerBoxes(isDontDestroy);


			if (boxObj != null)
			{
				managerObj.transform.SetParent(boxObj.transform);
			}

		}
		else { Debug.LogError($"응~~~ {typeof(T).Name}이미 있어~~"); }
	}

	public static void InstantiateManagerByPrefab(GameObject prefab, bool isDontDestroy = true)
	{
		//프리팹 있는거 확신 할 때.
		//앵간하면 InstantiateManager로 퉁치삼

		Debug.LogWarning("U should be use \"InstantiateManager\" Funcs instead of \"InstantiateManagerByPrefab\".");

		if (prefab.name != typeof(T).Name)
		{
			Debug.LogError("매니저 프리팹이랑 실행시키는 곳이랑 다른데?? 확인해확인해확인해확인해");
		}

		GameObject newObj = null;

		if (instance == null)
		{
			newObj = Instantiate(prefab);
			newObj.name = newObj.name.Replace("(Clone)", string.Empty);
			instance = newObj.GetComponent<T>();

			GameObject boxObj = FindManagerBoxes(isDontDestroy);

			if (boxObj != null)
			{
				newObj.transform.SetParent(boxObj.transform);
			}

			if (instance == null)
			{
				Debug.LogError(typeof(T).Name + "'s prefab don't include" + typeof(T).Name + " Script!!");
			}
		}
		else
		{
			Debug.Log($"이미 {typeof(T).Name}있는디?\n확인해확인해확인해");
		}
	}


	public static GameObject FindManagerBoxes(bool isDontDestroy)
	{
		GameObject boxObj = null;

		if (isDontDestroy)
		{
			boxObj = Funcs.CheckGameObjectExist("ManagerBox");
			DontDestroyOnLoad(boxObj);
		}
		else
		{
			boxObj = Funcs.CheckGameObjectExist("ManagerBox_Destory");
		}

		return boxObj;
	}

	

	public virtual void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneChanged;

	}

	public virtual void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneChanged;

	}

	public virtual void OnSceneChanged(Scene scene, LoadSceneMode mode)
	{ }


	public virtual void OnDestroy()
	{
		instance = null;
	}

}
