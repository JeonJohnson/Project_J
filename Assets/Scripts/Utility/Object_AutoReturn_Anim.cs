using UnityEngine;
using System.Collections;

public class Object_AutoReturn_Anim : MonoBehaviour
{
	public float returnTime = 1f;
	[SerializeField] Animator animator;
	void OnEnable()
	{
		if (animator == null) animator = GetComponent<Animator>();
		animator.Play(0);
        returnTime = animator.GetCurrentAnimatorStateInfo(0).length;
        StartCoroutine("CheckIfAlive");
    }

    IEnumerator CheckIfAlive()
	{
		while(true)
		{
			yield return new WaitForSeconds(returnTime);
            PoolingManager.Instance.ReturnObj(this.gameObject);
		    break;
		}
	}
}
