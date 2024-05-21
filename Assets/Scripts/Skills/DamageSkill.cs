using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realmwalker.Combat
{
    public class DamageSkill : MonoBehaviour, ISkill
    {
        [SerializeField] private GameObject damageEffect;
        [SerializeField] private float attackInterval;
        [SerializeField] private float hitTimeOffset;
        [SerializeField] private float cameraShakeTime;
        [SerializeField] private float cameraShakeAmplitude;

        public bool DebugOn = true;

        private DamageSkillStats _stats;

        private void OnDestroy()
        {
            // Call the  queue to play the next skill
        }

        public SkillSpeed Speed { get; set; }

        public SkillStats Stats
        {
            get => _stats;
            set => _stats = value as DamageSkillStats;
        }

        public CharacterBase Source { get; set; }

        public void Use(CharacterBase target)
        {
            var targets = GetAllTargets(target);

            foreach (var character in targets) StartCoroutine(UseCourutine(character));
        }

        private IEnumerator UseCourutine(CharacterBase target)
        {
            for (var i = 0; i < _stats.numberOfAttacks; i++)
            {
                if (target == null)
                {
                    EndSkill();
                    break;
                }

                StartCoroutine(Hit(target, hitTimeOffset));

                var effect = Instantiate(damageEffect, target.transform.position, Quaternion.identity);
                Destroy(effect, attackInterval);
                // Add audio

                yield return new WaitForSeconds(attackInterval);
            }

            yield return new WaitForSeconds(0.5f);

            EndSkill();
        }

        private IEnumerator Hit(CharacterBase target, float time)
        {
            yield return new WaitForSeconds(time);
            CameraEffectManager.Instance.ShakeCamera(cameraShakeAmplitude, cameraShakeTime);
            PopUpGenerator.Instance.CreateDamagePopUp(target.transform.position, _stats.damage.ToString(), Color.red);
            target.DealDamage(_stats.damage);
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
                    while (i - _stats.rangeOfAreaEffectLeft <= j)
                    {
                        j--;
                        if (j > -1 && j < targetParty.Count)
                            targets.Add(targetParty[j]);
                    }

                    j = i;
                    while (i + _stats.rangeOfAreaEffectLeft >= j)
                    {
                        j++;
                        if (j > -1 && j < targetParty.Count)
                            targets.Add(targetParty[j]);
                    }

                    break;
                }

            return targets;
        }
    }
}