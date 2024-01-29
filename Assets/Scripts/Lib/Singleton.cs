using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


//원래 버젼은 
//싱글톤을
//Non-mono / mono비헤이비어상속 / 프리팹 / 하이어라키창
//의 여러 경우의 수를 모두 상정해서 
//편하게 사용할 수 있게 할라했는디
//생성을 코드로 시킨다거나 Awake에서 다른 싱글톤 객체 참조하거나 하는 등
//여러 경우에서 문제가 좀 생겼음

//그래서 이제는
//1. 각 싱글톤들은 초기화 시점 (Awake / 이니셜 라이즈 등) 에서 instance 체크/할당/생성 하기
//2. 이제 딴곳에서 참조할때 null이면 만드는게 아니라 그냥 null 반환할꺼임.
	//=> null이면 우리가 씬 구성 자체를 잘못한거임. 거기서 잡야주면됨.
//3. 만약 singleton 객체 Awake에서 체크해서 null이 아닌 경우 본인 바로 지우기



public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T instance = null;

	public static T Instance
	{
		get
		{
			//if (instance == null)
			//{
			//	instance = InstantiateManager();
			//}
			return instance;
		}
	}

    public void Initailize(bool destory)
	{
		if (instance == null)
		{
			instance = gameObject.GetComponent<T>();

			if (!destory)
			{
				DontDestroyOnLoad(gameObject);
			}
		}
		else
		{
			DestroyImmediate(gameObject);
		}
	}

    //public static T InstantiateManager(/*bool isDontDestroy*/)
    //{
    //	//1. 하이어라키 창에 있는지 확인.
    //	//2. 없으면 ManagerPrefabs 폴더안에 같은이름 프리팹 있는지 확인.
    //	//3. 없으면 새로 만듬.

    //	if (instance == null)
    //	{
    //		string managerName = typeof(T).Name;
    //		GameObject managerObj = null;
    //		T tempInstance = FindObjectOfType<T>();

    //		if (tempInstance)
    //		{ //이미 하이어라키창에 오브젝트가 올라가져 있을 경우

    //			managerObj = tempInstance.gameObject;
    //		}
    //		else
    //		{ //없는 경우 (새로 게임오브젝트 만들어야함)
    //		  //1. 프리팹 찾아보기
    //			GameObject prefab = Resources.Load(Defines.managerPrfabFolderPath + typeof(T).Name) as GameObject;

    //			if (prefab)
    //			{
    //				managerObj = Instantiate(prefab); //여기서 Awake 한번 더 호출함.
    //				managerObj.name = managerObj.name.Replace("(Clone)", string.Empty);
    //				tempInstance = managerObj.GetComponent<T>();
    //			}
    //			else
    //			{
    //				managerObj = new GameObject(managerName);
    //				tempInstance = managerObj.AddComponent<T>();//여기서 Awake 한번 더 호출함.
    //			}
    //		}

    //		instance = tempInstance;
    //	}

    //	return instance;
    //}

    //public static void CreateManagerBoxes()
    //{
    //	DontDestroyOnLoad(Funcs.CheckGameObjectExist("ManagerBox"));
    //	Funcs.CheckGameObjectExist("ManagerBox_Destory");
    //}

    ///// <summary>
    ///// Plz Must Call this Method at Awake instead of 'DontDestroyOnLoad'
    ///// </summary>
    ///// <param name="isDestroy"></param>
    //public void SetDestructible(bool isDestroy)
    //{
    //	string boxName = string.Empty;
    //	boxName = isDestroy ? Defines.destoryMgrBoxName : Defines.dontDestoryMgrBoxName;
    //	gameObject.transform.SetParent(Funcs.CheckGameObjectExist(boxName).transform);
    //}


    //public virtual void Awake()
    //{

    //}

    private void Awake()
    {
        Debug.Log("매니저 이닛" + this.gameObject.name);
        Initailize(false);
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
		var obj = GameObject.Find(gameObject.name);

		if (obj == null)
		{
			instance = null;
		}
	}

}
