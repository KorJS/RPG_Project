using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(EffectSetting))]
public class EffectEditor : Editor
{
    private EffectSetting effect = null;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        effect = (EffectSetting)target;

        EditorGUILayout.LabelField("- Effect Helper -");

        // Enum형 팝업
        effect.equipType = (EffectSetting.EquipType)EditorGUILayout.EnumPopup("EquipType", effect.equipType);
        effect.continueType = (EffectSetting.ContinueType)EditorGUILayout.EnumPopup("ContinueType", effect.continueType);

        // 새로 지정한 위치 저장
        if (GUILayout.Button("Save effect position"))
        {
            Transform effectT = effect.transform;
            Vector3 effectPos = effectT.localPosition;
            effect.infoSettings.effectPosition = effectPos;
        }
    }
}