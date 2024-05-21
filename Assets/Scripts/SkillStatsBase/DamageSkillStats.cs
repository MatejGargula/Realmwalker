using UnityEngine;

[CreateAssetMenu(fileName = "Damage skill", menuName = "SkillStats/Damage skill stats", order = 1)]
public class DamageSkillStats : SkillStats
{
    public int damage;
    public int numberOfAttacks;
}