using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Structs;

public abstract class CObj : MonoBehaviour
{

	public abstract HitInfo Hit(int dmg, Vector2 dir);
	
}
