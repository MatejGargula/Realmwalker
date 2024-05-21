using System;
using System.Collections.Generic;
using Realmwalker.Combat;
using UnityEngine;

public enum CharacterStatus
{
}

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CharacterSelector))]
public class CharacterBase : MonoBehaviour
{
    #region Properties

    public int Health
    {
        get => health;
        set
        {
            health = value;
            UpdateHealth(health);
        }
    }

    #endregion // Properties

    #region Serialized field

    [SerializeField] public CombatStats stats;
    [SerializeField] private List<SkillStats> selectedSkills;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int health = 100;
    [SerializeField] private int damage = 10;
    [SerializeField] private bool isAlive = true;
    [SerializeField] public bool isPlayer;

    [Header("Support objects")] [SerializeField]
    private HealthBar healthBar;

    #endregion // Serialized field

    #region Data members

    private readonly List<(GameObject, ISkill)> _skills = new();
    private readonly List<int> _skillCooldowns = new();

    public event Action<CharacterStatus> characterApplyStatus;

    #endregion

    #region Data members

    private CharacterSelector _characterSelector;
    private SpriteRenderer _renderer;

    public bool usedSlowSkill;

    #endregion // Data members

    #region Unity callback methods

    private void Awake()
    {
        _characterSelector = GetComponent<CharacterSelector>();
        _renderer = GetComponent<SpriteRenderer>();

        PrepareSkills();
    }

    private void Start()
    {
        maxHealth = stats.maxHealth;

        healthBar = GetComponentInChildren<HealthBar>();
        healthBar.SetUpHealthBar(maxHealth);

        healthBar.UpdateHealth(health);

        EventManager.Instance.EnemyTurnEndedEvent += EnableSelection;
        EventManager.Instance.TurnEndEvent += OnTurnEndEvent;
        EventManager.Instance.AddedSkillToPlayerQueueEvent += OnAddedSkillToPlayerQueue;
    }

    private void OnDestroy()
    {
        EventManager.Instance.EnemyTurnEndedEvent -= EnableSelection;
        EventManager.Instance.TurnEndEvent -= OnTurnEndEvent;
        EventManager.Instance.AddedSkillToPlayerQueueEvent -= OnAddedSkillToPlayerQueue;
    }

    #endregion // Unity callback methods

    #region Public methods

    public SkillStats GetSkillStats(int skillNumber)
    {
        return selectedSkills[skillNumber];
    }

    public List<SkillStats> GetAllSkillStats()
    {
        return selectedSkills;
    }

    public void PlaySkill(int skillNumber, CharacterBase target)
    {
        _skills[skillNumber].Item1.SetActive(true);
        _skills[skillNumber].Item2.Use(target);
        _skillCooldowns[skillNumber] = selectedSkills[skillNumber].cooldown;
        if (_skills[skillNumber].Item2.Speed == SkillSpeed.Slow) usedSlowSkill = false;
    }

    public List<int> GetCurrentCooldowns()
    {
        return _skillCooldowns;
    }

    public void DealDamage(int damage)
    {
        Health -= damage;
    }

    public void Heal(int healAmount)
    {
        Health += healAmount;
    }

    public void DisableSelection()
    {
        _characterSelector.DisableSelection();
        _renderer.color = Color.gray;
    }

    public void EnableSelection()
    {
        if (!usedSlowSkill)
        {
            _characterSelector.EnableSelection();
            _renderer.color = Color.white;
        }
    }

    #endregion // Public methods

    #region Private methods

    private void OnAddedSkillToPlayerQueue(QueuedSkill skill)
    {
        if (skill.Origin == this) _skillCooldowns[skill.SkillOrder] = selectedSkills[skill.SkillOrder].cooldown;
    }

    private void UpdateHealth(int newHealth)
    {
        if (health < 0)
        {
            health = 0;
            isAlive = false;
            Die();
        }

        if (health > maxHealth) health = maxHealth;

        healthBar.UpdateHealth(newHealth);
    }

    private void Die()
    {
        gameObject.SetActive(false);
    }

    private void PrepareSkills()
    {
        foreach (var selectedSkill in selectedSkills)
        {
            var skillObject = Instantiate(selectedSkill.skillPrefab, transform);
            var skill = skillObject.GetComponent<ISkill>();
            skill.Stats = selectedSkill;
            skill.Source = this;
            skill.Speed = selectedSkill.speed;
            _skills.Add((skillObject, skill));
            skillObject.SetActive(false);

            _skillCooldowns.Add(0);
        }
    }

    protected virtual void OnCharacterApplyStatus(CharacterStatus status)
    {
        characterApplyStatus?.Invoke(status);
    }

    private void OnTurnEndEvent()
    {
        for (var i = 0; i < _skillCooldowns.Count; i++)
            if (_skillCooldowns[i] != 0)
                _skillCooldowns[i] -= 1;
    }

    #endregion // Private methods
}