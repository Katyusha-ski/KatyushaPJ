using UnityEngine;

[CreateAssetMenu(menuName = "Config/InputConfig")]
public class InputConfig : ScriptableObject
{
    [Header("Movement")]
    public string horizontalAxis = "Horizontal";
    public KeyCode runKey = KeyCode.LeftShift;
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Skills")]
    public KeyCode skill1Key = KeyCode.E;
    public KeyCode skill2Key = KeyCode.Q;
    public KeyCode skill3Key = KeyCode.R;
    public KeyCode skill4Key = KeyCode.F;

    [Header("Other Actions")]
    public KeyCode pauseKey = KeyCode.Escape;

    public static InputConfig GetDefault()
    {
        var config = CreateInstance<InputConfig>();
        config.horizontalAxis = "Horizontal";
        config.runKey = KeyCode.LeftShift;
        config.jumpKey = KeyCode.Space;
        config.skill1Key = KeyCode.E;
        config.skill2Key = KeyCode.Q;
        config.skill3Key = KeyCode.R;
        config.skill4Key = KeyCode.F;
        config.pauseKey = KeyCode.Escape;
        return config;
    }

}