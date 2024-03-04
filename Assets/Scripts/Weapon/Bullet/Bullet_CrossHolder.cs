using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_CrossHolder : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;
    public float destroyTime;
    private float timer;
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

        GameObject go = PoolingManager.Instance.LentalObj("Bullet_Enemy", 1);
        go.SetActive(false);
        go.transform.position = this.transform.position;
        go.SetActive(true);
        go.transform.parent = pivotTr;
        Bullet_Normal bullet = go.GetComponent<Bullet_Normal>();
        bullet.Fire(bullet.transform.right, 0, 0f, 0.5f);

        float angle = 0;
        for (int i = 0; i < 4; i++)
        {
            float speed = 10;
            for (int j = 0; j < 3; j++)
            {
                GameObject bulletGo = PoolingManager.Instance.LentalObj("Bullet_Enemy", 1);
                bulletGo.SetActive(false);
                bulletGo.transform.position = this.transform.position;
                bulletGo.SetActive(true);
                bulletGo.transform.parent = pivotTr;
                bulletGo.transform.localEulerAngles = new Vector3(0f, 0f, angle);
                Bullet_Normal bullet1 = bulletGo.GetComponent<Bullet_Normal>();
                bullet1.Fire(bullet1.transform.right, 0, speed, 0.5f);
                speed += 10;
                Debug.Log(speed);
            }
            angle += 90;
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
