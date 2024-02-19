using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bullet_RainHolder : MonoBehaviour
{
    public GameObject bullet_Rain_Prefab;
    public SpriteRenderer targetPosSpriteRenderer;

    public void Fire(float time, float distance)
    {
        //SoundManager.Instance.PlayTempSound("Boss_Demo_Atk_Alarm", this.transform.position, 0.2f, 0.8f, 0.9f);
        targetPosSpriteRenderer.color = Color.clear;
        targetPosSpriteRenderer.DOColor(Color.red, 0.15f).SetLoops(3);

        GameObject bulletGo = Instantiate(bullet_Rain_Prefab, this.transform.position + new Vector3(0f, distance, 0f), Quaternion.identity);
        bulletGo.GetComponent<Bullet_Rain>().Fire(targetPosSpriteRenderer.transform.position, time);
        StartCoroutine(FireCoro(time + 2f));
    }

    IEnumerator FireCoro(float time)
    {
        yield return new WaitForSeconds(time);
        targetPosSpriteRenderer.DOColor(Color.clear, 0.2f);
        yield return new WaitForSeconds(0.25f);
        Destroy(this.gameObject); // Change Pooling Later
    }
}
