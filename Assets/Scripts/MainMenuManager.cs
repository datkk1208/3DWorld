using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Cần cho Slider
using TMPro; // Cần cho TextMeshPro

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject LoadingPanel;
    public GameObject SettingPanel;

    [Header("Loading UI")]
    public Slider LoadingSlider;
    public TextMeshProUGUI ProgressText;

    [Header("Scene Config")]
    public string GameSceneName = "Tên_Scene_Game_Của_Bạn";

    private void Start()
    {
        if (LoadingPanel != null) LoadingPanel.SetActive(false);
        if (SettingPanel != null) SettingPanel.SetActive(false);
    }

    public void PlayGame()
    {
        StartCoroutine(LoadSceneAsync());
    }

    public void ToggleSetting()
    {
        if (SettingPanel != null) SettingPanel.SetActive(!SettingPanel.activeSelf);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // --- COROUTINE LOADING CÓ HIỆU ỨNG MƯỢT ---
    private IEnumerator LoadSceneAsync()
    {
        if (LoadingPanel != null) LoadingPanel.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(GameSceneName);

        // Chặn không cho tự động chuyển Scene ngay lập tức để chờ UI chạy hết 100%
        operation.allowSceneActivation = false;

        float targetProgress = 0f;
        float currentProgress = 0f;

        while (!operation.isDone)
        {
            // Unity tải tối đa đến 0.9 là dừng, ta chia 0.9 để lấy mốc chuẩn 1.0 (100%)
            targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

            // Hiệu ứng: Thanh UI chạy từ từ đuổi theo tiến trình thật
            currentProgress = Mathf.MoveTowards(currentProgress, targetProgress, 1.5f * Time.deltaTime);

            // Cập nhật lên UI
            if (LoadingSlider != null) LoadingSlider.value = currentProgress;
            if (ProgressText != null) ProgressText.text = (currentProgress * 100f).ToString("F0") + "%";

            // Chỉ cho phép sang Scene mới khi thanh UI đã chạy đủ 100%
            if (currentProgress >= 1f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}