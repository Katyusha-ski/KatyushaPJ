using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ConsumableEffect))]
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
            ConsumableEffectType type = (ConsumableEffectType)effectType.enumValueIndex;

            if (HasDuration(type))
                y = DrawProperty(position, y, duration);

            if (HasValue(type))
                y = DrawProperty(position, y, value);

            if (HasTick(type))
                y = DrawProperty(position, y, tickInterval);

            if (type == ConsumableEffectType.Heal)
                y = DrawProperty(position, y, tickValue);

            if (type == ConsumableEffectType.StatModifier)
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
        ConsumableEffectType type = (ConsumableEffectType)effectType.enumValueIndex;

        if (HasDuration(type))
            height += EditorGUI.GetPropertyHeight(duration) + EditorGUIUtility.standardVerticalSpacing;

        if (HasValue(type))
            height += EditorGUI.GetPropertyHeight(value) + EditorGUIUtility.standardVerticalSpacing;

        if (HasTick(type))
            height += EditorGUI.GetPropertyHeight(tickInterval) + EditorGUIUtility.standardVerticalSpacing;

        if (type == ConsumableEffectType.Heal)
            height += EditorGUI.GetPropertyHeight(tickValue) + EditorGUIUtility.standardVerticalSpacing;

        if (type == ConsumableEffectType.StatModifier)
        {
            height += EditorGUI.GetPropertyHeight(isDebuff) + EditorGUIUtility.standardVerticalSpacing;
            height += EditorGUI.GetPropertyHeight(statModifiers) + EditorGUIUtility.standardVerticalSpacing;
        }

        return height;
    }

    private static bool HasDuration(ConsumableEffectType type) => type switch
    {
        ConsumableEffectType.None or ConsumableEffectType.Cleanse => false,
        _ => true
    };

    private static bool HasValue(ConsumableEffectType type) => type switch
    {
        ConsumableEffectType.Heal or ConsumableEffectType.DamageOverTime => true,
        _ => false
    };

    private static bool HasTick(ConsumableEffectType type) => type switch
    {
        ConsumableEffectType.Heal or ConsumableEffectType.DamageOverTime => true,
        _ => false
    };
}
