using System;
using UnityEngine;

namespace Character
{
    public class BombController : MonoBehaviour
    {
        [SerializeField] private GameObject bombPrefab;
        [SerializeField] private float countdown;
        [SerializeField] private Collider collider;
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
            var bomb = Instantiate(bombPrefab, position, Quaternion.identity);
            Physics.IgnoreCollision(bomb.GetComponent<Collider>(), collider);
            
            Invoke(nameof(ResetSpawn), countdown);
        }

        private void ResetSpawn() => _canSpawn = true;

        
    }
}