using UnityEngine;
using UnityEditor;

public class OverdrawMonitorWindow : EditorWindow
{
    private static float m_OverdrawRatio;
    private static float m_MaxOverdrawRatio;

    [MenuItem("Tools/Overdraw Monitor")]
    public static void ShowWindow()
    {
        var window = GetWindow<OverdrawMonitorWindow>("Overdraw Monitor");
        window.Focus();
    }

    private void OnGUI()
    {
        OverdrawMonitorScripts.GetOverdrawRatio(out m_OverdrawRatio, out m_MaxOverdrawRatio);
        using (new GUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Start"))
            {
                OverdrawMonitorScripts.Instance.TurnOnOverdrawMonitor();
            }
            if (GUILayout.Button("End"))
            {
                OverdrawMonitorScripts.Instance.TurnOffOverdrawMonitor();
            }
        }
        using (new GUILayout.HorizontalScope())
        {
            GUILayout.Label("Current\n" + m_OverdrawRatio.ToString("0.000"));
            GUILayout.FlexibleSpace();
            GUILayout.Label("Max\n" + m_MaxOverdrawRatio.ToString("0.000"));
        }
        Repaint();
    }

}
