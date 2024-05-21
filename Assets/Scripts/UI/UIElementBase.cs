using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public abstract class UIElementBase : VisualElement
    {
        private readonly StyleEnum<DisplayStyle> _oldStyle;

        #region Constructor

        public UIElementBase()
        {
            _oldStyle = style.display;
            style.display = DisplayStyle.Flex;
            RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
        }

        #endregion // Constructor

        #region Private methods

        private void OnGeometryChange(GeometryChangedEvent evt)
        {
            var template = Resources.Load<VisualTreeAsset>(GetTemplateResourcePath());

            if (template == null)
                throw new InvalidOperationException("Cannot find template for custom control.");

            Clear();
            Add(template.Instantiate());

            Init();
            style.display = _oldStyle;
            UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
        }

        #endregion // Private methods

        #region Abstract methods

        protected abstract string GetTemplateResourcePath();

        protected abstract void Init();

        #endregion // Abstract methods
    }
}