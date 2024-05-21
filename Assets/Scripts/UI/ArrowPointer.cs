using System;
using Realmwalker.Combat;
using UnityEngine;

public class ArrowPointer : MonoBehaviour
{
    #region Serialized fields

    [SerializeField] private GameObject arrowPointer;
    [SerializeField] private GameObject attackPointer;
    [SerializeField] private GameObject defendPointer;
    [SerializeField] private GameObject buffPointer;
    [SerializeField] private GameObject debuffPointer;

    #endregion // Serialized fields

    #region Unity callback methods

    private void Awake()
    {
        arrowPointer.SetActive(false);
        attackPointer.SetActive(false);
        defendPointer.SetActive(false);
        buffPointer.SetActive(false);
        debuffPointer.SetActive(false);
    }

    private void Start()
    {
        EventManager.Instance.SelectedCharacterEvent += OnSelectedCharacter;
        EventManager.Instance.HoverOnCharacterEvent += OnHoverOnCharacter;
        EventManager.Instance.SelectedTargetEvent += OnSelectedTarget;
        EventManager.Instance.HoverEndOnCharacterEvent += OnHoverEndOnCharacter;
        EventManager.Instance.SelectedSkillEvent += OnSelectedSkill;
        EventManager.Instance.TurnEndEvent += Hide;
    }

    private void OnDestroy()
    {
        EventManager.Instance.SelectedCharacterEvent -= OnSelectedCharacter;
        EventManager.Instance.HoverOnCharacterEvent -= OnHoverOnCharacter;
        EventManager.Instance.SelectedTargetEvent -= OnSelectedTarget;
        EventManager.Instance.HoverEndOnCharacterEvent -= OnHoverEndOnCharacter;
        EventManager.Instance.SelectedSkillEvent -= OnSelectedSkill;
        EventManager.Instance.TurnEndEvent -= Hide;
    }

    #endregion // Unity callback methods

    #region Private methods

    private void OnHoverOnCharacter(CharacterBase character)
    {
        if (CombatManager.Instance.State == CombatState.SkillPick && !character.gameObject.CompareTag("Player"))
            return;

        if (CombatManager.Instance.State == CombatState.SkillPick)
            ShowArrowPointer();

        if (CombatManager.Instance.State == CombatState.TargetPick)
            ShowTargetPointer(CombatManager.Instance.selectedSkillStats.type);

        transform.position = character.transform.position;
    }

    private void OnHoverEndOnCharacter(CharacterBase character)
    {
        if (CombatManager.Instance.State == CombatState.SkillPick && CombatManager.Instance.selectedCharacter != null)
        {
            ResetArrowPointer();
            return;
        }

        Hide();
    }

    private void OnSelectedCharacter(CharacterBase character)
    {
        ShowArrowPointer();
        transform.position = character.transform.position;
    }

    private void ShowArrowPointer()
    {
        arrowPointer.SetActive(true);

        attackPointer.SetActive(false);
        defendPointer.SetActive(false);
        buffPointer.SetActive(false);
        debuffPointer.SetActive(false);
    }

    private void ShowTargetPointer(SkillType skillType)
    {
        arrowPointer.SetActive(false);

        switch (skillType)
        {
            case SkillType.Attack:
                attackPointer.SetActive(true);
                defendPointer.SetActive(false);
                buffPointer.SetActive(false);
                debuffPointer.SetActive(false);
                break;
            case SkillType.Defend:
                attackPointer.SetActive(false);
                defendPointer.SetActive(true);
                buffPointer.SetActive(false);
                debuffPointer.SetActive(false);
                break;
            case SkillType.Buff:
                attackPointer.SetActive(false);
                defendPointer.SetActive(false);
                buffPointer.SetActive(true);
                debuffPointer.SetActive(false);
                break;
            case SkillType.Debuff:
                attackPointer.SetActive(false);
                defendPointer.SetActive(false);
                buffPointer.SetActive(false);
                debuffPointer.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(skillType), skillType, null);
        }
    }

    private void Hide()
    {
        arrowPointer.SetActive(false);
        attackPointer.SetActive(false);
        defendPointer.SetActive(false);
        buffPointer.SetActive(false);
        debuffPointer.SetActive(false);
    }

    private void ResetArrowPointer()
    {
        ShowArrowPointer();

        var character = CombatManager.Instance.selectedCharacter;
        if (character != null)
        {
            transform.position = character.transform.position;
            return;
        }

        Hide();
    }

    private void OnSelectedTarget(CharacterBase character)
    {
        //ResetArrowPointer();
        Hide();
    }

    private void OnSelectedSkill(SkillStats stats, int order)
    {
        Hide();
    }

    #endregion // Private methods
}