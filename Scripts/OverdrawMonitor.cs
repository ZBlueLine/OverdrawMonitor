//using UnityEditor;
//using UnityEngine;
//using UnityEngine.Rendering;
//using UnityEngine.Rendering.Universal;

//[ExecuteInEditMode]
//public class OverdrawMonitor : MonoBehaviour
//{
//    public LayerMask layerMask;
//    public bool DisplayOverDrawMonitor = true;

//    private ComputeShader OverdrawCountComputeShader;
//    private OverdrawMonitorPass overdrawMonitorPass;
//    private float overdrawRatio
//    {
//        get => overdrawMonitorPass.OverdrawRatio;
//    }

//    private void Awake()
//    {
//        Init();
//    }
//    private void OnEnable()
//    {
//        RenderPipelineManager.beginCameraRendering += BeginCameraRendering;
//    }
//    private void Update()
//    {
//        if (overdrawMonitorPass == null)
//            return;
//        Debug.Log(overdrawRatio);
//    }
//    private void OnDisable()
//    {
//        Cleanup();
//    }

//    private void Init()
//    {
//        layerMask = LayerMask.GetMask("Default");
//        OverdrawCountComputeShader = AssetDatabase.LoadAssetAtPath<ComputeShader>("Assets/OverdrawMonitor/Shaders/OverdrawCountComputeShader.compute");
//    }
//    private void BeginCameraRendering(ScriptableRenderContext src, Camera cam)
//    {
//        if (cam.cameraType != CameraType.Game)
//            return;

//        overdrawMonitorPass = new OverdrawMonitorPass(layerMask, OverdrawCountComputeShader, DisplayOverDrawMonitor);
//        overdrawMonitorPass.renderPassEvent = RenderPassEvent.AfterRendering;
//        var urpData = cam.GetUniversalAdditionalCameraData();
//        urpData.scriptableRenderer.EnqueuePass(overdrawMonitorPass);
//    }

//    void Cleanup()
//    {
//        RenderPipelineManager.beginCameraRendering -= BeginCameraRendering;
//    }
//}
