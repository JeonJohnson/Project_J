using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestBomb : MonoBehaviour
{
    public TileBase bombTile; // 폭탄을 나타내는 타일
    public int bombRadius = 10; // 폭탄의 범위

    void Explode(Vector3 bombPosition)
    {
        // 폭발 반경에 있는 타일 제거
        Tilemap tilemap = FindTilemapAtPosition(bombPosition, "Wall");
        if (tilemap != null)
        {
            Vector3Int cellPosition = tilemap.WorldToCell(bombPosition);
            for (int x = -bombRadius; x <= bombRadius; x++)
            {
                for (int y = -bombRadius; y <= bombRadius; y++)
                {
                    Vector3Int currentCell = new Vector3Int(cellPosition.x + x, cellPosition.y + y, cellPosition.z);

                    // 폭발 범위 내의 타일 삭제
                    if (Vector3Int.Distance(cellPosition, currentCell) <= bombRadius)
                    {
                        tilemap.SetTile(currentCell, null);
                    }
                }
            }
        }
    }

    Tilemap FindTilemapAtPosition(Vector3 position, string targetObjectName)
    {
        // 폭탄 위치 주변에 있는 Tilemap 찾기
        Collider2D collider = Physics2D.OverlapCircle(position, 0.1f); // Collider2D를 사용하여 주변에 있는 오브젝트 찾기 (적절한 반지름 설정)
        if (collider != null && collider.gameObject.name == targetObjectName)
        {
            // 찾은 오브젝트에서 Tilemap 컴포넌트 찾기
            Tilemap tilemap = collider.GetComponent<Tilemap>();
            return tilemap;
        }

        return null;
    }

    Tilemap FindTilemapAtPosition(Vector3 position)
    {
        // 폭탄 위치 주변에 있는 Tilemap 찾기
        Collider2D collider = Physics2D.OverlapCircle(position, 0.1f); // Collider2D를 사용하여 주변에 있는 오브젝트 찾기 (적절한 반지름 설정)
        if (collider != null)
        {
            // 찾은 오브젝트에서 Tilemap 컴포넌트 찾기
            Tilemap tilemap = collider.GetComponent<Tilemap>();
            return tilemap;
        }

        return null;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Explode(this.transform.position);
        }
    }
}
