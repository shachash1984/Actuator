using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraFollow : MonoBehaviour {

    public Transform _target;
    public float minOffSet = 0.5f;
    private Vector3 prevPos;

    private void Awake()
    {
        if (!_target)
            _target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Start()
    {
        prevPos =  _target.position;
        transform.position = _target.position;
    }

    private void LateUpdate()
    {
        //transform.position = _target.position;
        if (Vector3.SqrMagnitude(prevPos - _target.position) > minOffSet)
        {
            transform.DOMove(_target.position, 1f).SetEase(Ease.OutCubic);
            prevPos = transform.position;
        }
            
    }
}
