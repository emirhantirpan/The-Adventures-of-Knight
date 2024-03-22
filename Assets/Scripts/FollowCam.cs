using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    private Vector3 _offset = new Vector3(0f,0f,-5f);
    private float _smoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;
    private float _zoom;
    private float _zoomMultipler = 4f;
    private float _minZoom = 2f;
    private float _maxZoom = 8f;
    private float _velocity = 0f;

    [SerializeField] private Transform _target;
    [SerializeField] private Camera _cam;

    private void Start()
    {
        _zoom = _cam.orthographicSize;
    }
    void Update()
    {
        Vector3 targetPosition = _target.position + _offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, _smoothTime); 

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        _zoom -= scroll * _zoomMultipler;
        _zoom = Mathf.Clamp(_zoom, _minZoom, _maxZoom);
        _cam.orthographicSize = Mathf.SmoothDamp(_cam.orthographicSize, _zoom, ref _velocity, _smoothTime);
    }
}
