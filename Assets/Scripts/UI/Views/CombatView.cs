using System;
using System.Collections.Generic;
using Realmwalker.Combat;
using Realmwalker.UI.Controls;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class CombatView : MonoBehaviour
{
    #region Data members

    private UIDocument _uiDocument;
    private VisualElement _root;
    private EventManager _eventManager;

    private readonly List<(SkillButton, SkillDetail, int)> _skillButtons = new();

    private readonly List<ListView> _playerQueue = new();
    private readonly List<ListView> _enemyQueue = new();
    private Icon _characterIcon;
    private Label _characterNameLabel;
    private Button _endTurnButton;

    private VisualElement _bottomButtonPanel;
    private VisualElement _portraitPanel;

    private VisualElement _cancelButton;

    private const string HiddenUssClass = "hidden";
    private const string VisibleUssClass = "visible";

    #endregion // Data members

    #region Unity callback methods

    private void Start()
    {
        _eventManager = EventManager.Instance;

        _eventManager.SelectedCharacterEvent += OnSelectCharacter;
        _eventManager.AddedSkillToPlayerQueueEvent += OnAddedSkillToPlayerQueue;
        _eventManager.TurnEndEvent += OnTurnEnded;
        _eventManager.SkillPlayedEvent += OnSkillPlayed;
        _eventManager.SkillEndedEvent += OnSkillEnded;
        _eventManager.CombatStateChangedEvent += OnCombatStateChanged;

        InitButtons();
        InitQueues();
    }

    private void OnDestroy()
    {
        _eventManager.SelectedCharacterEvent -= OnSelectCharacter;
        _eventManager.AddedSkillToPlayerQueueEvent -= OnAddedSkillToPlayerQueue;
        _eventManager.TurnEndEvent -= OnTurnEnded;
        _eventManager.SkillPlayedEvent -= OnSkillPlayed;
        _eventManager.SkillEndedEvent -= OnSkillEnded;
        _eventManager.CombatStateChangedEvent -= OnCombatStateChanged;
    }

    private void OnEnable()
    {
        _uiDocument = GetComponent<UIDocument>();
        _root = _uiDocument.rootVisualElement;

        if (_root == null)
        {
            Debug.Log($"{gameObject.name}: cannot initialize UI - root visual element is NULL.");
            return;
        }

        _bottomButtonPanel = _root.Q<VisualElement>("buttons-panel");
        _portraitPanel = _root.Q<VisualElement>("character-icon-panel");
        _characterIcon = _root.Q<Icon>("character-icon");
        _characterNameLabel = _root.Q<Label>("character-name-label");
        _endTurnButton = _root.Q<Button>("end-turn-button");
        _cancelButton = _root.Q<Button>("cancel-button");

        _endTurnButton.RegisterCallback<ClickEvent>(OnEndTurnCLick);
        _cancelButton.RegisterCallback<ClickEvent>(OnCancelSelectedSkill);

        HideCancelButton();
        HideBottomUI();
    }

    #endregion // Unity callback methods


    #region Private methods

    private void OnEndTurnCLick(ClickEvent evt)
    {
        _eventManager.OnTurnEnd();
    }

    private void OnCancelSelectedSkill(ClickEvent evt)
    {
        _eventManager.OnCancelSelectedSkill();
        HideCancelButton();
    }

    private void OnSelectCharacter(CharacterBase character)
    {
        ShowBottomUI();
        _characterIcon.sprite = character.stats.portrait;
        _characterNameLabel.text = character.stats.characterName;

        var currentCooldowns = character.GetCurrentCooldowns();

        for (var i = 0; i < _skillButtons.Count; i++)
        {
            _skillButtons[i].Item1.SkillStats = character.GetSkillStats(i);
            _skillButtons[i].Item2.SkillStats = character.GetSkillStats(i);
            _skillButtons[i].Item1.Cooldown = currentCooldowns[i];
        }
    }

    private void InitButtons()
    {
        _skillButtons.Add((
            _root.Q<SkillButton>("skill-button-1"),
            _root.Q<SkillDetail>("skill-detail-1"),
            0
        ));
        _skillButtons.Add((
            _root.Q<SkillButton>("skill-button-2"),
            _root.Q<SkillDetail>("skill-detail-2"),
            1
        ));
        _skillButtons.Add((
            _root.Q<SkillButton>("skill-button-3"),
            _root.Q<SkillDetail>("skill-detail-3"),
            2
        ));
        _skillButtons.Add((
            _root.Q<SkillButton>("skill-button-4"),
            _root.Q<SkillDetail>("skill-detail-4"),
            3
        ));

        foreach (var skillButton in _skillButtons)
        {
            skillButton.Item1.Detail = skillButton.Item2;
            skillButton.Item1.RegisterCallback<ClickEvent, (SkillButton, int)>(OnSkillButtonClick,
                (skillButton.Item1, skillButton.Item3));
        }
    }

    private void OnSkillButtonClick(ClickEvent evt, (SkillButton, int) button)
    {
        if (button.Item1.SkillStats == null)
            return;

        _eventManager.OnSelectedSkill(button.Item1.SkillStats, button.Item2);
    }

    private void InitQueues()
    {
        {
            var queue = _root.Q<ListView>("queue-3");
            queue.makeItem = MakeQueuedSkillView;
            queue.bindItem = BindItemFast;
            queue.itemsSource = CombatManager.Instance._playerQueueFast;

            var scroll = queue.Q<ScrollView>();
            scroll.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            scroll.verticalScrollerVisibility = ScrollerVisibility.Hidden;

            _playerQueue.Add(queue);
        }

        {
            var queue = _root.Q<ListView>("queue-2");
            queue.makeItem = MakeQueuedSkillView;
            queue.bindItem = BindItemMedium;
            queue.itemsSource = CombatManager.Instance._playerQueueMedium;

            var scroll = queue.Q<ScrollView>();
            scroll.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            scroll.verticalScrollerVisibility = ScrollerVisibility.Hidden;

            _playerQueue.Add(queue);
        }

        {
            var queue = _root.Q<ListView>("queue-1");
            queue.makeItem = MakeQueuedSkillView;
            queue.bindItem = BindItemSlow;
            queue.itemsSource = CombatManager.Instance._playerQueueSlow;

            var scroll = queue.Q<ScrollView>();
            scroll.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            scroll.verticalScrollerVisibility = ScrollerVisibility.Hidden;

            _playerQueue.Add(queue);
        }

        // Enemy

        {
            var queue = _root.Q<ListView>("queue-enemy-1");
            queue.makeItem = MakeQueuedSkillView;
            queue.bindItem = BindItemEnemyFast;
            queue.itemsSource = CombatManager.Instance._enemyQueueFast;

            var scroll = queue.Q<ScrollView>();
            scroll.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            scroll.verticalScrollerVisibility = ScrollerVisibility.Hidden;

            _enemyQueue.Add(queue);
        }

        {
            var queue = _root.Q<ListView>("queue-enemy-2");
            queue.makeItem = MakeQueuedSkillView;
            queue.bindItem = BindItemEnemyMedium;
            queue.itemsSource = CombatManager.Instance._enemyQueueMedium;

            var scroll = queue.Q<ScrollView>();
            scroll.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            scroll.verticalScrollerVisibility = ScrollerVisibility.Hidden;

            _enemyQueue.Add(queue);
        }

        {
            var queue = _root.Q<ListView>("queue-enemy-3");
            queue.makeItem = MakeQueuedSkillView;
            queue.bindItem = BindItemEnemySlow;
            queue.itemsSource = CombatManager.Instance._enemyQueueSlow;

            var scroll = queue.Q<ScrollView>();
            scroll.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            scroll.verticalScrollerVisibility = ScrollerVisibility.Hidden;

            _enemyQueue.Add(queue);
        }
    }

    private void BindItemFast(VisualElement element, int idx)
    {
        var queuedSkills = CombatManager.Instance._playerQueueFast;
        var skillView = element as QueuedSkillView;
        if (skillView != null)
        {
            skillView.SetSkillData(queuedSkills[idx]);
            queuedSkills[idx].SetView(skillView);
        }
    }

    private void BindItemMedium(VisualElement element, int idx)
    {
        var queuedSkills = CombatManager.Instance._playerQueueMedium;
        var skillView = element as QueuedSkillView;
        if (skillView != null)
        {
            skillView.SetSkillData(queuedSkills[idx]);
            queuedSkills[idx].SetView(skillView);
        }
    }

    private void BindItemSlow(VisualElement element, int idx)
    {
        var queuedSkills = CombatManager.Instance._playerQueueSlow;
        var skillView = element as QueuedSkillView;
        if (skillView != null)
        {
            skillView.SetSkillData(queuedSkills[idx]);
            queuedSkills[idx].SetView(skillView);
        }
    }

    private void BindItemEnemyFast(VisualElement element, int idx)
    {
        var queuedSkills = CombatManager.Instance._enemyQueueFast;
        var skillView = element as QueuedSkillView;
        if (skillView != null)
        {
            skillView.SetSkillData(queuedSkills[idx]);
            queuedSkills[idx].SetView(skillView);
        }
    }

    private void BindItemEnemyMedium(VisualElement element, int idx)
    {
        var queuedSkills = CombatManager.Instance._enemyQueueMedium;
        var skillView = element as QueuedSkillView;
        if (skillView != null)
        {
            skillView.SetSkillData(queuedSkills[idx]);
            queuedSkills[idx].SetView(skillView);
        }
    }

    private void BindItemEnemySlow(VisualElement element, int idx)
    {
        var queuedSkills = CombatManager.Instance._enemyQueueSlow;
        var skillView = element as QueuedSkillView;
        if (skillView != null)
        {
            skillView.SetSkillData(queuedSkills[idx]);
            queuedSkills[idx].SetView(skillView);
        }
    }

    private VisualElement MakeQueuedSkillView()
    {
        var queuedSkill = new QueuedSkillView
        {
            name = "queued-skill"
        };
        queuedSkill.AddToClassList("queued-skill");
        return queuedSkill;
    }

    private void OnAddedSkillToPlayerQueue(QueuedSkill skill)
    {
        if (skill == null || skill.SkillStats == null)
            return;
        RefreshQueues();
    }

    private void OnCombatStateChanged(CombatState state)
    {
        RefreshQueues();
        switch (state)
        {
            case CombatState.EnemyTurn:
                HideBottomUI();
                break;
            case CombatState.SkillPick:
                // Show bottom UI
                // ShowBottomUI();
                HideCancelButton();

                break;
            case CombatState.TargetPick:
                // Disable bottom UI
                // Show Cancel button
                HideBottomUI();
                ShowCancelButton();

                break;
            case CombatState.TurnEnded:
                // Disable bottom UI
                HideBottomUI();
                HideCancelButton();

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private void HideCancelButton()
    {
        _cancelButton.AddToClassList(HiddenUssClass);
        _cancelButton.RemoveFromClassList(VisibleUssClass);
    }

    private void ShowCancelButton()
    {
        _cancelButton.AddToClassList(VisibleUssClass);
        _cancelButton.RemoveFromClassList(HiddenUssClass);
    }

    private void HideBottomUI()
    {
        _bottomButtonPanel.AddToClassList(HiddenUssClass);
        _portraitPanel.AddToClassList(HiddenUssClass);
        _bottomButtonPanel.RemoveFromClassList(VisibleUssClass);
        _portraitPanel.RemoveFromClassList(VisibleUssClass);
    }

    private void ShowBottomUI()
    {
        _bottomButtonPanel.AddToClassList(VisibleUssClass);
        _portraitPanel.AddToClassList(VisibleUssClass);
        _bottomButtonPanel.RemoveFromClassList(HiddenUssClass);
        _portraitPanel.RemoveFromClassList(HiddenUssClass);
    }

    private void OnSkillPlayed()
    {
        RefreshQueues();
    }

    private void OnSkillEnded()
    {
        // RefreshQueues();
    }

    private void OnTurnEnded()
    {
        //RefreshQueues();
    }

    private void RefreshQueues()
    {
        _playerQueue[0].Clear();
        _playerQueue[1].Clear();
        _playerQueue[2].Clear();
        _playerQueue[0].itemsSource = CombatManager.Instance._playerQueueFast;
        _playerQueue[1].itemsSource = CombatManager.Instance._playerQueueMedium;
        _playerQueue[2].itemsSource = CombatManager.Instance._playerQueueSlow;
        _playerQueue[0].RefreshItems();
        _playerQueue[1].RefreshItems();
        _playerQueue[2].RefreshItems();

        _enemyQueue[0].Clear();
        _enemyQueue[1].Clear();
        _enemyQueue[2].Clear();
        _enemyQueue[0].itemsSource = CombatManager.Instance._enemyQueueFast;
        _enemyQueue[1].itemsSource = CombatManager.Instance._enemyQueueMedium;
        _enemyQueue[2].itemsSource = CombatManager.Instance._enemyQueueSlow;
        _enemyQueue[0].RefreshItems();
        _enemyQueue[1].RefreshItems();
        _enemyQueue[2].RefreshItems();
    }

    #endregion // Private methods
}