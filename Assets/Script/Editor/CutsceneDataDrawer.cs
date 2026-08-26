using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CutsceneData))]
public class CutsceneDataDrawer : Editor
{
    private static readonly HashSet<string> NestedActionListFieldNames = new() { "workInBlack" };

    private SerializedProperty actions;

    private void OnEnable()
    {
        actions = serializedObject.FindProperty("actions");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Cutscene Actions", EditorStyles.boldLabel);
        DrawActionList(actions, showAddButton: false);

        if (GUILayout.Button("+ Add Action", GUILayout.Height(24)))
        {
            BuildTypeMenu(actions, -1).ShowAsContext();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawActionList(SerializedProperty listProp, bool showAddButton = true)
    {
        if (listProp == null) return;

        for (int i = 0; i < listProp.arraySize; i++)
        {
            SerializedProperty element = listProp.GetArrayElementAtIndex(i);
            bool isNull = element.managedReferenceValue == null;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();
            element.isExpanded = EditorGUILayout.Foldout(element.isExpanded,
                isNull ? "None (SequenceAction)" : element.managedReferenceValue.GetType().Name, true);
            GUILayout.FlexibleSpace();

            int captured = i;
            bool shouldRefresh = false;
            int moveTarget = -1;

            using (new EditorGUI.DisabledScope(captured == 0))
            {
                if (GUILayout.Button("↑", GUILayout.Width(24)))
                {
                    moveTarget = captured - 1;
                    shouldRefresh = true;
                }
            }

            using (new EditorGUI.DisabledScope(captured >= listProp.arraySize - 1))
            {
                if (GUILayout.Button("↓", GUILayout.Width(24)))
                {
                    moveTarget = captured + 1;
                    shouldRefresh = true;
                }
            }

            if (GUILayout.Button("Type", GUILayout.Width(50)))
                BuildTypeMenu(listProp, captured).ShowAsContext();
            if (GUILayout.Button("-", GUILayout.Width(24)))
            {
                listProp.DeleteArrayElementAtIndex(captured);
                shouldRefresh = true;
            }
            EditorGUILayout.EndHorizontal();

            if (shouldRefresh)
            {
                EditorGUILayout.EndVertical();
                if (moveTarget >= 0)
                    SwapArrayElements(listProp, captured, moveTarget);
                break;
            }



            if (element.isExpanded && !isNull)
            {
                EditorGUILayout.BeginVertical();
                DrawManagedFields(element);
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();
        }

        if (showAddButton && GUILayout.Button("+ Add Nested Action", GUILayout.Height(24)))
        {
            BuildTypeMenu(listProp, -1).ShowAsContext();
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

            if (NestedActionListFieldNames.Contains(iterator.name))
            {
                EditorGUILayout.LabelField(iterator.displayName, EditorStyles.boldLabel);
                DrawActionList(iterator.Copy());
                continue;
            }

            EditorGUILayout.PropertyField(iterator, true);
        }
        EditorGUI.indentLevel--;
    }

    private void SwapArrayElements(SerializedProperty listProp, int indexA, int indexB)
    {
        SerializedProperty elementA = listProp.GetArrayElementAtIndex(indexA);
        SerializedProperty elementB = listProp.GetArrayElementAtIndex(indexB);
        object valueA = elementA.managedReferenceValue;
        elementA.managedReferenceValue = elementB.managedReferenceValue;
        elementB.managedReferenceValue = valueA;
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }

    private GenericMenu BuildTypeMenu(SerializedProperty listProp, int targetIndex)
    {
        GenericMenu menu = new GenericMenu();
        foreach (Type type in GetActionTypes())
        {
            menu.AddItem(new GUIContent(type.Name), false, () =>
            {
                object instance = Activator.CreateInstance(type);
                if (targetIndex >= 0)
                    listProp.GetArrayElementAtIndex(targetIndex).managedReferenceValue = instance;
                else
                {
                    listProp.arraySize++;
                    listProp.GetArrayElementAtIndex(listProp.arraySize - 1).managedReferenceValue = instance;
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
