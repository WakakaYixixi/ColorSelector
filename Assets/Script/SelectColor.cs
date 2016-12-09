using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SelectColor : MonoBehaviour
{
    public Image colorPadImg;
    public GameObject Selector;
    public CanvasScaler canvas;
    public BrightnessBar brightness;
    public Image viewer;
    public SetColorPad colorPad;

    private void OnGUI()
    {
        GUILayout.Label("r" + viewer.color.r);
        GUILayout.Label("g" + viewer.color.g);
        GUILayout.Label("b" + viewer.color.b);
    }

    private void Start()
    {
        UIhandler.Get(colorPadImg.gameObject).onDown = Select;
        UIhandler.Get(colorPadImg.gameObject).onDrag = Select;

        Vector3 pos = Selector.transform.localPosition;
        RectTransform rectTrans = colorPadImg.GetComponent<RectTransform>();
        Image img = colorPadImg.GetComponent<Image>();
        float pixelX = ((pos.x + rectTrans.sizeDelta.x / 2) / rectTrans.sizeDelta.x) /* * img.sprite.texture.width*/;
        float pixelY = ((pos.y + rectTrans.sizeDelta.y / 2) / rectTrans.sizeDelta.y)/* * img.sprite.texture.height*/;
        //Color color = img.sprite.texture.GetPixel((int)pixelX, (int)pixelY);
        FreshColor(pixelX, pixelY);
    }

    public void SetColor(Color color)
    {
        Vector2 xyPer;
        float brig;
        colorPad.SetColor(color, out xyPer, out brig);
        Selector.transform.localPosition = new Vector2((xyPer.x - 1f / 2) * colorPadImg.rectTransform.sizeDelta.x, (xyPer.y - 1f / 2) * colorPadImg.rectTransform.sizeDelta.y);
        brightness.SetBrightness(brig);
        FreshColor(xyPer.x,xyPer.y);
    }

    private void Select(GameObject o)
    {
        Vector3 pos = Input.mousePosition;
        pos = MouseToCanvas(pos,canvas);
        RectTransform rectTrans = o.GetComponent<RectTransform>();
        Image img = o.GetComponent<Image>();
        Vector3 targetPos = rectTrans.localPosition + transform.localPosition;
        if (pos.x > targetPos.x + rectTrans.sizeDelta.x / 2
            || pos.x < targetPos.x - rectTrans.sizeDelta.x / 2
            || pos.y > targetPos.y + rectTrans.sizeDelta.y / 2
            || pos.y < targetPos.y - rectTrans.sizeDelta.y / 2)
        {
            return;
        }
        Selector.transform.localPosition = pos - transform.localPosition - rectTrans.localPosition;
        float pixelX = ((Selector.transform.localPosition.x + rectTrans.sizeDelta.x / 2) / rectTrans.sizeDelta.x)/* * img.sprite.texture.width*/;
        float pixelY = ((Selector.transform.localPosition.y + rectTrans.sizeDelta.y / 2) / rectTrans.sizeDelta.y)/* * img.sprite.texture.height*/;
        //Color color = img.sprite.texture.GetPixel((int)pixelX, (int)pixelY);
        FreshColor(pixelX, pixelY);
    }

    public void FreshColor(float perX,float perY)
    {
        Color color = colorPad.GetColorByPercentage(perX, perY);
        color = brightness.FreshColorTex(color);
        viewer.color = color;
    }

    public static Vector3 MouseToCanvas(Vector2 pos,CanvasScaler canvas)
    {
        Vector2 canvasResolution = canvas.referenceResolution;
        float resolutionRatio = (float)Screen.height / Screen.width;
        float scaleX = pos.x / (Screen.width);
        float scaleY = pos.y / (Screen.height);
        if (resolutionRatio >= canvasResolution.y / canvasResolution.x)
        {
            pos = new Vector2(canvasResolution.x * scaleX, canvasResolution.x * resolutionRatio * scaleY);
            pos -= new Vector2(canvasResolution.x, canvasResolution.x * resolutionRatio) / 2;
        }
        else
        {
            pos = new Vector2(canvasResolution.y / resolutionRatio * scaleX, canvasResolution.y * scaleY);
            pos -= new Vector2(canvasResolution.y / resolutionRatio, canvasResolution.y) / 2;
        }

        if (float.IsNaN(pos.x) || float.IsNaN(pos.y))
        {
            return Vector2.zero;
        }

        return pos;
    }

}
