using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    [Serializable]
    public struct ColorScheme
    {
        public string name;
        public Color background;
        public Color playground;
        public Color ambientColor;
    }

    [SerializeField] private ColorScheme[] schemes;
    private int currentScheme;

    [SerializeField] private Renderer backgroundRd;
    [SerializeField] private Renderer playgroundRd;
    private static readonly int Color = Shader.PropertyToID("_Color");

    private void Start()
    {
        currentScheme = -1;
        ChangeColorScheme();
    }

    public void ChangeColorScheme()
    {
        currentScheme++;
        if (currentScheme == schemes.Length) currentScheme = 0;
        Debug.Log("Changing Color scheme");
        playgroundRd.material.color = schemes[currentScheme].playground;
        backgroundRd.material.color = schemes[currentScheme].background;
        RenderSettings.ambientLight = schemes[currentScheme].ambientColor;
    }
}
