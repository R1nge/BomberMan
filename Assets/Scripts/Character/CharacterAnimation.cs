using UnityEngine;

namespace Character
{
    public class CharacterAnimation : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float height;
        private float _startPosY;

        private void Awake() => _startPosY = transform.position.y;

        private void Update()
        {
            var pos = transform.localPosition;
            var newY = Mathf.Clamp(Mathf.Sin(Time.time * speed) * height, _startPosY, height);
            pos.y = newY;
            transform.localPosition = pos;
        }
    }
}