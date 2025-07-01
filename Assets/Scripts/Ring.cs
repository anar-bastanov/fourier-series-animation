using UnityEngine;

namespace Local
{
    public class Ring : MonoBehaviour
    {
        private SpriteRenderer _renderer;

        private MaterialPropertyBlock _block;

        [field: SerializeField]
        public float Radius { get; set; } = 1.0f;

        [field: SerializeField]
        public float Thickness { get; set; } = 0.05f;

        [field: SerializeField]
        public Color Color { get; set; } = Color.white;

        private void Awake()
        {
            _block = new MaterialPropertyBlock();
            _renderer = GetComponent<SpriteRenderer>();
        }

        public void Update()
        {
            _renderer.GetPropertyBlock(_block);
            _block.SetFloat("_Radius", Radius);
            _block.SetFloat("_Thickness", Thickness);
            _block.SetColor("_Color", Color);
            _renderer.SetPropertyBlock(_block);
        }
    }
}
