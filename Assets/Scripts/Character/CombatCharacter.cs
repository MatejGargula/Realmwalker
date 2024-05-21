using UnityEngine;

namespace Realmwalker.Combat
{
    /// <summary>
    ///     Base class for the charaters for the combat. Use this class when creating a new character for the combat
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class CombatCharacter : MonoBehaviour
    {
        // array of cooldowns for each character. Initialize this after every turn.  
        [SerializeField] public CombatStats stats;
        [SerializeField] private int health = 100;
        [SerializeField] private int damage = 10;

        [SerializeField] private GameObject[] skillPrefabs;
        // Implement each of these functions for each player
        // These functions  will be called by the combat manager

        // use null for target if the character targets itself
        public bool debugOn = true;

        private void OnMouseDown()
        {
            Debug.Log($"You have clicked on :{gameObject.name}");
        }

        public void UseSkill(int skillNumber, CombatCharacter target)
        {
            if (skillPrefabs.Length < skillNumber)
            {
                GameManager.Instance.logger.Log(debugOn, "Error [Character UseSkill method]: Skill is null!");
                return;
            }

            var skill = Instantiate(skillPrefabs[skillNumber - 1], target.transform.position,
                Quaternion.identity);
            //skill.GetComponent<ISkill>().Source = this;
            //skill.GetComponent<ISkill>().Use(target);
            //GameManager.Instance.logger.Log(debugOn, stats.characterName + " is using skill " + stats.skillStats[skillNumber - 1].skillName + " ( " + skillNumber + " ) on a target " + target.stats.characterName);
        }

        public void DealDamage(int damage)
        {
            health -= damage;
            if (health <= 0) Destroy(gameObject);
        }

        public void SetDamage(int newDamage)
        {
            damage = newDamage;
        }

        public int getDamage()
        {
            return damage;
        }
    }
}