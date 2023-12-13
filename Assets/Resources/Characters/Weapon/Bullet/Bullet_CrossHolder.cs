using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_CrossHolder : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;
    public float destroyTime;
    private float timer;
    private Bullet_Normal[] bullets;
    [SerializeField] Transform pivotTr;

    private int rotateClockDir = -1;

    private void Awake()
    {
        timer = destroyTime;
        int rnd = Random.Range(0, 100);
        if (rnd <= 50) rotateClockDir = 1;
    }

    private void Start()
    {
        bullets = pivotTr.GetComponentsInChildren<Bullet_Normal>();
        for (int i = 0; i < bullets.Length; i++)
        {
            bullets[i].Fire(bullets[i].transform.right, 0, bullets[i].defaultStat.moveSpd, 0.5f);
        }
    }

    void Update()
    {
        this.transform.position += Time.deltaTime * this.transform.right * speed;

        timer -= Time.deltaTime;
        if (timer <= 0f) Destroy(this.gameObject);


        pivotTr.transform.Rotate(0f, 0f, rotateClockDir * rotationSpeed * Time.deltaTime);
    }
}
