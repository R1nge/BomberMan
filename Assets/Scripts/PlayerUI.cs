using Character;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUI : NetworkBehaviour
{
    [SerializeField] private GameObject UI, mobileControls;
    [SerializeField] private TextMeshProUGUI hp, bombs;
    private PlayerInput _input;
    private Health _playerHealth;
    private CharacterBomb _characterBomb;

    private void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _playerHealth = GetComponent<Health>();
        _playerHealth.OnTakenDamage += UpdateHealth;
        _characterBomb = GetComponent<CharacterBomb>();
        _characterBomb.OnBombPlaced += UpdateBombs;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            UI.SetActive(false);
            mobileControls.SetActive(false);
            _input.enabled = false;
        }
#if !UNITY_ANDROID
       else
        {
            mobileControls.SetActive(false);       
        }
#endif
    }

    private void UpdateHealth(int current) => hp.text = current.ToString();

    private void UpdateBombs(int current, int max) => bombs.text = current + "/" + max;

    public override void OnDestroy()
    {
        base.OnDestroy();
        _playerHealth.OnTakenDamage += UpdateHealth;
        _characterBomb.OnBombPlaced -= UpdateBombs;
    }
}