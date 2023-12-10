using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    public Vector2 defaultSize;

    private SpriteRenderer mySR;
    private SpriteRenderer planeSR;
    public void UpdateGrid()
    {
        Vector2 planeObjSize = PlaneObject.transform.localScale;
		if (!mySR)
		{ mySR = GetComponent<SpriteRenderer>(); }
        transform.localScale = new Vector2(1 / planeObjSize.x * defaultSize.x, 1 / planeObjSize.y * defaultSize.y);
        mySR.size = new Vector2 (planeObjSize.x / defaultSize.x, planeObjSize.y / defaultSize.y);
    }

	// Start is called before the first frame update
	void Start()
    {
		if (PlaneObject == null)
		{
			PlaneObject = transform.parent.gameObject;
		}
		if (!planeSR)
		{
			planeSR = PlaneObject.GetComponent<SpriteRenderer>();
        }

        mySR = GetComponent<SpriteRenderer>();

        mySR.sortingOrder = planeSR.sortingOrder + 1;
	}

  
}
