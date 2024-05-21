using System.Collections.Generic;
using BT;
using Realmwalker.Combat;
using UnityEngine;

public class CharacterAI : MonoBehaviour, IBTAgent
{
    #region Unity callbacks

    private void Awake()
    {
        _character = GetComponent<CharacterBase>();
        tree = tree.Clone(this);
    }

    #endregion // Unity callbacks

    #region Node methods

    public Node.State ChooseSkillOnUpdate(int skillNumber)
    {
        _selectedSkill = skillNumber;
        _selectedSkill = Random.Range(0, 3);
        _isChoosingSkill = false;
        return Node.State.Success;
    }

    #endregion // Node methods

    #region Public methods

    public void ChooseNextSkill()
    {
        _isChoosingSkill = true;

        _currentSkills = _character.GetAllSkillStats();
        var availableTargets = CombatManager.Instance.GetAvailablePlayerTargets();

        RunBT();

        var selectedSkill = SelectSkillToPlay();
        var target = SelectTarget(availableTargets);

        var skill = new QueuedSkill(selectedSkill.Item1, _character, target, selectedSkill.Item2);

        EventManager.Instance.OnEnemySelectedTarget(skill);
    }

    #endregion // Public methods

    #region Data members

    public BehaviourTree tree;

    private CharacterBase _character;
    private List<SkillStats> _currentSkills;

    private bool _isChoosingSkill;
    private int _selectedSkill;

    #endregion

    #region Properties

    public GameObject Go { get; }

    public BehaviourTree Tree
    {
        get => tree;
        set => tree = value;
    }

    #endregion // Properties

    #region Private methods

    private (SkillStats, int) SelectSkillToPlay()
    {
        var skillIdx = _selectedSkill;

        return (_currentSkills[skillIdx], skillIdx);
    }

    private CharacterBase SelectTarget(List<CharacterBase> availableTargets)
    {
        var targetIdx = Random.Range(0, availableTargets.Count);

        return availableTargets[targetIdx];
    }

    private void RunBT()
    {
        while (_isChoosingSkill) Tree.Update();
    }

    #endregion // Private methods
}