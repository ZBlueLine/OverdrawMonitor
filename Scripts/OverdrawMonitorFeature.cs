using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static Unity.Burst.Intrinsics.X86.Avx;
using static UnityEditor.Rendering.InspectorCurveEditor;

class OverdrawMonitorPass : ScriptableRenderPass
{
    public float OverdrawRatio;
    public float MaxOverdrawRatio;

    private RenderTargetIdentifier defaultColorTargetID;

    private FilteringSettings filteringSettingsTransparent;
    private FilteringSettings filteringSettingsOpaque;

    private ProfilingSampler _profilingSampler;

    private int colorTargetDestinationID;
    private List<ShaderTagId> shaderTagsList = new List<ShaderTagId>();

    private Material overrideMaterialOpaque = new Material(Shader.Find("OverDrawMonitor"));
    private Material overrideMaterialTransparent = new Material(Shader.Find("OverDrawMonitor"));
    private ComputeShader OverdrawParallelReduction;
    private int kernelIndex;
    private int OverdrawTexID;
    private int OverdrawOutputID;
    private const int dataSize = 128 * 128;
    private int[] inputData = new int[dataSize];
    private int[] outputData = new int[dataSize];
    private int xGroups;
    private int yGroups;
    private ComputeBuffer resultBuffer;

    private OverdrawMonitorComponent overdrawmonitorcomponent;

    public OverdrawMonitorPass(LayerMask layerMask, ComputeShader overdrawParallelReduction, OverdrawMonitorComponent overdrawmonitorcomponent)
    {
        OverdrawParallelReduction = overdrawParallelReduction;
        kernelIndex = OverdrawParallelReduction.FindKernel("CSMain");
        OverdrawTexID = Shader.PropertyToID("Overdraw");
        OverdrawOutputID = Shader.PropertyToID("Output");
        resultBuffer = new ComputeBuffer(outputData.Length, 4);
        this.overdrawmonitorcomponent = overdrawmonitorcomponent;

        _profilingSampler = new ProfilingSampler("OverdrawMonitor");

        filteringSettingsOpaque = new FilteringSettings(RenderQueueRange.opaque, layerMask);
        filteringSettingsTransparent = new FilteringSettings(RenderQueueRange.transparent, layerMask);

        colorTargetDestinationID = Shader.PropertyToID("OverDrawRenderTexture");

        shaderTagsList.Add(new ShaderTagId("SRPDefaultUnlit"));
        shaderTagsList.Add(new ShaderTagId("UniversalForward"));
        shaderTagsList.Add(new ShaderTagId("UniversalForwardOnly"));

        overrideMaterialOpaque.SetFloat("HARDWARE_ZWrite", 1);
        overrideMaterialTransparent.SetFloat("HARDWARE_ZWrite", 0);

    }

