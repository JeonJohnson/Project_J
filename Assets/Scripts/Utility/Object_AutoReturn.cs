using UnityEngine;
using System.Collections;

public class Object_AutoReturn : MonoBehaviour
{
	public float returnTime = 1f;
	void OnEnable()
	{
		StartCoroutine("CheckIfAlive");

    }
	
	IEnumerator CheckIfAlive ()
	{
		while(true)
		{
			yield return new WaitForSeconds(returnTime);
            //SubManager<PoolingManager>.Get().ReturnObj(this.gameObject);
		    break;
		}
	}
}
