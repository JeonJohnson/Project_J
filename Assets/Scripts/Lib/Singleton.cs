using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	//제네뤽 클래스
	//C++의 템플릿 클래스 비슷한거임
	private static T instance = null;
	//public bool isDontDestory;
	//스스로 Awake에서 SetDestructible 호출해서 설정해주셈

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

		if (instance == null)
		{
			string managerName = typeof(T).Name;
			GameObject managerObj = null;
			T tempInstance = FindObjectOfType<T>();

			if (tempInstance)
			{ //이미 하이어라키창에 오브젝트가 올라가져 있을 경우

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
		}

		return instance;
	}

	public static void CreateManagerBoxes()
	{
		DontDestroyOnLoad(Funcs.CheckGameObjectExist("ManagerBox"));
		Funcs.CheckGameObjectExist("ManagerBox_Destory");
	}

	/// <summary>
	/// Plz Must Call this Method at Awake instead of 'DontDestroyOnLoad'
	/// </summary>
	/// <param name="isDestroy"></param>
	public void SetDestructible(bool isDestroy)
	{
		string boxName = string.Empty;
		boxName = isDestroy ? Defines.destoryMgrBox : Defines.dontDestoryMgrBox;
		gameObject.transform.SetParent(Funcs.CheckGameObjectExist(boxName).transform);
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
