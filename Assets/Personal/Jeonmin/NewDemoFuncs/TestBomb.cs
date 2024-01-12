using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestBomb : MonoBehaviour
{
    public TileBase bombTile; // ��ź�� ��Ÿ���� Ÿ��
    public int bombRadius = 10; // ��ź�� ����

    void Explode(Vector3 bombPosition)
    {
        // ���� �ݰ濡 �ִ� Ÿ�� ����
        Tilemap tilemap = FindTilemapAtPosition(bombPosition, "Wall");
        if (tilemap != null)
        {
            Vector3Int cellPosition = tilemap.WorldToCell(bombPosition);
            for (int x = -bombRadius; x <= bombRadius; x++)
            {
                for (int y = -bombRadius; y <= bombRadius; y++)
                {
                    Vector3Int currentCell = new Vector3Int(cellPosition.x + x, cellPosition.y + y, cellPosition.z);

                    // ���� ���� ���� Ÿ�� ����
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
        // ��ź ��ġ �ֺ��� �ִ� Tilemap ã��
        Collider2D collider = Physics2D.OverlapCircle(position, 0.1f); // Collider2D�� ����Ͽ� �ֺ��� �ִ� ������Ʈ ã�� (������ ������ ����)
        if (collider != null && collider.gameObject.name == targetObjectName)
        {
            // ã�� ������Ʈ���� Tilemap ������Ʈ ã��
            Tilemap tilemap = collider.GetComponent<Tilemap>();
            return tilemap;
        }

        return null;
    }

    Tilemap FindTilemapAtPosition(Vector3 position)
    {
        // ��ź ��ġ �ֺ��� �ִ� Tilemap ã��
        Collider2D collider = Physics2D.OverlapCircle(position, 0.1f); // Collider2D�� ����Ͽ� �ֺ��� �ִ� ������Ʈ ã�� (������ ������ ����)
        if (collider != null)
        {
            // ã�� ������Ʈ���� Tilemap ������Ʈ ã��
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
