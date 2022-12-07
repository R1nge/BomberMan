using UnityEngine;

public class HowToPlayUI : MonoBehaviour
{
    [SerializeField] private GameObject howTo, button;

    private void Awake() => Close();

    public void Open()
    {
        button.SetActive(false);
        howTo.SetActive(true);
    }

    public void Close()
    {
        button.SetActive(true);
        howTo.SetActive(false);
    }
}