using System.Collections.Generic;
using Realmwalker.Combat;
using UnityEngine;

public class EnemyAIController : MonoBehaviour
{
    #region Serialized fields

    public List<CharacterBase> party;

    #endregion // Serialized fields

    #region Data members

    private EventManager _eventManager;

    #endregion // Data members

    #region Public methods

    public void ChooseNewSkillsToPlay()
    {
        party = CombatManager.Instance.enemyParty;

        foreach (var character in party)
            if (character.TryGetComponent(out CharacterAI characterAI))
                characterAI.ChooseNextSkill();

        _eventManager.OnEnemyTurnEnded();
    }

    #endregion // Public methods

    #region Unity callback methods

    private void Start()
    {
        _eventManager = EventManager.Instance;

        _eventManager.EnemyTurnStartedEvent += ChooseNewSkillsToPlay;
    }

    private void OnDestroy()
    {
        _eventManager.EnemyTurnStartedEvent -= ChooseNewSkillsToPlay;
    }

    #endregion // Unity callback methods
}