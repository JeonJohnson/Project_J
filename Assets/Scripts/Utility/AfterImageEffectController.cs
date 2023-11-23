using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AfterImageEffectController : MonoBehaviour
{
    public Material afterImageMaterial ; // ��Unity�༭���У�����Ҫ�������Ĳ�ӰShader��ֵ�����Material
    public float fadeDuration = 0.1f; // ��Ӱ��ʧ��ʱ�䣬��λΪ��
    public float fixedAfterImageDelay = 0.07f; // ������Ӱ��ʱ��������λΪ��
    public bool enable = true;
    public float afterImageDelayDistance;
    private Vector3 lastPos;

    private float afterImageDelay; // ��ǰ��Ӱ��ʱ��������λΪ��
    private SpriteRenderer spriteRenderer; // �洢����ͼ����Ⱦ��
    private List<SpriteRenderer> afterImages = new List<SpriteRenderer>(); // ���ڴ洢���еĲ�Ӱ
    private Vector3 lastGlobalPosition;  // ���ڼ�¼����ͼ��һ֡��ȫ��λ��

    void Start() // �ھ���ͼ����ʱִ��
    {
        afterImageDelay = fixedAfterImageDelay;

        spriteRenderer = GetComponent<SpriteRenderer>(); // ��ȡ����ͼ����Ⱦ��
        lastGlobalPosition = transform.position; // ��¼����ͼ�ĳ�ʼȫ��λ��
        lastPos = this.transform.position;
    }

    void Update() // ��ÿһ֡��ִ��
    {

        // �������в�Ӱ�ĵ���������
        for (int i = afterImages.Count - 1; i >= 0; i--)
        {
            SpriteRenderer afterImage = afterImages[i];

            // �𽥸ı��Ӱ��͸����
            Color color = afterImage.color;
            color.a -= Time.deltaTime / fadeDuration;
            afterImage.color = color;

            // ����Ӱ��ȫ͸����ɾ����
            if (color.a <= 0)
            {
                afterImages.RemoveAt(i); // ���б����Ƴ�
                Destroy(afterImage.gameObject); // ���ٲ�Ӱ����
            }
        }

        if (!enable) return;

        // �������ͼ��ȫ��λ���б仯��Ҳ����˵����ͼ���ƶ�
        if (Vector3.Distance(lastGlobalPosition, transform.position) > 0)
        {
            //if (afterImageDelay > 0)
            //{
            //    afterImageDelay -= Time.deltaTime; 
            //    if (afterImageDelay <= 0) 
            //    {
            //        CreateAfterImage(); 
            //        afterImageDelay = fixedAfterImageDelay;
            //    }
            //}

            if (Vector3.Distance(lastPos, transform.position) > afterImageDelayDistance)
            {
                lastPos = this.transform.position;
                CreateAfterImage();
            }
        }

        lastGlobalPosition = transform.position; // ���¾���ͼ��ȫ��λ��
    }

    // ����һ���µĲ�Ӱ
    void CreateAfterImage()
    {

        GameObject afterImageObj = new GameObject("AfterImage"); // ����һ���µ���Ϸ�������ڴ洢��Ӱ
        SpriteRenderer afterImageRenderer = afterImageObj.AddComponent<SpriteRenderer>(); // ���µ���Ϸ���������Ⱦ�����

        // ����Ӱ����Ϊ��ǰ����ͼ�ĸ���
        afterImageRenderer.sprite = spriteRenderer.sprite; // ���ò�Ӱ��ͼ��
        afterImageRenderer.material = afterImageMaterial; // ���ò�Ӱ�Ĳ���
        afterImageRenderer.color = spriteRenderer.color; // ���ò�Ӱ����ɫ
        afterImageObj.transform.position = transform.position; // ���ò�Ӱ��λ��
        afterImageObj.transform.rotation = transform.rotation; // ���ò�Ӱ����ת
        afterImageObj.transform.localScale = transform.localScale; // ���ò�Ӱ�Ĵ�С

        //������Ⱦ˳��ʹ����ԭʼ����֮��
        afterImageRenderer.sortingLayerID = spriteRenderer.sortingLayerID; // ������Ⱦ��
        afterImageRenderer.sortingOrder = spriteRenderer.sortingOrder - 1; // ������Ⱦ˳��

        // ���µĲ�Ӱ��ӵ��б���
        afterImages.Add(afterImageRenderer);
    }
}
