using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;

[CustomEditor(typeof(MSChartParser))]
public class MSChartParserEditor : Editor
{
    private MSChartParser _target;

    private void OnEnable()
    {
        _target = (MSChartParser)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Parse Charts"))
        {
            _target.ParseCharts();
        }
    }
}