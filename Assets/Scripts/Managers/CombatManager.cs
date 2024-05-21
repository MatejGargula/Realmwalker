using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Realmwalker.Combat
{
    public enum CombatState
    {
        EnemyTurn,
        SkillPick,
        TargetPick,
        TurnEnded
    }

    [RequireComponent(typeof(UIDocument))]
    public class CombatManager : MonoBehaviour
    {
        #region Public methods

        public List<CharacterBase> GetAvailablePlayerTargets()
        {
            return playerParty;
        }

        #endregion // Public methods

        #region Serialized fields

        public List<CharacterBase> playerParty = new();
        public List<CharacterBase> enemyParty = new();

        public CharacterBase selectedCharacter;
        public SkillStats selectedSkillStats;

        #endregion // Serialized fields

        #region Properties

        public static CombatManager Instance { get; private set; }

        [SerializeField] private CombatState _state = CombatState.SkillPick;

        public CombatState State
        {
            get => _state;
            private set
            {
                _state = value;
                _eventManager.OnCombatStateChanged(_state);
            }
        }

        #endregion // Properties

        #region Data members

        private EventManager _eventManager;

        public List<QueuedSkill> _playerQueueFast = new();
        public List<QueuedSkill> _playerQueueMedium = new();
        public List<QueuedSkill> _playerQueueSlow = new();

        //TODO: Add a view for enemy queues
        public List<QueuedSkill> _enemyQueueFast = new();
        public List<QueuedSkill> _enemyQueueMedium = new();
        public List<QueuedSkill> _enemyQueueSlow = new();

        private int _selectedButtonOrder;

        private const int SkillInQueueLimit = 4;

        private bool _queueSwitcher;

        #endregion // Data members

        #region Unity callbacks

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;
        }

        private void Start()
        {
            _eventManager = EventManager.Instance;

            _eventManager.SkillPlayedEvent += OnSkillPlayed;
            _eventManager.SkillEndedEvent += OnSkillEnded;
            _eventManager.SelectedCharacterEvent += OnSelectCharacter;
            _eventManager.SelectedSkillEvent += OnSelectedSkill;
            _eventManager.SelectedTargetEvent += OnSelectedTarget;
            _eventManager.CancelSelectedSkillEvent += OnCancelSelectedSkill;
            _eventManager.TurnEndEvent += OnTurnEnded;
            _eventManager.SkillEndedEvent += PlaySkillInQueues;
            _eventManager.EnemySelectedTargetEvent += OnEnemySelectedTarget;
            _eventManager.EnemyTurnEndedEvent += OnEnemyTurnEnded;

            State = CombatState.SkillPick;
        }

        private void OnDestroy()
        {
            _eventManager.SkillPlayedEvent -= OnSkillPlayed;
            _eventManager.SkillEndedEvent -= OnSkillEnded;
            _eventManager.SelectedCharacterEvent -= OnSelectCharacter;
            _eventManager.SelectedSkillEvent -= OnSelectedSkill;
            _eventManager.SelectedTargetEvent -= OnSelectedTarget;
            _eventManager.CancelSelectedSkillEvent -= OnCancelSelectedSkill;
            _eventManager.TurnEndEvent -= OnTurnEnded;
            _eventManager.SkillEndedEvent -= PlaySkillInQueues;
            _eventManager.EnemySelectedTargetEvent -= OnEnemySelectedTarget;
            _eventManager.EnemyTurnEndedEvent -= OnEnemyTurnEnded;
        }

        #endregion // Unity callbacks

        #region Private methods

        private void OnSelectedSkill(SkillStats stats, int order)
        {
            _selectedButtonOrder = order;
            selectedSkillStats = stats;
            State = CombatState.TargetPick;
        }

        private void OnSelectCharacter(CharacterBase character)
        {
            selectedCharacter = character;
        }

        private void OnSelectedTarget(CharacterBase target)
        {
            selectedCharacter.DisableSelection();

            var skillSpeed = selectedSkillStats.speed;

            var queuedSkill =
                new QueuedSkill(selectedSkillStats, selectedCharacter, target, _selectedButtonOrder);

            if (skillSpeed == SkillSpeed.Fast && _playerQueueFast.Count < SkillInQueueLimit)
                _playerQueueFast.Add(queuedSkill);

            if (skillSpeed == SkillSpeed.Medium && _playerQueueMedium.Count < SkillInQueueLimit)
                _playerQueueMedium.Add(queuedSkill);

            if (skillSpeed == SkillSpeed.Slow && _playerQueueSlow.Count < SkillInQueueLimit)
                _playerQueueSlow.Add(queuedSkill);

            selectedCharacter = null;

            State = CombatState.SkillPick;
            _eventManager.OnAddedSkillToPlayerQueue(queuedSkill);
        }

        private void OnEnemyTurnEnded()
        {
            State = CombatState.SkillPick;
        }

        private void OnEnemySelectedTarget(QueuedSkill skill)
        {
            var skillSpeed = skill.SkillStats.speed;

            if (skillSpeed == SkillSpeed.Fast && _enemyQueueFast.Count < SkillInQueueLimit)
                _enemyQueueFast.Add(skill);

            if (skillSpeed == SkillSpeed.Medium && _enemyQueueMedium.Count < SkillInQueueLimit)
                _enemyQueueMedium.Add(skill);

            if (skillSpeed == SkillSpeed.Slow && _enemyQueueSlow.Count < SkillInQueueLimit)
                _enemyQueueSlow.Add(skill);
        }

        private void OnTurnEnded()
        {
            State = CombatState.TurnEnded;

            PlaySkillInQueues();
        }

        private void OnCancelSelectedSkill()
        {
            selectedSkillStats = null;
            State = CombatState.SkillPick;
        }

        private void PlaySkillInQueues()
        {
            _queueSwitcher = !_queueSwitcher;

            // Player turn to play skill
            if (_queueSwitcher)
            {
                if (_playerQueueFast.Count != 0)
                {
                    // Queue still has skills to play
                    var skill = _playerQueueFast[0];
                    _playerQueueFast.RemoveAt(0);
                    skill.Play();
                    return;
                }
            }
            else // Enemy turn to play skill
            {
                if (_enemyQueueFast.Count != 0)
                {
                    // Queue still has skills to play
                    var skill = _enemyQueueFast[0];
                    _enemyQueueFast.RemoveAt(0);
                    skill.Play();
                    return;
                }
            }

            if (_playerQueueFast.Count != 0 || _enemyQueueFast.Count != 0)
            {
                PlaySkillInQueues();
                return;
            }


            //Queue is empty
            MoveQueues();

            State = CombatState.EnemyTurn;
            StartEnemyTurn();
        }

        private void OnSkillEnded()
        {
            //if (_playerQueueFast.Count != 0)
        }

        private void OnSkillPlayed()
        {
        }


        private void MoveQueues()
        {
            // Player Queues
            // Move medium queue to fast
            _playerQueueFast.Clear();
            _playerQueueFast = new List<QueuedSkill>(_playerQueueMedium);

            // Move slow queue to medium
            _playerQueueMedium.Clear();
            // HACK: To make sure the queue view is refreshed and showing the correct skills            
            _eventManager.OnCombatStateChanged(State);
            _playerQueueMedium = new List<QueuedSkill>(_playerQueueSlow);

            _playerQueueSlow.Clear();

            // Enemy Queues
            // Move medium queue to fast
            _enemyQueueFast.Clear();
            _enemyQueueFast = new List<QueuedSkill>(_enemyQueueMedium);

            // Move slow queue to medium
            _enemyQueueMedium.Clear();
            // HACK: To make sure the queue view is refreshed and showing the correct skills            
            _eventManager.OnCombatStateChanged(State);
            _enemyQueueMedium = new List<QueuedSkill>(_enemyQueueSlow);

            _enemyQueueSlow.Clear();
        }

        private void StartEnemyTurn()
        {
            _eventManager.OnEnemyTurnStarted();
        }

        #endregion // Private methods
    }
}