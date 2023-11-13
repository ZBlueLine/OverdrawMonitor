using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class OverdrawMonitorScripts : MonoBehaviour
{

    private Text text;
    //private OverdrawMonitorFeature renderFeature;
    //private UniversalRendererData URPData;
    private void Awake()
    {
        //UniversalRenderPipelineAsset URPAsset = (UniversalRenderPipelineAsset)QualitySettings.renderPipeline;
        //FieldInfo propertyInfo = URPAsset.GetType().GetField("m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic);
        //URPData = (UniversalRendererData)(((ScriptableRendererData[])propertyInfo?.GetValue(URPAsset))?[0]);
        //foreach (var feature in URPData.rendererFeatures)
        //{
        //    if (feature.GetType() == typeof(OverdrawMonitorFeature))
        //        renderFeature = feature as OverdrawMonitorFeature;
        //}
    }
    void Start()
    {
        // 创建新的Canvas
        GameObject canvasGameObject = new GameObject("Canvas");
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
        text = textGameObject.AddComponent<Text>();
        text.text = "Hello, World!";
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 46;
        text.color = Color.white;
    }

    private void Update()
    {
        text.text = string.Format("OverdrawRatio:{0}\nMaxOverdrawRatio:{1}",
            Math.Round(OverdrawMonitorFeature.OverdrawRatio, 2),
            Math.Round(OverdrawMonitorFeature.MaxOverdrawRatio, 2));
    }
}