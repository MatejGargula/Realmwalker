using UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Realmwalker.UI.Controls
{
    public class SkillDetail : UIElementBase
    {
        #region Nested class

        public new class UxmlFactory : UxmlFactory<SkillDetail, UxmlTraits>
        {
        }

        #endregion // Nested class

        #region Properties

        public Sprite SpriteIcon
        {
            set
            {
                if (_icon != null) _icon.sprite = value;
            }
        }

        private SkillStats _stats;

        public SkillStats SkillStats
        {
            get => _stats;
            set
            {
                _stats = value;
                DisplaySkillStats(value);
            }
        }

        #endregion // Properties

        #region Data members

        private VisualElement _backgroundPanel;

        private Icon _icon;
        private Label _skillName;
        private Label _skillDescription;
        private Label _damageLabel;
        private Label _numOfAttacksLabel;
        private Label _speedLabel;
        private Label _cooldownLabel;
        private Label _healLabel;

        private Color _healColor;
        private Color _damageColor;

        private const string HealStyle = "heal-stats";
        private const string DamageStyle = "damage-stats";

        #endregion // Data members

        #region Override methods

        protected override string GetTemplateResourcePath()
        {
            return "Controls/SkillDetail";
        }

        protected override void Init()
        {
            _damageColor = new Color(112, 19, 19, 1);
            _healColor = new Color(76, 117, 26, 1);

            _backgroundPanel = this.Q<VisualElement>("detail-root");

            _icon = this.Q<Icon>("skill-icon");
            _skillName = this.Q<Label>("skill-name");
            _skillDescription = this.Q<Label>("skill-description");
            _speedLabel = this.Q<Label>("speed-label");
            _cooldownLabel = this.Q<Label>("cooldown-label");

            _damageLabel = this.Q<Label>("damage-label");
            _numOfAttacksLabel = this.Q<Label>("number-of-attacks-label");
            _healLabel = this.Q<Label>("heal-label");

            _damageLabel.style.display = DisplayStyle.None;
            _numOfAttacksLabel.style.display = DisplayStyle.None;
            _healLabel.style.display = DisplayStyle.None;

            AddToClassList("hidden");

            if (_stats != null)
                DisplaySkillStats(_stats);
        }

        #endregion // Override methods

        #region Private methods

        private void DisplaySkillStats(SkillStats stats)
        {
            if (_icon == null || _skillName == null || _skillDescription == null)
                return;

            _icon.sprite = stats.sprite;
            _skillName.text = stats.skillName;
            _skillDescription.text = stats.description;

            _speedLabel.text = stats.speed.ToString();
            _cooldownLabel.text = stats.cooldown.ToString();

            switch (stats)
            {
                case DamageSkillStats damageSkillStats:
                    DisplayDamageStats(damageSkillStats);
                    break;
                case HealSkillStats healSkillStats:
                    DisplayHealStats(healSkillStats);
                    break;
            }
        }

        private void DisplayHealStats(HealSkillStats healSkillStats)
        {
            _backgroundPanel.RemoveFromClassList(DamageStyle);
            _backgroundPanel.AddToClassList(HealStyle);

            _healLabel.style.display = DisplayStyle.Flex;
            _healLabel.text = healSkillStats.heal.ToString();
        }

        private void DisplayDamageStats(DamageSkillStats damageSkillStats)
        {
            _backgroundPanel.RemoveFromClassList(HealStyle);
            _backgroundPanel.AddToClassList(DamageStyle);

            _damageLabel.style.display = DisplayStyle.Flex;
            _numOfAttacksLabel.style.display = DisplayStyle.Flex;

            _damageLabel.text = damageSkillStats.damage.ToString();
            _numOfAttacksLabel.text = damageSkillStats.numberOfAttacks.ToString();
        }

        #endregion // Private methods
    }
}