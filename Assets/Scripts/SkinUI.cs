using UnityEngine;

public class SkinUI : MonoBehaviour
{
    [SerializeField] private SkinSlot prefab;
    [SerializeField] private Transform parent;
    [SerializeField] private GameObject child;
    [SerializeField] private GameObject open, close;
    private PlayerSkins _skins;

    private void Awake()
    {
        _skins = FindObjectOfType<PlayerSkins>();
        Close();
    }

    private void Start()
    {
        for (int i = 0; i < _skins.GetSkinsCount(); i++)
        {
            Generate(i);
        }
    }

    private void Generate(int index)
    {
        var inst = Instantiate(prefab.gameObject, parent);
        var slot = inst.GetComponent<SkinSlot>();
        slot.SetIcon(_skins.GetSprite(index));
        slot.SetCallback(_skins, index);
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