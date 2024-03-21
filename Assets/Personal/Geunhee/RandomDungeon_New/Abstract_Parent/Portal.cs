using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Portal : MonoBehaviour
{
	public Collider2D col;
	public SpriteRenderer ssr;
	public Material mat;
	public UnityEngine.Rendering.Universal.Light2D light;

	public RoomType type;

	public abstract void Appear();
	public abstract void Disappear();


	protected abstract IEnumerable AppearProduceCor();
	protected abstract IEnumerable DisappearProduceCor();

}
