using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using AYellowpaper.SerializedCollections;
using Enums;
using UnityEngine.Analytics;

public interface IPoolable
{
    void PoolableInit();
    void PoolableReset();
}

public class PoolingManager: Singleton<PoolingManager>
{
	[SerializedDictionary("Prefabs", "Pool Count")]
	public SerializedDictionary<GameObject, int> Prefabs;
	//정민아 여기다가 프리펩 하나씩 채우면 됨.
	//우리가 이제 폴더 정리를 객체 기준으로 하기로 해서
	//Prefab이라는 폴더 자체가 없음!!!


	List<KeyValuePair<GameObject, int>> allPrefabList;
	//private List<GameObject>prefabs; //인스펙터에서 담은 프리팹들 모아놓을 곳

	GameObject[] objBoxes; //각 오브젝트 담아 놓을 박스
    //빈 게임오브젝트, 인스펙터창에 실제로 만들꺼임
    //그니까 각 오브젝트 최상위 EmptyGameObjectpartial


    public Dictionary<string, Queue<GameObject>> poolingObjDic;



	public void CreateBoxes()
	{
		objBoxes = new GameObject[allPrefabList.Count];

		for(int i = 0; i< allPrefabList.Count; ++i)
		{
			GameObject box = new GameObject(allPrefabList[i].Key.name + "_Box");
			box.transform.SetParent(this.gameObject.transform);
			objBoxes[i] = box;
		}

		
	}

	public void FillAllObjects()
	{
		//1. 각 종류 프리팹들 한곳으로 모으기
		//prefabs = new List<GameObject>();

		//prefabs.AddRange(UnitPrefabs);
		//prefabs.AddRange(BuildingPrefabs);
		//prefabs.AddRange(EnemyPrefabs);
		allPrefabList = new();

		allPrefabList.AddRange(Prefabs.ToList());

		//2. 해당 길이만큼 박스 만들기
		CreateBoxes();


		//3. 이제 하나씩 채우기
		//굳이 각 오브젝트 속성(유닛,건물 등) 으로 하는거 보다
		//이런식으로 하는게 더 편한듯
		poolingObjDic = new Dictionary<string, Queue<GameObject>>();

		for(int i = 0; i< allPrefabList.Count; ++i)
		{
			if (allPrefabList[i].Key == null)
			{
				continue;
			}

			GameObject prefab = allPrefabList[i].Key;

			Queue<GameObject> tempQueue = new Queue<GameObject>();

			for (int k = 0; k < allPrefabList[i].Value; ++k)
			{
				GameObject tempObj = Instantiate(prefab, objBoxes[i].transform);
				tempObj.name = tempObj.name.Replace("(Clone)", string.Empty);
				tempObj.SetActive(false);
				tempQueue.Enqueue(tempObj);
			}

			poolingObjDic.Add(prefab.name, tempQueue);
		}
	}

	public void FillObject(string objName, int count)
	{
		var tempPair = poolingObjDic.FirstOrDefault(t => t.Key == objName);

		//List<GameObject> tempList = prefabs.ToList();
		//GameObject prefab = tempList.Find(x => x.name == objName);
		string boxName = objName + "_Box";
		for (int i = 0; i < objBoxes.Length; ++i)
		{
			if (objBoxes[i] == null)
			{
				continue;
			}

			if (objBoxes[i].name.Equals(boxName))
			{
				for (int k = 0; k < count; ++k)
				{
					GameObject newObj = Instantiate(allPrefabList[i].Key, objBoxes[i].transform);
					newObj.name = newObj.name.Replace("(Clone)", string.Empty);
					newObj.SetActive(false);
					tempPair.Value.Enqueue(newObj);
				}

				break;
			}
		}
	}

	//public T	LentalObj<T>(string obj) 

	public GameObject LentalObj(string objName, int count = 1)
	{ 
		//디스이즈 람다식
		var tempPair = poolingObjDic.FirstOrDefault(t => t.Key == objName);

         if (tempPair.Value.Count < count)
		{
			FillObject(objName, count * 2);

			GameObject obj = LentalObj(objName, count);

            return obj;
		}
		else 
		{
			GameObject tempObj = tempPair.Value.Dequeue();

            IPoolable[] poolables = tempObj.GetComponents<IPoolable>();
            foreach (var poolable in poolables)
            {
                poolable.PoolableInit();
            }

            tempObj.SetActive(true);
			tempObj.transform.SetParent(null);
			return tempObj;
		}
	}

	public void ReturnObj(GameObject obj)
	{
		//무~~주건 밖에서 리지드바디나 뭐 트랜스폼들 초기화 시키고 보내셈
		//아니면 반품처리함

		obj.transform.position = Vector3.zero;
		obj.transform.rotation = Quaternion.identity;
		//obj.transform.localScale = new Vector3(1f, 1f, 1f);
		
		string realName = obj.name.Replace("(Clone)", string.Empty);
		var tempPair = poolingObjDic.FirstOrDefault(t => t.Key == realName);
		string boxName = realName + "_Box";

		for (int i = 0; i < objBoxes.Length; ++i)
		{
			if (objBoxes[i] == null)
			{
				continue;
			}

			if (objBoxes[i].name.Equals(boxName))
			{
				bool isExist = false;
#if UNITY_EDITOR

				foreach (var item in tempPair.Value)
				{
					if (item == obj)
					{
						isExist = true;
						Potato.Debug.LogWarning("이미 풀링된 아이템입니다!!! 확인요망");
						break;
					}
				}
#endif
				if (!isExist)
				{
					obj.transform.SetParent(objBoxes[i].transform);
					obj.SetActive(false);

					IPoolable[] poolables = obj.GetComponents<IPoolable>();
					foreach (var poolable in poolables)
					{
						poolable.PoolableReset();
					}

					tempPair.Value.Enqueue(obj);
				}
			}
		}
	}

	void Awake()
	{
		Initailize(true);
		FillAllObjects();
    }



}
