//using System.Collections;
//using System.Collections.Generic;
//using System.Reflection;
//using UnityEngine;
//using UnityEngine.Rendering;
//using UnityEngine.Rendering.Universal;

//[ExecuteInEditMode]
//public class GetOverdrawInfo : MonoBehaviour
//{
//    public UniversalRendererData RendererData;
//    public bool DisplayOverDrawMonitor;


//    private OverdrawMonitorFeature renderFeature;

//    private void Awake()
//    {

//        foreach (var feature in RendererData.rendererFeatures)
//        {
//            if (feature.GetType() == typeof(OverdrawMonitorFeature))
//                renderFeature = feature as OverdrawMonitorFeature;
//        }
//        if (renderFeature == null)
//        {
//            RendererData.rendererFeatures.Add(new OverdrawMonitorFeature());
//        }
//    }

//    private void OnEnable()
//    {
//        if (renderFeature != null)
//        {
//            renderFeature.SetActive(true);
//        }
//        else
//        {
//            foreach (var feature in RendererData.rendererFeatures)
//            {
//                if (feature.GetType() == typeof(OverdrawMonitorFeature))
//                    renderFeature = feature as OverdrawMonitorFeature;
//            }
//        }
//    }

//    void Update()
//    {
//        if (renderFeature != null)
//        {
//            Debug.Log(renderFeature.OverdrawRatio);
//        }
//    }
//    private void OnDisable()
//    {
//        if (renderFeature != null)
//        {
//            renderFeature.SetActive(false);
//        }
//    }
//    private void OnValidate()
//    {
//        if (renderFeature != null)
//        {
//            //renderFeature.DisplayOverDrawMonitor = DisplayOverDrawMonitor;
//            RendererData.SetDirty();
//        }
//    }
//}
