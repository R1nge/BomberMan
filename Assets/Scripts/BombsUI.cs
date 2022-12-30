using UnityEngine;

public class BombsUI : MonoBehaviour
{
    [SerializeField] private BombSlot prefab;
    [SerializeField] private Transform parent;
    [SerializeField] private GameObject child;
    [SerializeField] private GameObject open, close;
    private BombSoundPreview _soundPreview;
    private PlayerBombs _bombs;

    private void Awake()
    {
        _soundPreview = FindObjectOfType<BombSoundPreview>();
        _bombs = FindObjectOfType<PlayerBombs>();
        Close();
    }

    private void Start()
    {
        for (int i = 0; i < _bombs.GetBombsCount(); i++)
        {
            Generate(i);
        }
    }

    private void Generate(int index)
    {
        var inst = Instantiate(prefab.gameObject, parent);
        var slot = inst.GetComponent<BombSlot>();
        slot.SetIcon(_bombs.GetSprite(index));
        slot.SetCallback(_bombs, _soundPreview, index);
    }

    public void Open()
    {
        child.SetActive(true);
        open.SetActive(false);
        close.SetActive(true);
    }

    public void Close()
    {
        child.SetActive(false);
        open.SetActive(true);
        close.SetActive(false);
    }
}