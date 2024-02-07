using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class Particle_AutoReturn : MonoBehaviour
{
	public bool OnlyDeactivate;
	
	void OnEnable()
	{
		StartCoroutine("CheckIfAlive");
        if(GetComponent<ParticleSystem>()) GetComponent<ParticleSystem>().Play();
    }
	
	IEnumerator CheckIfAlive ()
	{
		while(true)
		{
			yield return new WaitForSeconds(0.5f);
			if(!GetComponent<ParticleSystem>().IsAlive(true))
			{
				if (OnlyDeactivate)
				{
#if UNITY_3_5
						this.gameObject.SetActiveRecursively(false);
#else
					this.gameObject.SetActive(false);
#endif
				}
				else
					GetComponent<ParticleSystem>().Stop();
                    //SubManager<PoolingManager>.Get().ReturnObj(this.gameObject);
				break;
			}
		}
	}
}
