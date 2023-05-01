using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public Transform target;

	void Update()
	{
		Vector3 camara = new Vector3(target.position.x, transform.position.y, transform.position.z);
		transform.position = camara;
	}
}
