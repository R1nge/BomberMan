using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUI : NetworkBehaviour
{
    [SerializeField] private GameObject UI, mobileControls;
    [SerializeField] private TextMeshProUGUI hp, bombs;
    private PlayerInput _input;

    private void Awake() => _input = GetComponent<PlayerInput>();

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

    public void UpdateHealth(int old, int current) => hp.text = current.ToString();

    public void UpdateBombs(int current, int max) => bombs.text = current + "/" + max;
}