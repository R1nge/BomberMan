using UnityEngine;

namespace Character
{
    public class BombController : MonoBehaviour
    {
        [SerializeField] private GameObject bombPrefab;
        [SerializeField] private float countdown;
        private bool _canSpawn = true;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && _canSpawn)
            {
                Spawn(transform.position);
            }
        }

        private void Spawn(Vector3 position)
        {
            _canSpawn = false;
            Instantiate(bombPrefab, position, Quaternion.identity);
            Invoke(nameof(ResetSpawn), countdown);
        }

        private void ResetSpawn() => _canSpawn = true;
    }
}