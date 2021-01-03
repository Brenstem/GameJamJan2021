using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPosition : MonoBehaviour
{
    private GameManager _gm;

    void Start()
    {
        _gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        transform.position = _gm._lastCheckPoint;
    }

    void Update()
    {
		if (gameObject.GetComponent<Health>().isDead)
		{
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
    }
}