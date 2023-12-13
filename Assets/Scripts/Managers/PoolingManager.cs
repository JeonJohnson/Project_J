using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class PoolingManager : Singleton<PoolingManager>
{
	[SerializeField] GameObject[] prefabs; //�����յ� ��ƵѰ�
    string prefabsFolderPath = "Prefabs";

    GameObject[] objBoxes; //�� ������Ʈ ��� ���� �ڽ�
						   //�� ���ӿ�����Ʈ, �ν�����â�� ������ ���鲨��
						   //�״ϱ� �� ������Ʈ �ֻ��� EmptyGameObject

	Queue<GameObject>[] poolingObjQueue;
	//���⿡ ��Ƶΰ� �ϳ��� ��������

	public Dictionary<string, Queue<GameObject>> poolingObjDic;


	[HideInInspector]
	public List<GameObject> trashBin = new List<GameObject>();
	public Transform trashBinCan;

	public void AddTrashBin(GameObject obj)
	{
		obj.SetActive(false);
		obj.transform.SetParent(trashBinCan);
		trashBin.Add(obj);
	}

	public void CreateBoxes()
	{
        prefabs = Resources.LoadAll<GameObject>(prefabsFolderPath);
        objBoxes = new GameObject[prefabs.Length];

		for (int i = 0; i < prefabs.Length; ++i)
		{
			if (prefabs[i] == null)
			{
				continue;
			}

			GameObject box = new GameObject(prefabs[i].name + "_Box");
			box.transform.SetParent(this.gameObject.transform);
			objBoxes[i] = box;
		}
	}

	public void FillAllObjects(int iCount = 10)
	{
		poolingObjDic = new Dictionary<string, Queue<GameObject>>();

		//		foreach (var prefab in prefabs)
		for (int i = 0; i < prefabs.Length; ++i)
		{
			if (prefabs[i] == null)
			{
				continue;
			}

			GameObject prefab = prefabs[i];

			Queue<GameObject> tempQueue = new Queue<GameObject>();

			for (int k = 0; k < iCount; ++k)
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
				GameObject newObj = Instantiate(prefabs[i], objBoxes[i].transform);
				newObj.name = newObj.name.Replace("(Clone)", string.Empty);
				newObj.SetActive(false);
				tempPair.Value.Enqueue(newObj);
			}
		}
	}

	public GameObject LentalObj(string objName, int count = 1)
	{
		//������ ���ٽ�
		var tempPair = poolingObjDic.FirstOrDefault(t => t.Key == objName);
		if (tempPair.Value.Count < count)
		{
			FillObject(objName, count * 2);
			return LentalObj(objName, count);
		}
		else
		{
			GameObject tempObj = tempPair.Value.Dequeue();
			tempObj.SetActive(true);
			tempObj.transform.SetParent(null);
			return tempObj;
		}
	}

	public void ReturnObj(GameObject obj)
	{
		//��~~�ְ� �ۿ��� ������ٵ� �� Ʈ�������� �ʱ�ȭ ��Ű�� ������
		//�ƴϸ� ��ǰó����

		obj.transform.position = Vector3.zero;
		obj.transform.rotation = Quaternion.identity;
		//obj.transform.localScale = new Vector3(1f, 1f, 1f);

		string realName = obj.name.Replace("(Clone)", string.Empty);
		var tempPair = poolingObjDic.FirstOrDefault(t => t.Key == realName);

		//default(KeyValuePair<string, Queue<GameObject>>);

		if (default(KeyValuePair<string, Queue<GameObject>>).Equals(tempPair))
		{
			obj.SetActive(false);
			return;
		}

		string boxName = realName + "_Box";

		for (int i = 0; i < objBoxes.Length; ++i)
		{
			if (objBoxes[i] == null)
			{
				continue;
			}

			if (objBoxes[i].name.Equals(boxName))
			{
				obj.transform.SetParent(objBoxes[i].transform);
				obj.SetActive(false);
				tempPair.Value.Enqueue(obj);
			}
		}
	}


	public void Awake()
	{
		CreateBoxes();
		FillAllObjects();
	}


	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}
}

