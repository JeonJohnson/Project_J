using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Boss_Curve : Bullet
{
    public Transform target; // ���� ����
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
            // �Ѿ� ������ ����ϰ� �ʱ� �ӵ��� ����
            Vector2 direction = (target.position - transform.position).normalized;

            float rotateAmount = Vector3.Cross(direction, transform.up).z;
            // �Ѿ��� ������ �����ϸ鼭 ȸ��
            rb.angularVelocity = rotationSpeed * -rotateAmount;
            rb.velocity = transform.up * speed;
        }
    }
}
