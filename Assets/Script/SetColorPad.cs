using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class SetColorPad : MonoBehaviour
{
    public Image pad;
    public Texture2D tex2d;

    private void Awake()
    {
        Profiler.BeginSample("Test");
        tex2d=CreateColorTex();
        pad.sprite = Sprite.Create(tex2d, new Rect(0, 0, 512, 512), Vector3.zero);
        //byte[] bytes = tex2d.EncodeToPNG();
        //FileStream file = File.Open("D:/"+"save" + ".png", FileMode.Create);
        //BinaryWriter writer = new BinaryWriter(file);
        //writer.Write(bytes);
        //file.Close();
        Profiler.EndSample();
    }

    private Texture2D CreateColorTex()
    {
        Texture2D tex = new Texture2D(512, 512);
        for (int i = 0; i < tex.width; i++)
        {
            for (int j = 0; j < tex.height; j++)
            {
                Color color = GetColor(i, j, 512, 512);
                tex.SetPixel(i, j, color);
            }
        }
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.Apply();
        return tex;
    }

    private Color GetColor(float x, float y, float texW, float texH)
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
            b = 1 - Mathf.Abs(texW * 2 / 3- x) / (texW / 6);
        }
        else if (texW *2 / 3 <= x && x < texW * 5 / 6)
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
