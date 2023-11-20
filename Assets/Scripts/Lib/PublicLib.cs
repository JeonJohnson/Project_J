using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public static class Funcs
{
	public static bool IsNull<T>(T script) where T : class
	{
		return script == null;
	}

	#region C# default Val Type Casting
	public static string GetEnumName<T>(int index) where T : struct, IConvertible
	{//where 조건 struct, IConvertible => Enum으로 제한
		return Enum.GetName(typeof(T), index);
	}
	public static int FlagToEnum(int flag)
	{
		if (flag <= 0)
		{
			return -1;
		}

		int temp = flag;
		int count = 0;
		while (true)
		{
			if (temp == 1)
			{
				break;
			}

			temp = temp >> 1;

			++count;
		}

		return count;
	}
	
	/// <summary>
	/// Convert Boolean To Inteager
	/// </summary>
	/// <param name="boolean"></param>
	/// <returns></returns>
	public static int B2I(bool boolean)
	{
		//false => 값 무 (0)
		//true => 값 유 
		return Convert.ToInt32(boolean);
	}
	/// <summary>
	/// Convert Inteager To Boolean
	/// </summary>
	/// <param name="integer"></param>
	/// <returns></returns>
	public static bool I2B(int integer)
	{
		return Convert.ToBoolean(integer);
	}
	#endregion

	#region Data Structure
	public static List<T> Shuffle<T>(List<T> list)
	{
		for (int i = list.Count - 1; i > 0; i--)
		{
			System.Random random = new System.Random(Guid.NewGuid().GetHashCode());
			int rnd = random.Next(0, i);
			T temp = list[i];
			list[i] = list[rnd];
			list[rnd] = temp;
		}
		return list;
	}
	#endregion

	#region Find/Search GameObject
	public static GameObject FindGameObjectInChildrenByName(GameObject Parent, string ObjName)
	{
		if (Parent == null)
		{
			return null;
		}

		//그냥 transform.Find 로 찾으면 한 단계 아래 자식들만 확인함.
		int childrenCount = Parent.transform.childCount;

		GameObject[] findObjs = new GameObject[childrenCount];

		if (Parent.name == ObjName)
		{
			return Parent;
		}

		if (childrenCount == 0)
		{
			return null;
		}
		else
		{
			for (int i = 0; i < childrenCount; ++i)
			{
				findObjs[i] = FindGameObjectInChildrenByName(Parent.transform.GetChild(i).gameObject, ObjName);

				if (findObjs[i] != null && findObjs[i].name == ObjName)
				{
					return findObjs[i];
				}
			}

			return null;
		}
	}
	public static GameObject FindGameObjectInChildrenByTag(GameObject Parent, string ObjTag)
	{
		if (Parent == null)
		{
			return null;
		}

		int childrenCount = Parent.transform.childCount;

		GameObject[] findObjs = new GameObject[childrenCount];

		if (Parent.CompareTag(ObjTag))
		{
			return Parent;
		}

		if (childrenCount == 0)
		{
			return null;
		}
		else
		{
			for (int i = 0; i < childrenCount; ++i)
			{
				findObjs[i] = FindGameObjectInChildrenByTag(Parent.transform.GetChild(i).gameObject, ObjTag);

				if (findObjs[i] != null && findObjs[i].CompareTag(ObjTag))
				{
					return findObjs[i];
				}
			}
			return null;
		}
	}
	public static T FindComponentInNearestParent<T>(Transform curTransform) where T : Component
	{
		if (curTransform == null)
		{
			return null;
		}

		T tempComponent = curTransform.GetComponent<T>();

		if (tempComponent == null)
		{
			if (curTransform.parent != null)
			{
				tempComponent = FindComponentInNearestParent<T>(curTransform.parent);
			}
			else
			{
				return null;
			}
		}

		return tempComponent;
	}

	/// <summary>
	/// If dont Exist, Create new GameObject.
	/// </summary>
	/// <param name="name">GameObject's Name</param>
	/// <returns></returns>
	public static GameObject CheckGameObjectExist(string name)
	{
		GameObject temp = GameObject.Find(name);

		if (temp == null)
		{
			temp = new GameObject(name);
		}

		return temp;
	}
	/// <summary>
	/// If dont Exist, Create new GameObject. And Add Component
	/// <para>
	/// return GameObject that u find
	/// </para>
	/// </summary>
	/// <typeparam name="T">T is Component that have GameObject to Find </typeparam>
	/// <param name="objName">GameObject's Name</param>
	/// <returns></returns>
	public static GameObject CheckGameObjectExist<T>(string objName) where T : Component
	{
		GameObject tempObj = GameObject.Find(objName);

		if (tempObj == null)
		{
			tempObj = new GameObject(objName);
		}

		T tempComponent = tempObj.GetComponent<T>();

		if (tempComponent == null)
		{
			tempObj.AddComponent<T>();
		}

		return tempObj;
	}
	/// <summary>
	/// If dont Exist, Create new GameObject. And Add Component
	/// <para>
	/// return Component that u want.
	/// </para>
	/// </summary>
	/// <typeparam name="T">T is Component that have GameObject to Find </typeparam>
	/// <param name="gameObjectName">GameObject's Name</param>
	/// <returns></returns>
	public static T CheckComponentExist<T>(string gameObjectName) where T : Component
	{
		GameObject temp = GameObject.Find(gameObjectName);

		if (temp == null)
		{
			temp = new GameObject(gameObjectName);
		}

		T tempComponent = temp.GetComponent<T>();

		if (tempComponent == null)
		{
			tempComponent = temp.AddComponent<T>();
		}

		return tempComponent;
	}
	#endregion

	#region Color
	public static Color SetAlpha(Color color, float alpha)
	{
		Color tempColor = color;
		tempColor.a = alpha;
		return tempColor;
	}
	public static void SetAlpha(ref Color color, float alpha)
	{
		Color tempColor = color;
		tempColor.a = alpha;
		color = tempColor;
	}
	public static void SetGizmoColor(Color color, float alpha = 1f)
	{
		Color temp = color;
		temp.a = alpha;
		Gizmos.color = temp;
	}
	#endregion

	#region Animation
	public static bool IsAnimationAlmostFinish(Animator animCtrl, string animationName, float ratio = 0.95f)
	{
		if (animCtrl.GetCurrentAnimatorStateInfo(0).IsName(animationName))
		{//여기서 IsName은 애니메이션클립 이름이 아니라 애니메이터 안에 있는 노드이름임
			if (animCtrl.GetCurrentAnimatorStateInfo(0).normalizedTime >= ratio)
			{
				return true;
			}
		}
		return false;
	}
	public static bool IsAnimationCompletelyFinish(Animator animCtrl, string animationName, float ratio = 1.0f)
	{
		if (animCtrl.GetCurrentAnimatorStateInfo(0).IsName(animationName))
		{//여기서 IsName은 애니메이션클립 이름이 아니라 애니메이터 안에 있는 노드이름임
			if (animCtrl.GetCurrentAnimatorStateInfo(0).normalizedTime >= ratio)
			{
				return true;
			}
		}
		return false;
	}
	#endregion

	#region FileIO
	#endregion
}

public static class Defines
{
#if UNITY_EDITOR
	public static int editorStartScene = -1;
#endif

	public const float winCX = 1600f;
	public const float winCY = 900f;

	public const float PI = 3.14159265f;


	public static string dontDestoryMgrBox = "ManagerBox";
	public static string destoryMgrBox = "ManagerBox_Destroy";

	public static string managerPrfabFolderPath = "Managers/Prefabs";

}

namespace Structs
{ 

}
