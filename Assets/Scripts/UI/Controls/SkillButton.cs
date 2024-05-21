using UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Realmwalker.UI.Controls
{
    public class SkillButton : UIElementBase
    {
        #region Nested class

        public new class UxmlFactory : UxmlFactory<SkillButton, UxmlTraits>
        {
        }

        #endregion // Nested class

        #region Properties

        public SkillStats SkillStats
        {
            get => _skillSkillStats;
            set
            {
                UpdateContent();

                _skillSkillStats = value;
            }
        }

        private int _cooldown;

        public int Cooldown
        {
            get => _cooldown;
            set
            {
                _cooldown = value;
                Debug.Log($"Cooldown: {value}");
                UpdateCooldown();
            }
        }

        public bool Disabled => _cooldown != 0;

        #endregion // Properties

        #region Data members

        private Icon _icon;
        private Label _cooldownLabel;

        private SkillStats _skillSkillStats;
        public SkillDetail Detail;

        private const string HiddenUssClass = "hidden";
        private const string VisibleUssClass = "visible";

        #endregion // Data members

        #region Override methods

        protected override string GetTemplateResourcePath()
        {
            return "Controls/SkillButton";
        }

        protected override void Init()
        {
            _cooldown = 0;

            _cooldownLabel = this.Q<Label>("cooldown-label");
            _icon = this.Q<Icon>("button-icon");

            RegisterCallback<MouseOverEvent>(ShowDetail);
            RegisterCallback<MouseOutEvent>(HideDetail);

            if (_skillSkillStats != null)
                if (_icon != null)
                    _icon.sprite = _skillSkillStats.sprite;

            UpdateCooldown();
        }

        #endregion // Override methods

        #region Private methods

        private void UpdateContent()
        {
            if (_icon != null) _icon.sprite = _skillSkillStats.sprite;
        }

        private void UpdateCooldown()
        {
            if (_cooldownLabel == null)
                return;

            if (_cooldown > 0)
            {
                _icon.style.display = DisplayStyle.None;

                _cooldownLabel.style.display = DisplayStyle.Flex;
                _cooldownLabel.text = _cooldown.ToString();

                pickingMode = PickingMode.Position;

                return;
            }

            _icon.style.display = DisplayStyle.Flex;
            pickingMode = PickingMode.Ignore;
            _cooldownLabel.style.display = DisplayStyle.None;
        }

        private void HideDetail(MouseOutEvent evt)
        {
            if (Detail == null)
                return;
            Detail.AddToClassList(HiddenUssClass);
            Detail.RemoveFromClassList(VisibleUssClass);
        }

        private void ShowDetail(MouseOverEvent evt)
        {
            if (Detail == null)
                return;

            Detail.AddToClassList(VisibleUssClass);
            Detail.RemoveFromClassList(HiddenUssClass);
        }

        #endregion
    }
}