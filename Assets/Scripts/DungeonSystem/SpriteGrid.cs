using System.Collections;
using System.Collections.Generic;
using UnityEngine;


////How to USE////
/*
1. Set PlaneObject's Scale Size that u want grid count
2. Update Grid()
 */
////How to USE////


#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(SpriteGrid))]
public class SpriteGrid_Editor : Editor
{
	public override void OnInspectorGUI()
	{
		SpriteGrid grid = (SpriteGrid)target;

		DrawDefaultInspector();


		GUILayout.Label("\n");

		if (GUILayout.Button("Update"))
		{
			grid.UpdateGrid();
		}
	}
}

#endif

public class SpriteGrid : MonoBehaviour
{
    [Tooltip("ur not insert planeObj, this script set mother Object to planeObj.")]
    public GameObject PlaneObject;

    public Vector2 defaultTileSize;

	[HideInInspector]
    public SpriteRenderer mySR;
	[HideInInspector]
	public SpriteRenderer planeSR;
    public void UpdateGrid()
    {
        Vector2 planeObjSize = PlaneObject.transform.localScale;
		if (!mySR)
		{ mySR = GetComponent<SpriteRenderer>(); }
        transform.localScale = new Vector2(1 / planeObjSize.x * defaultTileSize.x, 1 / planeObjSize.y * defaultTileSize.y);
        mySR.size = new Vector2 (planeObjSize.x / defaultTileSize.x, planeObjSize.y / defaultTileSize.y);
    }

	private void Awake()
	{
		mySR = GetComponent<SpriteRenderer>();
	}
	// Start is called before the first frame update
	void Start()
    {
		//유니티에서는 ?? 연산자 웬만하면 쓰지말라함.
		//null 비교 연산 자체도 오버로딩 되있는거라서 그런듯
		//걍 한줄로 처리하자

		//PlaneObject ??= transform.parent.gameObject;
		if(!PlaneObject) PlaneObject = transform.parent.gameObject;
		//planeSR ??= PlaneObject.GetComponent<SpriteRenderer>();
		if(!planeSR) planeSR = PlaneObject.GetComponent<SpriteRenderer>();
		if (planeSR)
		{ mySR.sortingOrder = planeSR.sortingOrder + 1; }
	}

  
}
