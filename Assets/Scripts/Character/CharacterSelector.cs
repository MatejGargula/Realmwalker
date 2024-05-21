using Realmwalker.Combat;
using UnityEngine;

[RequireComponent(typeof(CharacterBase))]
[RequireComponent(typeof(Collider))]
public class CharacterSelector : MonoBehaviour
{
    #region Unity callback methods

    private void Awake()
    {
        _character = GetComponent<CharacterBase>();
        _collider = GetComponent<Collider>();
    }

    #endregion // Unity callback methods

    #region Data members

    private CharacterBase _character;
    private Collider _collider;

    private bool _selectionEnabled = true;

    #endregion // Data members

    #region Public methods

    public void EnableSelection()
    {
        _selectionEnabled = true;
    }

    public void DisableSelection()
    {
        _selectionEnabled = false;
    }

    #endregion // Public methods

    #region Private methods

    private void OnMouseDown()
    {
        if (CombatManager.Instance.State == CombatState.TargetPick)
        {
            EventManager.Instance.OnSelectedTarget(_character);
            // CombatManager.Instance.SetTarget(this);
            return;
        }

        if (_selectionEnabled)
            EventManager.Instance.OnSelectedCharacter(_character);
        // CombatManager.Instance.SelectCharacter(this);
        //Debug.Log($"You have clicked on {gameObject.name}");
    }

    private void OnMouseEnter()
    {
        if (CombatManager.Instance.State is CombatState.EnemyTurn or CombatState.TurnEnded)
            return;

        if (CombatManager.Instance.State is CombatState.TargetPick)
            EventManager.Instance.OnHoverOnCharacter(_character);

        if (CombatManager.Instance.State is CombatState.SkillPick && _selectionEnabled)
            EventManager.Instance.OnHoverOnCharacter(_character);
    }

    private void OnMouseExit()
    {
        if (CombatManager.Instance.State is CombatState.EnemyTurn or CombatState.TurnEnded)
            return;

        if (CombatManager.Instance.State is CombatState.TargetPick)
            EventManager.Instance.OnHoverEndOnCharacter(_character);

        if (CombatManager.Instance.State is CombatState.SkillPick && _selectionEnabled)
            EventManager.Instance.OnHoverEndOnCharacter(_character);
    }

    #endregion // Private methods
}