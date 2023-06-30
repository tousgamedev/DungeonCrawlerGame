using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EncounterZone))]
public class EncounterZoneEditor : Editor
{
    private EncounterZone encounterZone;
    private SerializedProperty encounterRate;
    private SerializedProperty encounterPairsProperty;
    
    private void OnEnable()
    {
        encounterZone = (EncounterZone)target;
        encounterRate = serializedObject.FindProperty("encounterRatePercent");
        encounterPairsProperty = serializedObject.FindProperty("encounterPairs");
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(encounterZone), typeof(EncounterZone), false);
        EditorGUI.EndDisabledGroup();
        
        serializedObject.Update();

        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(encounterRate, true);
        EditorGUILayout.Space(10);
        
        DisplayEncounterPairs();
        DisplayButtons();
        
        serializedObject.ApplyModifiedProperties();
    }

    private void DisplayEncounterPairs()
    {
        EditorGUILayout.LabelField("Enemy Encounters", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Group");
        GUILayout.FlexibleSpace();
        GUILayout.Label("Weight");
        GUILayout.Space(40);
        EditorGUILayout.EndHorizontal();
        
        for (var i = 0; i < encounterPairsProperty.arraySize; i++)
        {
            SerializedProperty pairProperty = encounterPairsProperty.GetArrayElementAtIndex(i);
            SerializedProperty encounterProperty = pairProperty.FindPropertyRelative("EncounterGroup");
            SerializedProperty weightProperty = pairProperty.FindPropertyRelative("Weight");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(encounterProperty, GUIContent.none);
            EditorGUILayout.PropertyField(weightProperty, GUIContent.none, GUILayout.Width(60));
           
            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                encounterPairsProperty.DeleteArrayElementAtIndex(i);
            }
            
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DisplayButtons()
    {
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("+ Add Encounter", GUILayout.Width(120)))
        {
            encounterPairsProperty.InsertArrayElementAtIndex(encounterPairsProperty.arraySize);
        }
        
        GUILayout.FlexibleSpace();

        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("Sort Weights", GUILayout.Width(140)))
        {
            encounterZone.SortEncounterPairsByWeight();
        }
        
        if (GUILayout.Button("Recalculate Weights", GUILayout.Width(140)))
        {
            encounterZone.AdjustEncounterWeights();
        }
        GUILayout.EndVertical();
        
        EditorGUILayout.EndHorizontal();
    }
}