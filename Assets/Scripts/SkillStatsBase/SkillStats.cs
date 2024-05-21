using UnityEngine;

public enum SkillType
{
    Attack, // red cross sprite
    Defend, // shield sprite
    Buff, // arrows pointing up and plus sign
    Debuff // arrows pointing down and minus sign
}

public enum SkillSpeed
{
    Fast = 1,
    Medium = 2,
    Slow = 3
}

public abstract class SkillStats : ScriptableObject
{
    [SerializeField] public string skillName;
    [TextArea] [SerializeField] public string description;
    [SerializeField] public Sprite sprite;
    [SerializeField] public SkillSpeed speed = SkillSpeed.Fast;
    [SerializeField] public int cooldown;
    [SerializeField] public GameObject skillPrefab;
    [SerializeField] public SkillType type;
    [SerializeField] public int rangeOfAreaEffectLeft;
    [SerializeField] public int rangeOfAreaEffectRight;
}