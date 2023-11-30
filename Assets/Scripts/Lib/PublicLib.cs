using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Structs;

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

    public static T IsComponentExistInFamily<T>(GameObject obj) where T : Component
    {
        T temp = obj.GetComponent<T>();

        if (temp)
        {
            return temp;
        }
        else
        {
            temp = obj.GetComponentInParent<T>();

            if (temp)
            {
                return temp;
            }
            else
            {
                return obj.GetComponentInChildren<T>();
            }
        }
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

    #region CalcAngle
    public static Vector3 DegreeAngle2Dir(float degreeAngle)
    {
        //阿档甫 氦磐肺 官层林绰 芭

        //ex)雀傈登瘤 臼篮 坷宏璃飘牢 版快
        //rotation狼 y蔼 euler蔼 持栏搁 forward Dir唱咳.

        //炼陛 歹 磊技茄 郴侩篮 氦磐 郴利, 寇利 毫焊祭

        float radAngle = degreeAngle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radAngle), Mathf.Cos(radAngle));
    }
    #endregion

    #region Find 
    public static Sprite FindSprite(string imageName, string spriteName)
    {
        Sprite[] all = Resources.LoadAll<Sprite>(imageName);

        foreach (var s in all)
        {
            if (s.name == spriteName)
            {
                return s;
            }
        }
        return null;
    }
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


	public static string dontDestoryMgrBoxName = "ManagerBox";
	public static string destoryMgrBoxName = "ManagerBox_Destroy";

	public static string managerPrfabFolderPath = "Managers/Prefabs";

}

namespace Structs
{

	[System.Serializable]
    public struct PlayerStatus
    {
        [Header("Stat")]
        public int maxHp;
        public int curHp;
        public int maxArmor;
        public int curArmor;
        public float walkSpeed;
        public float runSpeed;

        public float rollSpeed;
        public float rollSpeedDrop;

        [Header("Hit")]
        public bool isInvincible;
		public float invincibleTimeWhenHit;

		public bool isDead;
    }

    [System.Serializable]
    public struct EnemyStatus
    {
        [Header("Stat")]
        public int maxHp;
        public int curHp;
        public float walkSpeed;

		[Header("Range")]
		public float traceRange;
        public bool isInvincible;
		public bool isDurable;

		[Header("Combat")]
		public float fireTimer;
		public float fireWaitTime;
        public float fireRate;
        public int bulletCountWhenAttackOnce;

		public float attackRange;
		public float spread;
    }

    public struct HitInfo
    {
        public bool isDurable;
        public bool isHitSucess;
    }
}

namespace Enums
{
	public enum Item_Type
	{
		Passive,
		Active,
		End
	}

    public enum PlayerMoveActions
    {
        None,
        Move,
        Roll,
        End
    }

    public enum SlimeActions
    {
        Idle,
        Move,
        Attack,
		Death,
        End
    }

    public enum BangtaniActions
    {
        Idle,
        Move,
        Attack,
        Death,
        End
    }
}

namespace JeonJohnson
{
	public enum TreeSearchOption
	{ 
		PRE, //Preorder : 전위 순회 (mother -> left -> right)
		IN, //Inorder : 중위 순회 (leaf left -> mother -> right)
		POST //Postorder : 후위 순회 (leaf left -> right -> mother)
	}

	/// <summary>
	/// 랜덤 방생성 (BSP 알고리즘)에서 쓸려고 간단하게 만든
	/// 이진 트리임 (완전 바이너리 트리)
	/// 필요하다면, 웬만하면 있는 Linq 자료구조 쓰셈ㅋ;
	/// 와~ 나 무적권 Tree필요하다 하면,,, 말해,,, 주세요,,,,
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[System.Serializable]
	public class Tree<T> /*: IEnumerable, IEnumerator*/
	{
		public Tree()
		{
			root = null;

			nodeList = new List<TreeNode<T>>();

			//root.Depth = 0;
			//root.Index = 0;
			//maxDepth = 0;
		}

		public Tree(T rootData)
		{
			root = new TreeNode<T>(rootData,0,0);

			nodeList = new List<TreeNode<T>>();
			nodeList.Add(root);

			Count = 1;
			//maxDepth = 0;
		}

		public Tree(TreeNode<T> rootNode)
		{
			root = rootNode;

			nodeList = new List<TreeNode<T>>();
			nodeList.Add(root);

			Count = 1;
			//maxDepth = 0;
		}

		private TreeNode<T> root;
		public TreeNode<T> RootNode
		{
			get { return root; }
			set { root = value; }
		}

		//순회 위해서 만들었는디, Node의 Index 순서대로 ㄱㄱㄱ
		//나중에 여유 있으면 보고 IEnumerable, IEnumerator 추가해서 ㄱㄱ
		public List<TreeNode<T>> nodeList;

		public int Count { get; private set; }
		

		//private int maxDepth;
		//public int MaxDepth
		//{
		//	get { return maxDepth; }
		//}

		private TreeNode<T> lastNode;
		
		public TreeNode<T> FindLastNode()
		{
			lastNode = root.NextNode();
			Count = lastNode.Index + 1;
			return lastNode;
		}

