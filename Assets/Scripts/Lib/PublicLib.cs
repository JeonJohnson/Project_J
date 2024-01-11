using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Structs;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public static class Funcs
{
	public static bool IsNull<T>(T script) where T : class
	{
		return script == null;
	}

	public static float Random_Dot5(float min, float max)
	{
		//C# float to Int 명시적 캐스팅은 무조건 버림임.
		float temp = UnityEngine.Random.Range(min, max);

		temp = temp - Mathf.FloorToInt(temp) > 0.5 ? Mathf.FloorToInt(temp) + 0.5f : Mathf.FloorToInt(temp);

		return temp;
	}

	public static Vector3 ToV3(Vector2Int vec2)
	{
		return new Vector3(vec2.x, vec2.y, 0f);
	}
	public static Vector2 ToV2(Vector2Int vec2)
	{
		return new Vector2(vec2.x, vec2.y);
	}

	
	public static int PingPongInt(int t, int length)
	{//t is 증감값
		int repeatedValue = t % (length * 2);
		return length - Math.Abs(repeatedValue - length);
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
		//이거 주석 와이라노,,, 암 코리안 돈 슛 미 

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

    #region Array Sorting
    public static void ArrayAdd<T>(T[]itemSlots, T newItem)
    {
        // 먼저 비어있는 슬롯을 찾음
        int emptySlotIndex = -1;
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i] == null)
            {
                emptySlotIndex = i;
                break;
            }
        }

        // 비어있는 슬롯이 있는 경우 추가
        if (emptySlotIndex != -1)
        {
            itemSlots[emptySlotIndex] = newItem;
            Debug.Log("Item added to slot " + emptySlotIndex);
        }
        else
        {
            Debug.LogWarning("Inventory is full, cannot add item");
        }
    }

    public static void ArrayRemove<T>(T[] itemSlots, T item)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (EqualityComparer<T>.Default.Equals(itemSlots[i], item))
            {
                itemSlots[i] = default(T); // 혹은 null로 설정할 수 있음
                return;
            }
        }
        Debug.LogWarning("Item not found in the inventory");
    }

    public static void ArrayRemoveByIndex<T>(T[] itemSlots, int index)
    {
        if (index >= 0 && index < itemSlots.Length)
        {
            itemSlots[index] = default(T); // 혹은 null로 설정할 수 있음
        }
        else
        {
            Debug.LogError("Invalid index to remove item");
        }
    }

    // 아이템 배열이 꽉 찼는지 확인하는 메서드
    public static bool IsArrayFull<T>(T[] itemSlots)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i] == null)
            {
                return false; // 배열에 비어있는 슬롯이 하나라도 있으면 아직 꽉 차지 않음
            }
        }
        return true; // 배열의 모든 슬롯이 차있음
    }

    public static void ArrayReplace<T>(T[] itemSlots, T newItem, int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < itemSlots.Length)
        {
            itemSlots[slotIndex] = newItem;
        }
        else
        {
            Debug.LogError("Invalid slot index");
        }
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

	public static Color[] rainbow =
	{
		Color.red,
		new Color(1, 127f/255f, 0f),
		Color.yellow,
		Color.green,
		Color.blue,
		new Color(0f,0f,0.5f),
		new Color(139f/255f, 0f, 1f)
	};
}

namespace Structs
{

	[System.Serializable]
    public struct PlayerStatus
    {
        [Header("Stat")]
        public int maxHp;
        public Data<int> curHp;
        public int maxArmor;
        public int curArmor;
        public float walkSpeed;

        public float rollSpeed;
        public float rollSpeedDrop;

		public float criticalPercent;

        [Header("Hit")]
        public bool isInvincible;
		public float invincibleTimeWhenHit;

		public bool isDead;


    }

    [System.Serializable]
    public struct WeaponStatus
	{
        public float bulletSpeed;
        public float bulletSpread;
		public float bulletSize;
        public int bulletNumPerFire;
		public int bulletSplatterCount;
        public float damage;
        public float fireRate;
        public float critial;

		public Data<int> bulletCount;
    }

    [System.Serializable]
    public struct BonusStatus
    {
        [Header("Player")]
        public int bonus_Player_Hp;
        public int bonus_Player_Armor;
        public float bonus_Player_Speed;

        [Header("Weapon")]
        public float bonus_Weapon_Speed; // 합연산
        public float bonus_Weapon_Spread; // 곱연산
        public float bonus_Weapon_Damage; // 곱연산
        public float bonus_Weapon_FireRate; // 곱연산
        public int bonus_Weapon_BulletNumPerFire;  // 합연산
        public float bonus_Weapon_Critial; // 합연산
		public float bonus_Weapon_BulletSize;
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
        public int fireCountPerAttack;
        public int bulletNumPerFire;
		public float bulletSize;

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
		Useable,
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

    public enum TangtangiActions
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

    public enum HwasariActions
    {
        Idle,
        Move,
        Attack,
        Death,
        End
    }

