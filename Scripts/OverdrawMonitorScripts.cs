using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

[ExecuteInEditMode]
public class OverdrawMonitorScripts : MonoBehaviour
{
    private Text OverdrawMonitorText;
    private OverdrawMonitorComponent overdrawMonitorComponent = null;
    private GameObject canvasGameObject = null;
    private static OverdrawMonitorScripts instance;

    void Start()
    {
        Init();
    }
    private void Update()
    {
        OverdrawMonitorText.text = string.Format("OverdrawRatio:{0}\nMaxOverdrawRatio:{1}",
            Math.Round(OverdrawMonitorFeature.OverdrawRatio, 2),
            Math.Round(OverdrawMonitorFeature.MaxOverdrawRatio, 2));
    }
    public static OverdrawMonitorScripts Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<OverdrawMonitorScripts>();
                if (instance == null)
                {
                    var go = new GameObject("OverdrawMonitor");
                    instance = go.AddComponent<OverdrawMonitorScripts>();
                    go.AddComponent<Volume>();
                }
            }
            return instance;
        }
    }

    public void TurnOnOverdrawMonitor()
    {
        Init();
        canvasGameObject.SetActive(true);
        overdrawMonitorComponent.SetAllOverridesTo(true);
        EditorUtility.SetDirty(overdrawMonitorComponent);
    }

    public void TurnOffOverdrawMonitor()
    {
        canvasGameObject.SetActive(false);
        overdrawMonitorComponent.SetAllOverridesTo(false);
        EditorUtility.SetDirty(overdrawMonitorComponent);
        OverdrawMonitorFeature.ResetOverdrawMonitor();
    }

    private void Init()
    {
        Instance.InitOverdrawMonitorComponent();
        Instance.InitCanvsa();
    }

    private void InitOverdrawMonitorComponent()
    {
        if (overdrawMonitorComponent == null)
        {
            GetComponent<Volume>().profile.TryGet(out overdrawMonitorComponent);
            if (overdrawMonitorComponent == null)
            {
                overdrawMonitorComponent = GetComponent<Volume>().profile.Add<OverdrawMonitorComponent>();
            }
        }
        overdrawMonitorComponent.CountOverdrawRatio.Override(true);
        overdrawMonitorComponent.DisplayOverDrawResultOnScreen.Override(true);
    }

    private void InitCanvsa()
    {
        if (canvasGameObject == null)
        {
            canvasGameObject = GetComponentInChildren<Canvas>()?.gameObject;
            if (canvasGameObject == null)
            {
                canvasGameObject = new GameObject("Canvas");
                canvasGameObject.transform.SetParent(transform);
                Canvas canvas = canvasGameObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGameObject.AddComponent<CanvasScaler>();

                GameObject textGameObject = new GameObject("Text");
                textGameObject.transform.SetParent(canvasGameObject.transform);

                RectTransform rectTransform = textGameObject.AddComponent<RectTransform>();

                rectTransform.anchorMax = new Vector2(1, 1);
                rectTransform.anchorMin = new Vector2(1, 1);
                rectTransform.pivot = new Vector2(1, 1);
                rectTransform.anchoredPosition = new Vector2(-10, -10);
                rectTransform.sizeDelta = new Vector2(600, 300);

                // 设置Text UI元素的文本
                OverdrawMonitorText = textGameObject.AddComponent<Text>();
                OverdrawMonitorText.text = "";
                OverdrawMonitorText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                OverdrawMonitorText.fontSize = 46;
                OverdrawMonitorText.color = Color.white;
                canvasGameObject.SetActive(false);
            }
        }
        if (OverdrawMonitorText == null)
        {
            OverdrawMonitorText = canvasGameObject.GetComponentInChildren<Text>();
        }
    }

    public static void GetOverdrawRatio(out float OverdrawRatio, out float MaxOverdrawRatio)
    {
        OverdrawRatio = OverdrawMonitorFeature.OverdrawRatio;
        MaxOverdrawRatio = OverdrawMonitorFeature.MaxOverdrawRatio;
    }
}