		public List<TreeNode<T>> GetLeafNodes()
		{
			List<TreeNode<T>> leafNodes = new List<TreeNode<T>>();

			leafNodes.AddRange(root.GetLeafChild());

			return leafNodes;
		}


		public void AddNode(TreeNode<T> left, TreeNode<T> right)
		{
			FindLastNode();

			left.MotherNode = lastNode;
			right.MotherNode = lastNode;

			lastNode.LeftNode = left;
			lastNode.RightNode = right;

			left.SiblingNode = right;
			right.SiblingNode = left;

			left.Index = lastNode.Index + 1;
			right.Index = lastNode.Index + 2;

			left.Depth = lastNode.Depth + 1;
			right.Depth = lastNode.Depth + 1;

			nodeList.Add(left);
			nodeList.Add(right);

			Count = right.Index + 1;
		}

		public void AddNode(T left, T right)
		{
			FindLastNode();

			TreeNode<T> leftNode = new TreeNode<T>(left, lastNode.Depth + 1, lastNode.Index +1);
			TreeNode<T> rightNode = new TreeNode<T>(right, lastNode.Depth +1, lastNode.Index+2);

			leftNode.MotherNode = lastNode;
			rightNode.MotherNode = lastNode;

			lastNode.LeftNode = leftNode;
			lastNode.RightNode = rightNode;

			leftNode.SiblingNode = rightNode;
			rightNode.SiblingNode = leftNode;

			//leftNode.Index = lastNode.Index + 1;
			//rightNode.Index = lastNode.Index + 2;

			//leftNode.Depth = lastNode.Depth + 1;
			//rightNode.Depth = lastNode.Depth + 1;
			
			nodeList.Add(leftNode);
			nodeList.Add(rightNode);

			Count = rightNode.Index + 1;
		}

		/////IEnumerator <summary>
		//public object Current => throw new NotImplementedException();
		//public bool MoveNext()
		//{
			
		//}

		//public void Reset()
		//{
			
		//}
		/////IEnumerator </summary>
		/////
		/////IEnumerable<summary>
		//public IEnumerator GetEnumerator()
		//{
		//	throw new NotImplementedException();
		//}
		/////IEnumerable</summary>

	
	}
	[System.Serializable]
	public class TreeNode<T> 
	{
		public TreeNode()
		{
			value = default(T);
			left = null;
			right = null;

			depth = -1;
			index = -1;
		}
		public TreeNode(T data)
		{
			value = data;
			left = null;
			right = null;

			depth = -1;
			index = -1;
		}

		public TreeNode(T data, int _depth, int _index)
		{
			value = data;
			left = null;
			right = null;

			depth = _depth;
			index = _index;
		}

		//~TreeNode()
		//{
		//	//value = null;
		//	mother = null;
		//	sibling = null;
		//	left = null;
		//	right = null;
		//}


		private int depth;
		public int Depth
		{
			get { return depth; }
			set { depth = value; }
		}

		private int index;
		public int Index
		{
			get { return index; }
			set { index = value; }
		}


		private T value;
		public T Value
		{
			get { return value; }
			set { this.value = value; }
		}

		private TreeNode<T> mother;
		public TreeNode<T> MotherNode
		{
			get { return mother; }
			set { mother = value; }
		}

		private TreeNode<T> sibling;
		public TreeNode<T> SiblingNode
		{
			get { return sibling; }
			set { sibling = value; }
		}
		
		private TreeNode<T> left;
		public TreeNode<T> LeftNode
		{
			get { return left; }
			set { left = value; }
		}

		private TreeNode<T> right;
		public TreeNode<T> RightNode
		{
			get { return right; }
			set { right = value; }
		}


		public bool IsLeaf()
		{
			return (left == null && right == null);
		}

		public List<TreeNode<T>> GetLeafChild()
		{
			var list = new List<TreeNode<T>>();

			if (left == null)
			{
				list.Add(this);
				return list;
			}
			else if (left.IsLeaf())
			{
				list.Add(left);
			}
			else
			{
				list.AddRange(left.GetLeafChild());
			}

			
			if (right.IsLeaf())
			{
				list.Add(right);
			}
			else
			{
				list.AddRange(right.GetLeafChild());
			}

			return list;
		}



		public TreeNode<T> NextNode()
		{ //전위순회 기준에서
			if (left != null)
			{
				return left.NextNode();
			}
			else if (right != null)
			{
				//return right;
				return right .NextNode();
			}
			else 
			{
				if (mother != null && mother.sibling != null)
				{
					return mother.sibling.NextNode();
				}
				else
				{
					return this;
				}
			}
		}


		

		//public TreeNode<T> FindNode(T data)
		//{
		//	if (left != null)
		//	{
		//		if (left.value.GetHashCode() == data.GetHashCode())
		//		{
		//			return LeftNode;
		//		}
		//		else
		//		{
		//			left.FindNode(data);
		//		}
		//	}
		//	else
		//	{ //가장 마지막 왼쪽인 경우.
		//		if(right)
				
		//	}
		//}
	}

}
