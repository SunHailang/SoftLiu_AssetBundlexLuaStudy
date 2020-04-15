using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExitGamePopup : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_textAppName = null;
    [SerializeField]
    private Image m_imageIcon = null;

    private void Start()
    {
        byte[] datas = SoftLiuNativeBinding.Instance.GetIconBytes();
        Texture2D texture = new Texture2D(120, 120);
        bool result = texture.LoadImage(datas);
        if (result)
        {
            m_imageIcon.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one / 2);
        }
        else
        {
            Debug.Log("GetIconBytes Error.");
        }
        string appName = SoftLiuNativeBinding.Instance.GetAppName();
        m_textAppName.SetText(appName);
    }

    public void ButtonSure_OnClick()
    {
        Application.Quit(0);
    }

    public void ButtonCancel_OnClick()
    {
        Destroy(gameObject);
    }
}
