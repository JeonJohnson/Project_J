using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CObj : MonoBehaviour
{

	public abstract void Hit(int dmg, Vector2 dir);
	
}
