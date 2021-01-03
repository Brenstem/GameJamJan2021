using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public Vector2 _lastCheckPoint;

    void Awake()
    {
		if (_instance == null)
		{
            _instance = this;
            DontDestroyOnLoad(_instance);
		}
		else
            Destroy(gameObject);
    }
}
