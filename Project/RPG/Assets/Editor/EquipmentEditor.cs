using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Equipment))]
public class EquipmentEditor : Editor
{
    private Equipment equipment = null;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        equipment = (Equipment)target;


        EditorGUILayout.LabelField("- Equipment Helper -");

        // Enum형 팝업
        equipment.eqType = (Equipment.EquipmentType)EditorGUILayout.EnumPopup("EquipType", equipment.eqType);

        // 새로 지정한 위치 저장
        if (GUILayout.Button("Save equip location"))
        {
            Transform equipT = equipment.locationSettings.equipT;
            Vector3 equipPos = equipT.localPosition;
            Vector3 equipRot = equipT.localEulerAngles;
            equipment.locationSettings.equipPosition = equipPos;
            equipment.locationSettings.equipRotation = equipRot;
        }

        if (GUILayout.Button("Save unequip location"))
        {
            Transform unequipT = equipment.locationSettings.unequipT;
            Vector3 unequipPos = unequipT.localPosition;
            Vector3 unequipRot = unequipT.localEulerAngles;
            equipment.locationSettings.unequipPosition = unequipPos;
            equipment.locationSettings.unequipRotation = unequipRot;
        }

        EditorGUILayout.LabelField("Debug Positioning");

        // 지정한 위치(원위치)로
        if (GUILayout.Button("Move equip location"))
        {
            Transform equipT = equipment.locationSettings.equipT;
            equipT.localPosition = equipment.locationSettings.equipPosition;
            Quaternion eulerAngles = Quaternion.Euler(equipment.locationSettings.equipRotation);
            equipT.localRotation = eulerAngles;
        }

        // 지정한 위치(원위치)로
        if (GUILayout.Button("Move unequip location"))
        {
            Transform unequipT = equipment.locationSettings.unequipT;
            unequipT.localPosition = equipment.locationSettings.unequipPosition;
            Quaternion eulerAngles = Quaternion.Euler(equipment.locationSettings.unequipRotation);
            unequipT.localRotation = eulerAngles;
        }
    }
}