    public enum BossDemoActions
    {
        Idle,
        Move,
        Attack0,
        Attack1,
        Attack2,
        Death,
		Hide,
        End
    }

	public enum MonsterNames
	{
		Bangtani,
		Tangtangi,
		Hwasari
	}

    public enum ItemNames
    {
        Active_Dash,
        Passive_BulletSpeedUp,
        Passive_Laser,
        Passive_Rapid,
        Passive_Shotgun
    }

	public enum BossNames
	{
		Boss_Demo
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
	//[System.Serializable]
	public class Tree<T> /*: IEnumerable, IEnumerator*/
	{
		public Tree()
		{
			root = null;

			nodeList = new List<TreeNode<T>>();

			Count = 0;
		}

		public Tree(T rootData)
		{
			root = new TreeNode<T>(rootData,0,0);

			nodeList = new List<TreeNode<T>>
			{
				root
			};

			Count = 1;
		}

		public Tree(TreeNode<T> rootNode)
		{
			root = rootNode;

			nodeList = new List<TreeNode<T>>();
			nodeList.Add(root);

			Count = 1;
			//maxDepth = 0;
		}

		~Tree()
		{ 
			
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
		


		public List<TreeNode<T>> GetLeafNodes()
		{
			List<TreeNode<T>> leafNodes = new List<TreeNode<T>>();

			leafNodes.AddRange(root.GetLeafChildren());

			return leafNodes;
		}

		public List<TreeNode<T>> GetCertainDepthNodes(int depth)
		{
			return nodeList.FindAll(x => x.Depth == depth);
		}


		public void AddNode(TreeNode<T> motherNode, TreeNode<T> left = null, TreeNode<T> right = null)
		{
			if (nodeList.Count != 0)
			{
				//FindLastNode();

				left.MotherNode = motherNode;
				right.MotherNode = motherNode;

				motherNode.LeftNode = left;
				motherNode.RightNode = right;

				left.SiblingNode = right;
				right.SiblingNode = left;

				left.Index = nodeList.Count;
				right.Index = left.Index +1 ;

				left.Depth = motherNode.Depth + 1;
				right.Depth = motherNode.Depth + 1;

				nodeList.Add(left);
				nodeList.Add(right);

				//Count = right.Index + 1;
				Count = nodeList.Count;
			}
			else if(left == null && right == null && nodeList.Count == 0)
			{
				root = motherNode;
				root.Index = 0;
				nodeList.Add(motherNode);
			}


		}

		public void AddNode(TreeNode<T> motherNode, T left, T right)
		{
			TreeNode<T> leftNode, rightNode;

			if (nodeList.Count != 0)
			{
				leftNode = new TreeNode<T>(left, motherNode.Depth +1 , nodeList.Count);
				 rightNode= new TreeNode<T>(right, motherNode.Depth + 1, nodeList.Count+1);

				leftNode.MotherNode = motherNode;
				rightNode.MotherNode = motherNode;

				motherNode.LeftNode = leftNode;
				motherNode.RightNode = rightNode;

				leftNode.SiblingNode = rightNode;
				rightNode.SiblingNode = leftNode;

				nodeList.Add(leftNode);
				nodeList.Add(rightNode);

				Count = nodeList.Count;
			}
			//else if()
			//{//root 넣을때
			//	leftNode = new TreeNode<T>(left, 0, 0);
			//	root = leftNode;
			//	root.Index = 0;
			//	nodeList.Add(leftNode);
			//}
		}

		public void SetRootNode(TreeNode<T> node)
		{
			nodeList.Add(node);
			root = node;
			Count = 1;
		}

	
	}
	//[System.Serializable]
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

		~TreeNode()
		{
			//value = null;
			mother = null;
			sibling = null;
			left = null;
			right = null;
		}


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

		//public List<TreeNode<T>> GetAllChildren(TreeNode<T> parent)
		//{
		//	var list = new List<TreeNode<T>>();

		//	if (IsLeaf())
		//	{
		//		if (this == parent)
		//		{
		//			return null;
		//		}
		//		else
		//		{
		//			list.Add(this);
		//		}
		//	}
		//	else
		//	{
		//		list.AddRange(left.GetAllChildren(parent));
		//		list.AddRange(right.GetAllChildren(parent));
		//	}

		//	return list;
		//}

		public void GetLeafChildren(TreeNode<T> parent, ref List<TreeNode<T>> list)
		{
			if (IsLeaf())
			{
				list.Add(this);
			}
			else
			{
				left.GetLeafChildren(parent, ref list);
				right.GetLeafChildren(parent, ref list);
			}
		}

		public List<TreeNode<T>> GetLeafChildren()
		{
			var list = new List<TreeNode<T>>();

			if (IsLeaf())
			{
				list.Add(this);
			}
			else
			{
				list.AddRange(left.GetLeafChildren());
				list.AddRange(right.GetLeafChildren());
			}

			return list;
		}



		public TreeNode<T> NextNode()
		{ //전위순회 방법으로
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
				return this;
			}
		}

	}

}
