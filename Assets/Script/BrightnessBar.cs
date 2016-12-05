using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BrightnessBar : MonoBehaviour
{
    public Texture2D tex;
    public Image bar;
    [Range(0,1)]
    public float sliderPersent;
    public CanvasScaler canvas;
    public Image viewer;
    public GameObject pointer;
    private Color curShowColor;

    private void Awake()
    {
        tex = new Texture2D(512, 1);
        tex.wrapMode = TextureWrapMode.Clamp;
        bar.sprite = Sprite.Create(tex, new Rect(0, 0, 512, 1), Vector3.zero);
    }

    private void Start()
    {
        UIhandler.Get(bar.gameObject).onDown = Select;
        UIhandler.Get(bar.gameObject).onDrag = Select;
    }

    private void Select(GameObject o)
    {
        Vector3 pos = Input.mousePosition;
        pos = SelectColor.MouseToCanvas(pos,canvas);
        RectTransform rectTrans = o.GetComponent<RectTransform>();
        Image img = o.GetComponent<Image>();
        Vector3 targetPos = rectTrans.localPosition + rectTrans.parent.localPosition;
        if (pos.x > targetPos.x + rectTrans.sizeDelta.x / 2
            || pos.x < targetPos.x - rectTrans.sizeDelta.x / 2
            || pos.y > targetPos.y + rectTrans.sizeDelta.y / 2
            || pos.y < targetPos.y - rectTrans.sizeDelta.y / 2)
        {
            return;
        }
        sliderPersent = (pos.x - rectTrans.parent.localPosition.x - rectTrans.localPosition.x + rectTrans.sizeDelta.x / 2) / rectTrans.sizeDelta.x;
        pointer.GetComponent<RectTransform>().localPosition = new Vector3(sliderPersent * bar.rectTransform.sizeDelta.x - rectTrans.sizeDelta.x / 2, pointer.transform.localPosition.y, pointer.transform.localPosition.z);
        viewer.color = GetTargetColor(curShowColor, sliderPersent);
    }

    public Color FreshColorTex(Color color)
    {
        for (int i = 0; i < tex.width; i++)
        {
            for (int j = 0; j < tex.height; j++)
            {
                float percent = (float)i / (float)tex.width;
                Color tmp = GetTargetColor(color, percent);
                bar.sprite.texture.SetPixel(i, j, tmp);
            }
        }
        bar.sprite.texture.Apply();
        curShowColor = color;
        return GetTargetColor(color, sliderPersent);
    }

    public Color GetTargetColor(Color color,float percent)
    {
        if (percent < 0.5f)
        {
            color.r *= percent / 0.5f;
            color.g *= percent / 0.5f;
            color.b *= percent / 0.5f;
        }
        else
        {
            color.r = color.r * (1 - percent) / 0.5f + 1f * (1 - (1 - percent) / 0.5f);
            color.g = color.g * (1 - percent) / 0.5f + 1f * (1 - (1 - percent) / 0.5f);
            color.b = color.b * (1 - percent) / 0.5f + 1f * (1 - (1 - percent) / 0.5f);
        }
        return color;
    }
}
