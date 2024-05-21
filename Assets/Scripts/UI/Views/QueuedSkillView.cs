using Realmwalker.Combat;
using UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Realmwalker.UI.Controls
{
    public class QueuedSkillView : UIElementBase
    {
        #region Public methods

        public void SetSkillData(QueuedSkill skillData)
        {
            _skillData = skillData;

            _skillSprite = _skillData.SkillStats.sprite;
            _characterOriginSprite = _skillData.Origin.stats.portrait;
            _characterTargetSprite = _skillData.Origin.stats.portrait;
            if (_skillData.Target != null) _characterTargetSprite = _skillData.Target.stats.portrait;

            Init();
        }

        #endregion // Public methods

        #region Nested class

        public new class UxmlFactory : UxmlFactory<QueuedSkillView, UxmlTraits>
        {
        }

        #endregion // Nested class

        #region Data members

        private Icon _skillIcon;
        private Icon _characterOriginIcon;
        private Icon _characterTargetIcon;

        private Sprite _skillSprite;
        private Sprite _characterOriginSprite;
        private Sprite _characterTargetSprite;

        private QueuedSkill _skillData;

        #endregion // Data members

        #region Constructors

        public QueuedSkillView()
        {
        }

        public QueuedSkillView(QueuedSkill skillData)
        {
            _skillData = skillData;

            _skillSprite = _skillData.SkillStats.sprite;
            _characterOriginSprite = _skillData.Origin.stats.portrait;
            _characterTargetSprite = _skillData.Origin.stats.portrait;
            if (_skillData.Target != null) _characterTargetSprite = _skillData.Target.stats.portrait;
        }

        #endregion // Constructors

        #region Override methods

        protected override string GetTemplateResourcePath()
        {
            return "Controls/QueuedSkill";
        }

        protected override void Init()
        {
            _skillIcon = this.Q<Icon>("queued-skill-icon");
            _characterOriginIcon = this.Q<Icon>("source");
            _characterTargetIcon = this.Q<Icon>("target");

            if (_skillData.SkillStats != null && _skillIcon != null) _skillIcon.sprite = _skillSprite;
            if (_characterOriginSprite != null && _characterOriginIcon != null)
                _characterOriginIcon.sprite = _characterOriginSprite;
            if (_characterTargetSprite != null && _characterTargetIcon != null)
                _characterTargetIcon.sprite = _characterTargetSprite;
        }

        #endregion // Override methods
    }
}