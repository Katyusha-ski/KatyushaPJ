using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CutsceneData))]
public class CutsceneDataDrawer : Editor
{
    private SerializedProperty actions;

    private void OnEnable()
    {
        actions = serializedObject.FindProperty("actions");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Cutscene Actions", EditorStyles.boldLabel);
        DrawActionList();

        if (GUILayout.Button("+ Add Action", GUILayout.Height(24)))
        {
            BuildTypeMenu(-1).ShowAsContext();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawActionList()
    {
        if (actions == null) return;

        for (int i = 0; i < actions.arraySize; i++)
        {
            SerializedProperty element = actions.GetArrayElementAtIndex(i);
            bool isNull = element.managedReferenceValue == null;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();
            element.isExpanded = EditorGUILayout.Foldout(element.isExpanded,
                isNull ? "None (SequenceAction)" : element.managedReferenceValue.GetType().Name, true);
            GUILayout.FlexibleSpace();
            int captured = i;
            if (GUILayout.Button("Type", GUILayout.Width(50)))
                BuildTypeMenu(captured).ShowAsContext();
            if (GUILayout.Button("-", GUILayout.Width(24)))
                actions.DeleteArrayElementAtIndex(captured);
            EditorGUILayout.EndHorizontal();

            if (element.isExpanded && !isNull)
            {
                EditorGUILayout.BeginVertical();
                DrawManagedFields(element);
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();
        }
    }

    private void DrawManagedFields(SerializedProperty element)
    {
        EditorGUI.indentLevel++;
        SerializedProperty iterator = element.Copy();
        SerializedProperty end = element.GetEndProperty();
        bool enterChildren = true;

        while (iterator.NextVisible(enterChildren))
        {
            if (SerializedProperty.EqualContents(iterator, end))
                break;
            enterChildren = false;

            if (iterator.name == "waiting")
                continue;

            EditorGUILayout.PropertyField(iterator, true);
        }
        EditorGUI.indentLevel--;
    }

    private GenericMenu BuildTypeMenu(int targetIndex)
    {
        GenericMenu menu = new GenericMenu();
        foreach (Type type in GetActionTypes())
        {
            menu.AddItem(new GUIContent(type.Name), false, () =>
            {
                object instance = Activator.CreateInstance(type);
                if (targetIndex >= 0)
                    actions.GetArrayElementAtIndex(targetIndex).managedReferenceValue = instance;
                else
                {
                    actions.arraySize++;
                    actions.GetArrayElementAtIndex(actions.arraySize - 1).managedReferenceValue = instance;
                }
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            });
        }
        return menu;
    }

    private static IEnumerable<Type> GetActionTypes()
    {
        return TypeCache.GetTypesDerivedFrom<SequenceAction>()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .OrderBy(t => t.Name);
    }
}