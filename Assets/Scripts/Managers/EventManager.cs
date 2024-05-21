using System;
using Realmwalker.Combat;
using UnityEngine;

public sealed class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public event Action<CharacterBase> SelectedCharacterEvent;
    public event Action<CharacterBase> HoverOnCharacterEvent;
    public event Action<CharacterBase> HoverEndOnCharacterEvent;

    public event Action<SkillStats, int> SelectedSkillEvent;
    public event Action<CharacterBase> SelectedTargetEvent;
    public event Action CancelSelectedSkillEvent;
    public event Action<QueuedSkill> AddedSkillToPlayerQueueEvent;

    public event Action EnemyTurnStartedEvent;
    public event Action EnemyTurnEndedEvent;
    public event Action<QueuedSkill> EnemySelectedTargetEvent;

    public event Action OnTurnEndedEvent;
    public event Action SkillPlayedEvent;
    public event Action SkillEndedEvent;

    public event Action<CombatState> CombatStateChangedEvent;
    public event Action TurnEndEvent;

    public void OnSelectedCharacter(CharacterBase character)
    {
        SelectedCharacterEvent?.Invoke(character);
    }

    public void OnSelectedSkill(SkillStats obj, int order)
    {
        SelectedSkillEvent?.Invoke(obj, order);
    }

    public void OnCombatStateChanged(CombatState state)
    {
        CombatStateChangedEvent?.Invoke(state);
    }

    public void OnTurnEnd()
    {
        TurnEndEvent?.Invoke();
    }

    public void OnSelectedTarget(CharacterBase targetCharacter)
    {
        SelectedTargetEvent?.Invoke(targetCharacter);
    }

    public void OnAddedSkillToPlayerQueue(QueuedSkill skill)
    {
        AddedSkillToPlayerQueueEvent?.Invoke(skill);
    }

    public void OnSkillEnded()
    {
        SkillEndedEvent?.Invoke();
    }

    public void OnSkillPlayed()
    {
        SkillPlayedEvent?.Invoke();
    }

    public void OnOnTurnEnded()
    {
        OnTurnEndedEvent?.Invoke();
    }

    public void OnHoverOnCharacter(CharacterBase character)
    {
        HoverOnCharacterEvent?.Invoke(character);
    }

    public void OnHoverEndOnCharacter(CharacterBase character)
    {
        HoverEndOnCharacterEvent?.Invoke(character);
    }

    public void OnCancelSelectedSkill()
    {
        CancelSelectedSkillEvent?.Invoke();
    }

    public void OnEnemyTurnStarted()
    {
        EnemyTurnStartedEvent?.Invoke();
    }

    public void OnEnemyTurnEnded()
    {
        EnemyTurnEndedEvent?.Invoke();
    }

    public void OnEnemySelectedTarget(QueuedSkill skill)
    {
        EnemySelectedTargetEvent?.Invoke(skill);
    }
}