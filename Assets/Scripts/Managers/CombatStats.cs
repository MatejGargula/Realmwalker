using UnityEngine;

/*

[CreateAssetMenu(fileName = "Data", menuName = "Inventory/List", order = 1)]
public class MyScriptableObjectClass : ScriptableObject
{
    public string objectName = "New MyScriptableObject";
    public bool colorIsRandom = false;
    public Color thisColor = Color.white;
    public Vector3[] spawnPoints;
}
  
  
 */

[CreateAssetMenu(fileName = "Combat Stats", menuName = "Character Combat Stats", order = 1)]
public class CombatStats : ScriptableObject
{
    [SerializeField] public string characterName;
    [SerializeField] public int maxHealth;
    [SerializeField] public int damage;
    [SerializeField] public Sprite portrait;

    [SerializeField] public SkillStats[] skillStats;
    //TODO: include resists, modifiers, buffs etc.


    /// <summary>
    ///     Returns damage of the character with modifiers applied
    /// </summary>
    /// <returns> returns damage (modifiers not yet included ( It's a simple getter ))  </returns>
    public int getDamage()
    {
        return damage;
    }

    /// <summary>
    ///     Returns max health of the character with modifiers applied
    /// </summary>
    /// <returns> returns health (modifiers not yet included ( It's a simple getter ))  </returns>
    public int getHealth()
    {
        return maxHealth;
    }
}