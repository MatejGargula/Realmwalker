using System.Collections.Generic;

namespace BT.Editor.Views.BlackboardView
{
    public interface IBlackboardList<T>
    {
        List<Blackboard.BlackboardProperty<T>> ListSource { get; set; }

        public void RefreshList();

        public void Hide();

        public void Show();
    }
}