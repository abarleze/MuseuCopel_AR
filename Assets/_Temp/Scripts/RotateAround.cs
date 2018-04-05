using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAround : MonoBehaviour
{
	public Transform center;
	
	void Update () 
	{
		transform.RotateAround(center.position, Vector3.up, 180.0f * Time.deltaTime);
		
	}
}
