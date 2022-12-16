using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class VfxPool : MonoBehaviour
{
    [SerializeField] private GameObject vfx;
    [SerializeField] private int amountToPool;
    private List<GameObject> _vfxList;

    private static VfxPool _instance;

    public static VfxPool Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<VfxPool>();

                if (_instance == null)
                {
                    var go = new GameObject
                    {
                        name = "VfxPool"
                    };

                    _instance = go.AddComponent<VfxPool>();
                }
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        _vfxList = new List<GameObject>();
        _vfxList.Clear();

        for (int i = 0; i < amountToPool; i++)
        {
            var go = Instantiate(vfx);
            go.gameObject.SetActive(false);
            _vfxList.Add(go);
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!_vfxList[i].gameObject.activeInHierarchy)
            {
                _vfxList[i].SetActive(true);
                return _vfxList[i];
            }
        }

        return null;
    }
}