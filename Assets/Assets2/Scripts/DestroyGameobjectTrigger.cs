using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyGameobjectTrigger : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.gameObject != null)
		{
			if (other.transform.gameObject.CompareTag("Player"))
				other.GetComponentInParent<Health>().Damage(1000);
			else
				Destroy(other.transform.gameObject);
		}
		else
			Destroy(other);
	}
}