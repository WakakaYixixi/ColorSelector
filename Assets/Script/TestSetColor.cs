using UnityEngine;
using System.Collections;

public class TestSetColor : MonoBehaviour
{
    public SelectColor test;
    [Range(0, 1)]
    public float r;
    [Range(0, 1)]
    public float g;
    [Range(0, 1)]
    public float b;

    private void Update()
    {
        if (test != null)
        {
            test.SetColor(new Color(r, g, b));
        }
    }
}
