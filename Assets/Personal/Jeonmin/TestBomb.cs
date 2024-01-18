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

        Destroy(this.gameObject);
    }

    Tilemap FindTilemapAtPosition(Vector3 position, string targetObjectName)
    {
        // ��ź ��ġ �ֺ��� �ִ� Tilemap ã��
        Collider2D[] collider = Physics2D.OverlapCircleAll(position, bombRadius); // Collider2D�� ����Ͽ� �ֺ��� �ִ� ������Ʈ ã�� (������ ������ ����)
        for(int i = 0; i < collider.Length; i++)
        {
            if (collider[i] != null && collider[i].gameObject.name == targetObjectName)
            {
                // ã�� ������Ʈ���� Tilemap ������Ʈ ã��
                Tilemap tilemap = collider[i].GetComponent<Tilemap>();
                return tilemap;
            }
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

    float timer = 2f;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Explode(this.transform.position);
        }

        if(isTrigged)
        {
            timer -= Time.deltaTime;
            if(timer <= 0f)
            {
                timer = 2f;
                Explode(this.transform.position);
            }
        }
    }

    private bool isTrigged = false;

    private Holdable holdable;
    private void Awake()
    {
        holdable = GetComponent<Holdable>();
        if (holdable) holdable.OnFired += () => { OnFiredEvent(); };
    }

    public void OnFiredEvent()
    {
        isTrigged = true;
    }


    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isTrigged) return;
        if(collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Explode(this.transform.position);
            Destroy(this.gameObject);
        }
    }
}
