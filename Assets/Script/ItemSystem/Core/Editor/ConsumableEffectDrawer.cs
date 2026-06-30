using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EffectData))]
public class ConsumableEffectDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float y = position.y;

        Rect foldoutRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true);
        y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;

            SerializedProperty effectType = property.FindPropertyRelative("effectType");
            SerializedProperty duration = property.FindPropertyRelative("duration");
            SerializedProperty value = property.FindPropertyRelative("value");
            SerializedProperty tickInterval = property.FindPropertyRelative("tickInterval");
            SerializedProperty tickValue = property.FindPropertyRelative("tickValue");
            SerializedProperty isDebuff = property.FindPropertyRelative("isDebuff");
            SerializedProperty statModifiers = property.FindPropertyRelative("statModifiers");

            y = DrawProperty(position, y, effectType);
            EffectDataType type = (EffectDataType)effectType.enumValueIndex;

            if (HasDuration(type))
                y = DrawProperty(position, y, duration);

            if (HasValue(type))
                y = DrawProperty(position, y, value);

            if (HasTick(type))
                y = DrawProperty(position, y, tickInterval);

            if (type == EffectDataType.Heal)
                y = DrawProperty(position, y, tickValue);

            if (type == EffectDataType.StatModifier)
            {
                y = DrawProperty(position, y, isDebuff);
                y = DrawProperty(position, y, statModifiers);
            }

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    private float DrawProperty(Rect position, float y, SerializedProperty prop)
    {
        float height = EditorGUI.GetPropertyHeight(prop);
        EditorGUI.PropertyField(new Rect(position.x, y, position.width, height), prop);
        return y + height + EditorGUIUtility.standardVerticalSpacing;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!property.isExpanded)
            return EditorGUIUtility.singleLineHeight;

        float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        SerializedProperty effectType = property.FindPropertyRelative("effectType");
        SerializedProperty duration = property.FindPropertyRelative("duration");
        SerializedProperty value = property.FindPropertyRelative("value");
        SerializedProperty tickInterval = property.FindPropertyRelative("tickInterval");
        SerializedProperty tickValue = property.FindPropertyRelative("tickValue");
        SerializedProperty isDebuff = property.FindPropertyRelative("isDebuff");
        SerializedProperty statModifiers = property.FindPropertyRelative("statModifiers");

        height += EditorGUI.GetPropertyHeight(effectType) + EditorGUIUtility.standardVerticalSpacing;
        EffectDataType type = (EffectDataType)effectType.enumValueIndex;

        if (HasDuration(type))
            height += EditorGUI.GetPropertyHeight(duration) + EditorGUIUtility.standardVerticalSpacing;

        if (HasValue(type))
            height += EditorGUI.GetPropertyHeight(value) + EditorGUIUtility.standardVerticalSpacing;

        if (HasTick(type))
            height += EditorGUI.GetPropertyHeight(tickInterval) + EditorGUIUtility.standardVerticalSpacing;

        if (type == EffectDataType.Heal)
            height += EditorGUI.GetPropertyHeight(tickValue) + EditorGUIUtility.standardVerticalSpacing;

        if (type == EffectDataType.StatModifier)
        {
            height += EditorGUI.GetPropertyHeight(isDebuff) + EditorGUIUtility.standardVerticalSpacing;
            height += EditorGUI.GetPropertyHeight(statModifiers) + EditorGUIUtility.standardVerticalSpacing;
        }

        return height;
    }

    private static bool HasDuration(EffectDataType type) => type switch
    {
        EffectDataType.None or EffectDataType.Cleanse => false,
        _ => true
    };

    private static bool HasValue(EffectDataType type) => type switch
    {
        EffectDataType.Heal or EffectDataType.DamageOverTime => true,
        _ => false
    };

    private static bool HasTick(EffectDataType type) => type switch
    {
        EffectDataType.Heal or EffectDataType.DamageOverTime => true,
        _ => false
    };
}
