using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    [SerializeField] private float fadeDuration;

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
        //playgroundRd.material.color = schemes[currentScheme].playground;
        playgroundRd.material.DOColor(schemes[currentScheme].playground, fadeDuration);
        //backgroundRd.material.color = schemes[currentScheme].background;
        backgroundRd.material.DOColor(schemes[currentScheme].background, fadeDuration);
        RenderSettings.ambientLight = schemes[currentScheme].ambientColor;
    }
}