    // This method is called before executing the render pass.
    // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
    // When empty this render pass will render to the active camera render target.
    // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
    // The render pipeline will ensure target setup and clearing happens in a performant manner.
    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        CameraData cameraData = renderingData.cameraData;
        cmd.GetTemporaryRT(colorTargetDestinationID, cameraData.cameraTargetDescriptor.width,
            cameraData.cameraTargetDescriptor.height, 24, FilterMode.Point, RenderTextureFormat.RFloat);
        defaultColorTargetID = colorAttachment;
        ConfigureTarget(colorTargetDestinationID, cameraData.renderer.cameraDepthTarget);

    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get();
        CameraData cameraData = renderingData.cameraData;
        using (new ProfilingScope(cmd, _profilingSampler))
        {
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            SortingCriteria sortingCriteria = cameraData.defaultOpaqueSortFlags;
            var drawingSettingsOpaque = CreateDrawingSettings(shaderTagsList, ref renderingData, sortingCriteria);
            var drawingSettingsTransparent = CreateDrawingSettings(shaderTagsList, ref renderingData, sortingCriteria);

            if (overrideMaterialTransparent != null)
            {
                drawingSettingsOpaque.overrideMaterial = overrideMaterialOpaque;
                drawingSettingsOpaque.overrideMaterialPassIndex = 0;
                drawingSettingsTransparent.overrideMaterial = overrideMaterialTransparent;
                drawingSettingsTransparent.overrideMaterialPassIndex = 0;
            }
            cmd.ClearRenderTarget(true, true, clearColor);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            context.DrawRenderers(renderingData.cullResults, ref drawingSettingsOpaque, ref filteringSettingsOpaque);
            context.DrawRenderers(renderingData.cullResults, ref drawingSettingsTransparent, ref filteringSettingsTransparent);

            cmd.SetBufferData(resultBuffer, inputData);
            cmd.SetComputeTextureParam(OverdrawParallelReduction, kernelIndex, OverdrawTexID, colorTargetDestinationID);
            //cmd.SetComputeBufferParam(OverdrawParallelReduction, kernelIndex, OverdrawOutputID, resultBuffer);
            cmd.SetGlobalBuffer(OverdrawOutputID, resultBuffer);
            xGroups = cameraData.cameraTargetDescriptor.width / 32;
            yGroups = cameraData.cameraTargetDescriptor.height / 32;
            cmd.DispatchCompute(OverdrawParallelReduction, kernelIndex, xGroups, yGroups, 1);
            if (overdrawmonitorcomponent.DisplayOverDrawResultOnScreen == true)
            {
                cmd.Blit(colorTargetDestinationID, cameraData.targetTexture);
            }
            ConfigureTarget(defaultColorTargetID);
        }
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }

    // Cleanup any allocated resources that were created during the execution of this render pass.
    public override void OnFinishCameraStackRendering(CommandBuffer cmd)
    {
        int TotalShadedFragments = 0;
        resultBuffer.GetData(outputData);

        foreach (int i in outputData)
        {
            TotalShadedFragments += i;
        }
        OverdrawRatio = (float)TotalShadedFragments / (xGroups * 32 * yGroups * 32);
        if(OverdrawRatio > MaxOverdrawRatio)
        {
            MaxOverdrawRatio = OverdrawRatio;
        }
        OverdrawMonitorFeature.OverdrawRatio = OverdrawRatio;
        OverdrawMonitorFeature.MaxOverdrawRatio = MaxOverdrawRatio;
    }
    public void Clear()
    {
        MaxOverdrawRatio = 0;
    }
}

//直接通过OverdrawMonitor类注入Pass，Renderfeature不便于动态控制
public class OverdrawMonitorFeature : ScriptableRendererFeature
{
    public LayerMask layerMask;
    public ComputeShader OverdrawCountComputeShader;

    public static float OverdrawRatio = 0;
    public static float MaxOverdrawRatio = 0;

    OverdrawMonitorPass m_ScriptablePass;

    private OverdrawMonitorComponent overdrawmonitorcomponent;
    public override void Create()
    {
        overdrawmonitorcomponent = VolumeManager.instance.stack.GetComponent<OverdrawMonitorComponent>();

        layerMask = LayerMask.GetMask("Default");
        OverdrawCountComputeShader = UnityEditor.AssetDatabase.LoadAssetAtPath<ComputeShader>("Assets/OverdrawMonitor/Shaders/OverdrawCountComputeShader.compute");
        m_ScriptablePass = new OverdrawMonitorPass(layerMask, OverdrawCountComputeShader, overdrawmonitorcomponent);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (overdrawmonitorcomponent.IsActive())
        {
            m_ScriptablePass.Clear();
            if (overdrawmonitorcomponent.DisplayOverDrawResultOnScreen == true)
            {
                m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRendering;
            }
            else
            {
                m_ScriptablePass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            }
            if (renderingData.cameraData.camera == Camera.main)
            {
                renderer.EnqueuePass(m_ScriptablePass);
            }
        }
    }
}


