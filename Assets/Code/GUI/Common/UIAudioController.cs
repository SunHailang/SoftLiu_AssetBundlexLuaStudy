using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAudioController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_sliderBGAudioProgress = null;
    [SerializeField]
    private Slider m_sliderBGAudio = null;
    [SerializeField]
    private TextMeshProUGUI m_sliderEffectAudioProgress = null;
    [SerializeField]
    private Slider m_sliderEffectAudio = null;

    private void Start()
    {
        m_sliderBGAudio.value = AudioController.Instance.audioBGVolume;
        m_sliderEffectAudio.value = AudioController.Instance.audioEffectVolume;
        SetSliderProgress(bg: m_sliderBGAudio.value, effect: m_sliderEffectAudio.value);
    }

    public void SliderBGAudio_OnChanged()
    {
        AudioController.Instance.SetAudioBGVolume(m_sliderBGAudio.value);
        SetSliderProgress(bg: m_sliderBGAudio.value);
    }

    public void SliderEffectAudio_OnChanged()
    {
        AudioController.Instance.SetAudioEffectsVolume(m_sliderEffectAudio.value);
        SetSliderProgress(effect: m_sliderEffectAudio.value);
    }

    private void SetSliderProgress(float bg = -1, float effect = -1)
    {
        if (bg >= 0)
            m_sliderBGAudioProgress.text = string.Format("{0}%", (bg * 100).ToString("F"));
        if (effect >= 0)
            m_sliderEffectAudioProgress.text = string.Format("{0}%", (effect * 100).ToString("F"));
    }
}
