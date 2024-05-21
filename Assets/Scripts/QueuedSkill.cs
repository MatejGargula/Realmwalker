using UnityEngine;
using UnityEngine.UI;

namespace Realmwalker.Combat
{
    public class OldQueuedSkill : MonoBehaviour
    {
        public CombatCharacter origin;
        public int skillNumber;
        public CombatCharacter target;
        public TargeterType type;
        public int speed;
        [SerializeField] private GameObject spriteObject;
        [SerializeField] private GameObject portraitObject;

        public void Init()
        {
            spriteObject = gameObject.transform.GetChild(0).gameObject;
            spriteObject.GetComponent<Image>().sprite = origin.stats.skillStats[skillNumber - 1].sprite;
            portraitObject = gameObject.transform.GetChild(1).gameObject;
            portraitObject.GetComponent<Image>().sprite = origin.stats.portrait;
        }

        public void PlaySkill()
        {
            if (target != null)
                origin.UseSkill(skillNumber, target);
            else
                Debug.Log("Yo this skill doesn't have a target");
            //Destroy(gameObject);
        }

        public void CopyQueuedSkill(OldQueuedSkill skill)
        {
            origin = skill.origin;
            skillNumber = skill.skillNumber;
            speed = skill.speed;
            target = skill.target;
        }
    }
}