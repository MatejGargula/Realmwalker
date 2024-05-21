using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realmwalker.Combat
{
    public class HealSkill : MonoBehaviour, ISkill
    {
        #region Public methods

        public void Use(CharacterBase target)
        {
            var targets = GetAllTargets(target);

            foreach (var character in targets) StartCoroutine(UseCourutine(character));
        }

        #endregion // Public methods

        #region Serialized fields

        [SerializeField] private GameObject healEffect;
        [SerializeField] private float duration;
        [SerializeField] private float hitTimeOffset;

        #endregion // Serialized fields

        #region Properties

        private HealSkillStats _stats;

        public SkillStats Stats
        {
            get => _stats;
            set => _stats = value as HealSkillStats;
        }

        public CharacterBase Source { get; set; }
        public SkillSpeed Speed { get; set; }

        #endregion // Properties

        #region Private methods

        private IEnumerator UseCourutine(CharacterBase target)
        {
            if (target == null) EndSkill();

            StartCoroutine(Hit(target, hitTimeOffset));
            var effect = Instantiate(healEffect, target.transform.position, Quaternion.identity);
            Destroy(effect, duration);

            // Add audio

            yield return new WaitForSeconds(duration + 0.5f);

            EndSkill();
        }

        private IEnumerator Hit(CharacterBase target, float time)
        {
            yield return new WaitForSeconds(time);
            PopUpGenerator.Instance.CreateDamagePopUp(target.transform.position, _stats.heal.ToString(), Color.green);
            target.Heal(_stats.heal);
        }

        private void EndSkill()
        {
            Debug.Log("Ending Skill");
            EventManager.Instance.OnSkillEnded();
            gameObject.SetActive(false);
        }

        private List<CharacterBase> GetAllTargets(CharacterBase target)
        {
            List<CharacterBase> targets = new() { target };

            List<CharacterBase> targetParty;
            if (target.isPlayer)
                targetParty = CombatManager.Instance.playerParty;
            else
                targetParty = CombatManager.Instance.enemyParty;

            for (var i = 0; i < targetParty.Count; i++)
                if (targetParty[i] == target)
                {
                    var j = i;
                    while (i - Stats.rangeOfAreaEffectLeft <= j)
                    {
                        j--;
                        if (j > 0 && j < targetParty.Count)
                            targets.Add(targetParty[j]);
                    }

                    j = i;
                    while (i + Stats.rangeOfAreaEffectLeft >= j)
                    {
                        j++;
                        if (j > 0 && j < targetParty.Count)
                            targets.Add(targetParty[j]);
                    }

                    break;
                }

            return targets;
        }

        #endregion // Private methods
    }
}