using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SelectColor : MonoBehaviour
{
    public Image colorPad;
    public GameObject Selector;
    public CanvasScaler canvas;
    public BrightnessBar brightness;
    public Image viewer;

    private void Start()
    {
        UIhandler.Get(colorPad.gameObject).onDown = Select;
        UIhandler.Get(colorPad.gameObject).onDrag = Select;

        Vector3 pos = Selector.transform.localPosition;
        RectTransform rectTrans = colorPad.GetComponent<RectTransform>();
        Image img = colorPad.GetComponent<Image>();
        float pixelX = ((pos.x + rectTrans.sizeDelta.x / 2) / rectTrans.sizeDelta.x) * img.sprite.texture.width;
        float pixelY = ((pos.y + rectTrans.sizeDelta.y / 2) / rectTrans.sizeDelta.y) * img.sprite.texture.height;
        Color color = img.sprite.texture.GetPixel((int)pixelX, (int)pixelY);
        color = brightness.FreshColorTex(color);
        viewer.color = color;
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
        float pixelX = ((Selector.transform.localPosition.x + rectTrans.sizeDelta.x / 2) / rectTrans.sizeDelta.x) * img.sprite.texture.width;
        float pixelY = ((Selector.transform.localPosition.y + rectTrans.sizeDelta.y / 2) / rectTrans.sizeDelta.y) * img.sprite.texture.height;
        Color color = img.sprite.texture.GetPixel((int)pixelX, (int)pixelY);
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
