using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    private GameManager _gm;

    void Start()
    {
        _gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
    }

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
            _gm._lastCheckPoint = transform.position;
		else if (other.transform.parent != null)
			if (other.transform.parent.gameObject.CompareTag("Player"))
                _gm._lastCheckPoint = transform.position;
    }
}
