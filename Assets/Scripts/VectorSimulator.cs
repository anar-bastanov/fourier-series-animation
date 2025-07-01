using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace Local
{
    public sealed class VectorSimulator : MonoBehaviour
    {
        private const int MaxArrowCount = 1023;

        private const double Period = 2 * Math.PI;

        private double _localRotationTimer = 0.0;

        private double _localMorphingTimer = 0.0;

        private bool _isMorphing = false;

        private List<Complex> _coefficients;

        private List<Complex> _morphingCoefficients;

        private List<ArrowObject> _arrows = new();

        [field: SerializeField]
        public int ArrowCount { get; private set; } = 64;

        [SerializeField]
        private GameObject _arrowPrefab;

        [SerializeField]
        private Transform _arrowsOrigin;

        [field: SerializeField]
        public float RotationSpeed { get; private set; } = 1.0f;

        [field: SerializeField]
        public double MorphingSpeed { get; private set; } = 3.0;

        [field: SerializeField]
        public float MaxLength { get; set; } = 4.0f;

        [field: SerializeField]
        public float MaxWidth { get; set; } = 0.2f;

        [field: SerializeField]
        public float MaxSize { get; set; } = 0.7f;

        [SerializeField]
        private TrailRenderer _trailDrawing;

        [SerializeField]
        private Material _trailMaterial;

        [field: SerializeField]
        public float Thickness { get; set; } = 0.1f;

        [field: SerializeField]
        public float TrailWidth { get; private set; } = 0.05f;

        [field: SerializeField]
        public float ActiveTrailTime { get; private set; } = 1.0f;

        private TextAsset _samplesFile;

        [field: SerializeField]
        public TextAsset SamplesFile { get; private set; }

        [SerializeField]
        private FocusPoint _focusPoint;

        private float CameraZoom => MathF.Min(_focusPoint.Zoom / _focusPoint.SceneScale, 1.0f);

        private void Awake()
        {
            _arrows = new(MaxArrowCount);
            _coefficients = new(MaxArrowCount);
            _morphingCoefficients = new(MaxArrowCount);

            for (int i = 0; i < MaxArrowCount; ++i)
            {
                var arrow = Instantiate(_arrowPrefab, _arrowsOrigin);
                arrow.SetActive(false);

                _arrows.Add(arrow.GetComponent<ArrowObject>());
                _coefficients.Add(new());
                _morphingCoefficients.Add(new());
            }
        }

        private void Update()
        {
            if (SamplesFile != _samplesFile)
            {
                _samplesFile = SamplesFile;
                StartMorphing();
                return;
            }

            HideUnusedArrows();

            if (_isMorphing)
            {
                _localMorphingTimer += Time.deltaTime * MorphingSpeed;
                MorphCoefficients();
            }
            else
            {
                _localRotationTimer += Time.deltaTime * RotationSpeed * Math.Pow(CameraZoom, 0.75f);
            }

            RotateArrows();
        }

        private void RotateArrows()
        {
            double t = _localRotationTimer;
            var pos = new Vector2();

            for (int i = 1; i <= ArrowCount; ++i)
            {
                int n = i % 2 == 0 ? i / 2 : -i / 2;
                double cos = Math.Cos(Period * n * t);
                double sin = Math.Sin(Period * n * t);
                var c = _coefficients[i - 1] * new Complex(cos, sin);
                c *= _focusPoint.SceneScale;

                var arrow = c.ToVector();
                _arrows[i - 1].Stretch(pos, arrow,
                    MaxLength * CameraZoom,
                    MaxWidth * CameraZoom,
                    MaxSize * CameraZoom,
                    Thickness * CameraZoom);
                pos += arrow;
            }

            DrawTrail(pos);
        }

        private void StartMorphing()
        {
            double rotTimer = _localRotationTimer % Period;
            _isMorphing = true;
            _localMorphingTimer = 0.0;
            _localRotationTimer = 0.0;
            _trailDrawing.emitting = false;

            var samples = FourierSeries.LoadSamples(SamplesFile);
            _morphingCoefficients.Clear();
            _morphingCoefficients.AddRange(FourierSeries.ComputeCoefficients(samples, MaxArrowCount));

            for (int i = 0; i < MaxArrowCount; ++i)
            {
                int n = i % 2 == 1 ? (i + 1) / 2 : -(i + 1) / 2;
                double cos = Math.Cos(Period * n * rotTimer);
                double sin = Math.Sin(Period * n * rotTimer);
                _coefficients[i] *= new Complex(cos, sin);

                double magnitude = _morphingCoefficients[i].Magnitude - _coefficients[i].Magnitude;
                double phase = _morphingCoefficients[i].Phase - _coefficients[i].Phase;
                phase = phase <= +Math.PI ? phase : phase - Period;
                phase = phase >= -Math.PI ? phase : phase + Period;

                _morphingCoefficients[i] = new(magnitude, phase);
            }
        }

        private void MorphCoefficients()
        {
            _localMorphingTimer = Math.Min(_localMorphingTimer, 1.0);

            double t = Math.Pow(_localMorphingTimer, 0.66);
            double ddt = Mathf.PingPong((float)(_localMorphingTimer * 4.0), 2.0f);
            double dt = Time.deltaTime * MorphingSpeed * ddt;

            for (int i = 0; i < MaxArrowCount; ++i)
            {
                double magnitude = _coefficients[i].Magnitude + _morphingCoefficients[i].Real * dt;
                double phase = _coefficients[i].Phase + _morphingCoefficients[i].Imaginary * dt;
                _coefficients[i] = Complex.FromPolarCoordinates(magnitude, phase);
            }

            _trailMaterial.SetFloat("_GlobalAlpha", 1.0f - (float)t);

            if (_localMorphingTimer == 1.0)
            {
                _isMorphing = false;
                _trailDrawing.Clear();
                _trailDrawing.emitting = true;
                _trailMaterial.SetFloat("_GlobalAlpha", 1.0f);
            }
        }

        private void DrawTrail(Vector2 pos)
        {
            float width = TrailWidth * MathF.Pow(CameraZoom, 0.5f);
            float z = _trailDrawing.transform.localPosition.z;

            _trailDrawing.transform.localPosition = new(pos.x, pos.y, z);
            _trailDrawing.time = ActiveTrailTime / RotationSpeed; // / Math.Pow(CameraZoom, 0.005f);
            _trailDrawing.startWidth = width;
            _trailDrawing.endWidth = width;
        }

        private void HideUnusedArrows()
        {
            for (int i = 0; i < MaxArrowCount; ++i)
                _arrows[i].gameObject.SetActive(i < ArrowCount && _coefficients[i].Magnitude > 0.00001f);
        }
    }
}
