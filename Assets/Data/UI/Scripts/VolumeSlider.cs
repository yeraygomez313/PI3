using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI volumeText;
    [SerializeField] private Slider slider;
    [SerializeField] private AudioMixerGroup audioMixerGroup;
    [SerializeField] private string playerPrefsKey = "Volume";

    private void Awake()
    {
        if (slider == null)
        {
            Debug.LogError("VolumeSlider is not assigned in the inspector.");
            return;
        }

        float savedVolume = PlayerPrefs.HasKey(playerPrefsKey) ? PlayerPrefs.GetFloat(playerPrefsKey) : 1f;
        slider.value = savedVolume;
        SetVolume(savedVolume);

        slider.onValueChanged.AddListener(SetVolume);
    }

    private void SetVolume(float value)
    {
        volumeText.text = $"{playerPrefsKey}: {(int)(value * 100)}%";

        if (audioMixerGroup != null)
        {
            float finalValue = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
            audioMixerGroup.audioMixer.SetFloat("Volume", finalValue);
        }
        else
        {
            Debug.LogError("AudioMixerGroup is not assigned in the inspector.");
            return;
        }

        // Save volume
        PlayerPrefs.SetFloat(playerPrefsKey, value);
        PlayerPrefs.Save();
    }
}
