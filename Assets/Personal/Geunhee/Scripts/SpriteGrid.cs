using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///How to Setting///
/*
1. Create Empty GameObject and Add 'Sprite Grid(this Script)' Component.
2. Create child Object. and Add Sprite Renderer Component
3. Option of the child object's SpriteRenderer
3-1. draw Type : Tiled
3-2. Sprite : Grid 
*/
///How to Setting///


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
    //public GameObject PlaneObject;

    public Vector2 defaultTileSize = Vector2.one;
	
	//public SpriteRenderer planeSR; //본인 (부모)
    public SpriteRenderer gridSR; //자식 (SpriteRenderer에서 Draw 모드 Tiled로 되어있는 친구
    public void UpdateGrid()
    {
        Vector2 planeObjSize = transform.localScale;
		if (!gridSR)
		{ gridSR = transform.GetChild(0).GetComponent<SpriteRenderer>(); }
        gridSR.transform.localScale = new Vector2(1 / planeObjSize.x * defaultTileSize.x, 1 / planeObjSize.y * defaultTileSize.y);
        gridSR.size = new Vector2 (planeObjSize.x / defaultTileSize.x, planeObjSize.y / defaultTileSize.y);
    }

	private void Awake()
	{
		//planeSR = GetComponent<SpriteRenderer>();

		if (defaultTileSize == Vector2.zero)
		{
			defaultTileSize = Vector2.one;
		}

		if (!gridSR)
		{ 
			gridSR = transform.GetChild(0).GetComponent<SpriteRenderer>(); 
		}
	}
	// Start is called before the first frame update
	void Start()
    {
		//유니티에서는 ?? 연산자 웬만하면 쓰지말라함.
		//null 비교 연산 자체도 오버로딩 되있는거라서 그런듯
		//걍 한줄로 처리하자

		//PlaneObject ??= transform.parent.gameObject;
		//if(!PlaneObject) PlaneObject = transform.parent.gameObject;
		//planeSR ??= PlaneObject.GetComponent<SpriteRenderer>();

		//if(!planeSR) planeSR = PlaneObject.GetComponent<SpriteRenderer>();
		//if (planeSR)
		//{ gridSR.sortingOrder = planeSR.sortingOrder + 1; }
	}

  
}
