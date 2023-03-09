using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

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
    private bool autoChangeColor;
    [SerializeField] private float autoColorDuration;
    [SerializeField] private float autoColorFade;
    [SerializeField] private Button colorButton;

    private void Start()
    {
        currentScheme = -1;
        complete = true;
        ChangeColorScheme();
    }

    public void ChangeColor() => ChangeColorScheme();

    private void ChangeColorScheme(float fade = -1f)
    {
        if (!complete) return;
        complete = false;
        if ((int) fade == -1) fade = fadeDuration;
        currentScheme++;
        if (currentScheme == schemes.Length) currentScheme = 0;
        Debug.Log("Changing Color scheme");
        playgroundRd.material.DOColor(schemes[currentScheme].playground, fade);
        backgroundRd.material.DOColor(schemes[currentScheme].background, fade).onComplete = OnComplete;
        RenderSettings.ambientLight = schemes[currentScheme].ambientColor;
    }

    private bool complete;
    private void OnComplete()
    {
        complete = true;
    }

    private void Update()
    {
        AutoChangeColor();
    }

    public void ToggleAutoChangeColor(bool b)
    {
        autoChangeColor = b;
        colorButton.interactable = !b;
        if (autoChangeColor) timer = autoColorDuration + autoColorFade;
    }

    private float timer;

    private void AutoChangeColor()
    {
        if (!autoChangeColor) return;

        if (timer >= autoColorDuration + autoColorFade)
        {
            ChangeColorScheme(autoColorFade);
            timer = 0;
        }
        else timer += Time.deltaTime;
    }
}