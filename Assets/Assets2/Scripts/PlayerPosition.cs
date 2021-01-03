using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPosition : MonoBehaviour
{
    private GameManager _gm;
    //private bool _dead;

    void Start()
    {
        _gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        transform.position = _gm._lastCheckPoint;
        //_dead = gameObject.GetComponent<Health>().isDead;
    }

    void Update()
    {
		if (gameObject.GetComponent<Health>().isDead)
		{
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
    }
}