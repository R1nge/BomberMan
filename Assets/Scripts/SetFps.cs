using UnityEngine;

public class SetFps : MonoBehaviour
{
#if UNITY_ANDROID
    [SerializeField] private int targetFrameRate;

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 999999;
        //Application.targetFrameRate = targetFrameRate;
    }
#endif
}