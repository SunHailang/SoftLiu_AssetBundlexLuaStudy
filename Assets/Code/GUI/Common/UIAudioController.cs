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
    [SerializeField]
    private GameObject m_AudioBG_Grid = null;
    [SerializeField]
    private Image m_imageAudio = null;

    [HideInInspector]
    public float[] progress = new float[64];

    List<Image> m_imageTrans = new List<Image>();


    public float height = 50;

    private void Start()
    {
        m_sliderBGAudio.value = AudioController.Instance.audioBGVolume;
        m_sliderEffectAudio.value = AudioController.Instance.audioEffectVolume;
        SetSliderProgress(bg: m_sliderBGAudio.value, effect: m_sliderEffectAudio.value);

        if (m_imageTrans.Count <= 0)
        {
            foreach (var item in progress)
            {
                Image image = Instantiate(m_imageAudio, m_AudioBG_Grid.transform);
                image.transform.localScale = new Vector3(1, item * height, 1);
                image.gameObject.SetActive(true);
                m_imageTrans.Add(image);
            }
        }
    }

    private void LateUpdate()
    {
        AudioPlayData data = AudioController.Instance.m_BGAudioData;
        if (data == null) return;
        data.audioSource.GetSpectrumData(progress, 0, FFTWindow.BlackmanHarris);
       
        for (int i = 0; i < m_imageTrans.Count; i++)
        {
            Image image = m_imageTrans[i];
            float p = GetProgress(i) * height;
            float r = p / 1.5f;
            float g = 1;
            if (r > 0) g = 1 / r;
            image.color = new Color(r >= 1 ? 1 : r, g, g);
            //if (p >= 1.5f)
            //    image.color = Color.red;
            //else
            //    image.color = Color.white;
            image.transform.localScale = new Vector3(1, p, 1);
        }
    }

    private float GetProgress(int index)
    {
        if (index >= 0 && index < progress.Length)
        {
            return progress[index];
        }
        return 0;
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
