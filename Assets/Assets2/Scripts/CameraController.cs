using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] GameObject player;
    [Range(0, 0.5f)] [SerializeField] float smoothTime = 0.3f;
    [SerializeField] float dampeningRange;


    Vector3 offset;
    Vector3 zeroVector = Vector3.zero;

    private void Awake()
    {
        offset = transform.position - player.transform.position;
    }

    void Update()
    {
        //transform.position = player.transform.position + offset;

        Vector3 targetPosition = player.transform.position + offset;
        
        transform.position = targetPosition;

        //if (Vector3.Distance(targetPosition, transform.position) > dampeningRange)
        //{
        //    targetPosition = targetPosition + -targetPosition.normalized * dampeningRange;
        //    transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref zeroVector, smoothTime);
        //}
        //if (Vector3.Distance(targetPosition, transform.position) > dampeningRange)
        //{
        //    targetPosition = targetPosition + -targetPosition.normalized * dampeningRange;
        //    transform.position = targetPosition;
        //}

        //transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref zeroVector, smoothTime);
    }
}
