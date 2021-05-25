using System.Collections.Generic;
using System.Linq;
using BOYAREngine.Controller;
using Cinemachine;
using UnityEngine;

namespace BOYAREngine.Utils
{
    [RequireComponent(typeof(Camera))]
    public class CameraTargetGroup : MonoBehaviour
    {
        public static CameraTargetGroup Instance;

        public List<GameObject> Targets;

        [Header("Camera ZoomIn/Out settings")]
        [SerializeField] private Vector3 _offset;
        [SerializeField] private Vector3 _velocity;
        [SerializeField] private float _smoothTime;
        [SerializeField] private float _minZoom;
        [SerializeField] private float _maxZoom;
        [SerializeField] private float _zoomLimiter;

        private Camera _camera;

        private void Awake()
        {
            Instance = this;

            _camera = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            if (Targets.Count == 0) return;

            Move();
            Zoom();
        }

        private void Move()
        {
            var centerPoint = GetCenterPoint();
            var newPosition = centerPoint + _offset;
            transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref _velocity, _smoothTime);
        }

        private void Zoom()
        {
            var newZoom = Mathf.Lerp(_maxZoom, _minZoom, GetGreatestDistance() / _zoomLimiter);
            _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, newZoom, Time.deltaTime);
        }

        private Vector3 GetCenterPoint()
        {
            if (Targets.Count == 1)
            {
                return Targets[0].transform.position;
            }

            var bounds = new Bounds(Targets[0].transform.position, Vector3.zero);
            foreach (var t in Targets)
            {
                bounds.Encapsulate(t.transform.position);
            }

            return bounds.center;
        }

        private float GetGreatestDistance()
        {
            var bounds = new Bounds(Targets[0].transform.position, Vector3.zero);
            foreach (var t in Targets)
            {
                bounds.Encapsulate(t.transform.position);
            }

            return (bounds.size.x + bounds.size.y) * .5f;
        }
    }
}

