using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Boss_Curve : Bullet
{
    public Transform target; // 도착 지점
    public float speed = 5f;
    public float rotationSpeed = 50f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (target != null)
        {
            // 총알 방향을 계산하고 초기 속도를 설정
            Vector2 direction = (target.position - transform.position).normalized;

            float rotateAmount = Vector3.Cross(direction, transform.up).z;
            // 총알의 방향을 유지하면서 회전
            rb.angularVelocity = rotationSpeed * -rotateAmount;
            rb.velocity = transform.up * speed;
        }
    }
}
