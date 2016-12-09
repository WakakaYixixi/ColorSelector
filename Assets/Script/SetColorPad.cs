using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class SetColorPad : MonoBehaviour
{
    private enum colorNum
    { max, mid, min };
    public Image pad;
    public Texture2D tex2d;
    private int texW = 512, texH = 512;

    private void Awake()
    {
        Profiler.BeginSample("Test");
        tex2d = CreateColorTex();
        pad.sprite = Sprite.Create(tex2d, new Rect(0, 0, texW, texH), Vector3.zero);
        //byte[] bytes = tex2d.EncodeToPNG();
        //FileStream file = File.Open("D:/"+"save" + ".png", FileMode.Create);
        //BinaryWriter writer = new BinaryWriter(file);
        //writer.Write(bytes);
        //file.Close();
        Profiler.EndSample();
    }

    private Texture2D CreateColorTex()
    {
        Texture2D tex = new Texture2D(texW, texH);
        for (int i = 0; i < tex.width; i++)
        {
            for (int j = 0; j < tex.height; j++)
            {
                Color color = GetColor(i, j);
                tex.SetPixel(i, j, color);
            }
        }
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.Apply();
        return tex;
    }

    public void SetColor(Color color, out Vector2 xyPercent, out float brightness)
    {
        xyPercent = Vector2.zero;
        brightness = 0;
        if (color == Color.white)
        {
            brightness = 1;
            return;
        }
        if (color == Color.black)
        {
            return;
        }
        //公式:最小+最大/2表示亮度
        //         去亮度后最小-0或者1-最大 除以0.5即为H值
        brightness = (GetMinMax(colorNum.max, color) + GetMinMax(colorNum.min, color)) / 2;
        if (brightness < 0.5f)
        {
            color.r /= brightness / 0.5f;
            color.g /= brightness / 0.5f;
            color.b /= brightness / 0.5f;
        }
        else
        {
            color.r = (color.r - 1f * (1 - (1 - brightness) / 0.5f)) / ((1 - brightness) / 0.5f);
            color.g = (color.g - 1f * (1 - (1 - brightness) / 0.5f)) / ((1 - brightness) / 0.5f);
            color.b = (color.b - 1f * (1 - (1 - brightness) / 0.5f)) / ((1 - brightness) / 0.5f);
        }
        xyPercent.y = (GetMinMax(colorNum.max, color) - 0.5f) / 0.5f;

        color = ReColor(color, xyPercent.y);

        if (color.r == 1)
        {
            if (color.g <= 0)
            {
                xyPercent.x = 1f / 6 - color.b * 1f / 6;
            }
            else
            {
                xyPercent.x = 1f / 6 + color.g * 1f / 6;
            }
        }
        else if (color.g == 1)
        {
            if (color.b <= 0)
            {
                xyPercent.x = 1f / 2 - color.r * 1f / 6;
            }
            else
            {
                xyPercent.x = 1f / 2 + color.b * 1f / 6;
            }
        }
        else if (color.b == 1)
        {
            if (color.r <= 0)
            {
                xyPercent.x = 5f / 6 - color.g * 1f / 6;
            }
            else
            {
                xyPercent.x = 5f / 6 + color.r * 1f / 6;
            }
        }
    }

    private Color ReColor(Color color, float yPer)
    {
        float max = GetMinMax(colorNum.max, color);
        float mid = GetMinMax(colorNum.mid, color);
        float min = GetMinMax(colorNum.min, color);
        if (Mathf.Abs(max - color.r) < 0.01f)
        {
            color.r = 1;
        }
        else if (Mathf.Abs(max - color.g) < 0.01f)
        {
            color.g = 1;
        }
        else
        {
            color.b = 1;
        }

        if (Mathf.Abs(min - color.r) < 0.01f)
        {
            color.r = 0;
        }
        else if (Mathf.Abs(min - color.g) < 0.01f)
        {
            color.g = 0;
        }
        else
        {
            color.b = 0;
        }

        if (Mathf.Abs(mid - color.r) < 0.01f)
        {
            color.r = (color.r - 0.5f) / yPer + 0.5f;
        }
        else if (Mathf.Abs(mid - color.g) < 0.01f)
        {
            color.g = (color.g - 0.5f) / yPer + 0.5f;
        }
        else
        {
            color.b = (color.b - 0.5f) / yPer + 0.5f;
        }
        return color;
    }

    private float GetMinMax(colorNum type, Color color)
    {
        float[] floats = new float[] { color.r, color.g, color.b };
        for (int i = floats.Length - 1; i > 0; i--)
        {
            for (int j = i - 1; j >= 0; j--)
            {
                if (floats[i] > floats[j])
                {
                    floats[j] += floats[i];
                    floats[i] = floats[j] - floats[i];
                    floats[j] -= floats[i];
                }
            }
        }
        switch (type)
        {
            case colorNum.max: return floats[0];
            case colorNum.mid: return floats[1];
            case colorNum.min: return floats[2];
        }
        return 0;
    }

    public Color GetColorByPercentage(float perX, float perY)
    {
        return GetColor(texW * perX, texH * perY);
    }

    private Color GetColor(float x, float y)
    {
        float r = 0;
        float g = 0;
        float b = 0;
        if (0 <= x && x < texW / 6)
        {
            r = 1;
            b = Mathf.Abs(texW / 6 - x) / (texW / 6);
        }
        else if (texW / 6 <= x && x < texW / 3)
        {
            r = 1;
            g = 1 - Mathf.Abs(texW / 3 - x) / (texW / 6);
        }
        else if (texW / 3 <= x && x < texW / 2)
        {
            g = 1;
            r = Mathf.Abs(texW / 2 - x) / (texW / 6);
        }
        else if (texW / 2 <= x && x < texW * 2 / 3)
        {
            g = 1;
            b = 1 - Mathf.Abs(texW * 2 / 3 - x) / (texW / 6);
        }
        else if (texW * 2 / 3 <= x && x < texW * 5 / 6)
        {
            b = 1;
            g = Mathf.Abs(texW * 5 / 6 - x) / (texW / 6);
        }
        else if (texW * 5 / 6 <= x && x < texW)
        {
            b = 1;
            r = 1 - Mathf.Abs(texW - x) / (texW / 6);
        }
        Color color = new Color(r, g, b);

        if (y <= texH)
        {
            color.r = color.r * y / texH + 0.5f * (1 - y / texH);
            color.g = color.g * y / texH + 0.5f * (1 - y / texH);
            color.b = color.b * y / texH + 0.5f * (1 - y / texH);
        }
        //else
        //{
        //    color =Color.white - (Color.white - color) *(1-( y-halfY) / halfY);
        //}
        return color;
    }
}
