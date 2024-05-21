using Realmwalker.UI.Controls;

namespace Realmwalker.Combat
{
    public class QueuedSkill
    {
        #region Constructors

        public QueuedSkill(SkillStats skillStats, CharacterBase origin, CharacterBase target, int order)
        {
            Target = target;
            Origin = origin;
            SkillStats = skillStats;
            SkillOrder = order;

            if (skillStats.speed == SkillSpeed.Slow) origin.usedSlowSkill = true;
        }

        #endregion // Constructors

        #region Data members

        public readonly SkillStats SkillStats;
        public readonly CharacterBase Origin;
        public readonly CharacterBase Target;
        public readonly int SkillOrder;

        private QueuedSkillView _view;

        #endregion // Data members

        #region Public methods

        public void SetView(QueuedSkillView view)
        {
            _view = view;
        }

        public void Play()
        {
            //Debug.Log($"Playing skill: {SkillStats.name} \n Target: {Target.name} \n Origin: {Origin.name}");
            EventManager.Instance.OnSkillPlayed();
            Origin.PlaySkill(SkillOrder, Target);
        }

        #endregion // Public methods
    }
}