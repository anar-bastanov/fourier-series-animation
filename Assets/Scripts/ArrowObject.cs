using System;
using UnityEngine;

namespace Local
{
    public sealed class ArrowObject : MonoBehaviour
    {
        [SerializeField]
        private Transform _line;

        [SerializeField]
        private Transform _tip;

        [SerializeField]
        private Ring _ring;

        public void Stretch(Vector2 origin, Vector2 direction,
            float maxLength, float maxWidth, float maxSize, float thickness)
        {
            float angle = MathF.Atan2(direction.y, direction.x) * 180.0f / MathF.PI;
            float length = direction.magnitude;
            float scale = MathF.Min(length / maxLength, 1.0f);
            float width = maxWidth * scale;
            float size = maxSize * scale;
            float ring = (length + thickness) * 2.0f;

            transform.SetLocalPositionAndRotation(origin, Quaternion.Euler(0.0f, 0.0f, angle));

            _line.localPosition = new Vector2((length - size) * 0.5f, 0.0f);
            _tip.localPosition = new Vector2(length - size * 0.5f, 0.0f);

            _line.localScale = new Vector3(length - size, width, _line.localScale.z);
            _tip.localScale = new Vector3(size, size, _tip.localScale.z);
            _ring.transform.localScale = new(ring, ring, _ring.transform.localScale.z);

            _ring.Thickness = thickness / length;
            _ring.Radius = 1.0f - _ring.Thickness;
        }
    }
}
