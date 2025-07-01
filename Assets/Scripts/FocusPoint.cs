using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Local
{
    public sealed class FocusPoint : MonoBehaviour
    {
        private Vector3 _position;

        [SerializeField]
        private Camera _camera;

        [field: SerializeField]
        public Transform Focus { get; private set; }

        [field: SerializeField]
        public float SceneScale { get; private set; } = 1.0f;

        [field: SerializeField]
        public float Zoom { get; private set; } = 1.0f;

        [field: SerializeField]
        public Vector2 Offset { get; private set; }

        private void Awake()
        {
            _camera.transform.position = _position = new(Offset.x, Offset.y, _camera.transform.position.z);
        }

        private void Update()
        {
            float cameraSize = MathF.Max(Zoom, 0.001f);
            _camera.orthographicSize = cameraSize;

            Vector3 point = new(Offset.x, Offset.y, _camera.transform.position.z);
            float omega = 0.2f;
            float rate = 1.0f;

            if (Focus != null)
            {
                //float omega = MathF.Cos(MathF.PI / 2.0f * MathF.Min(Zoom / SceneScale, 1.0f));
                omega = MathF.Pow(MathF.Min(Zoom / (SceneScale * 1.3f), 1.0f), 2.0f);
                point.x = Focus.position.x + (Offset.x - Focus.position.x) * omega;
                point.y = Focus.position.y + (Offset.y - Focus.position.y) * omega;
                rate = MathF.Pow((_camera.transform.position - point).magnitude / cameraSize, 1.2f);
            }

            _camera.transform.position = Vector3.Lerp(_camera.transform.position, point, Time.deltaTime * rate / omega);
        }
    }
}
