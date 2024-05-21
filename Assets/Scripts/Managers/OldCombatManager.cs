using System.Collections;
using Realmwalker.Combat;
using UnityEngine;

internal enum OldCombatState
{
    WAITING, // when an animations is playing 
    SKILL_PICK, // player is picking the skills for the queues
    TARGETING, // player is choosing the targets for the skills
    ATTACK // player is  executing the attacks from the main queue
}

public class OldCombatManager : MonoBehaviour
{
    #region Properties

    public OldCombatManager Instance { get; private set; }

    #endregion // Properties

    #region Serialized fields

    [Header("Debug")] [SerializeField] private GameObject[] playerParty;
    [SerializeField] private bool[] availibleParty;
    [SerializeField] private GameObject[] enemyParty;
    [SerializeField] private CombatCharacter selectedCharacter;
    [SerializeField] public QueuedSkill currentlySelectedSkill;

    [Header("Support objects")] [SerializeField]
    private OldSkillButton[] skillButtons;

    [SerializeField] private SelectorButton[] selectorButtons;
    [SerializeField] private Transform[] playerPositions;
    [SerializeField] private Transform[] enemyPositions;
    [SerializeField] private InfoBox infoBox;
    [SerializeField] private PointerArrow arrow;
    [SerializeField] private GameObject CombatUI;
    [SerializeField] private HelpText helpText;
    [SerializeField] private SkillQueue[] playerQueues;
    [SerializeField] private SkillQueue[] enemyQueues;
    [SerializeField] public CombatSkeleton combatSkeleton;
    [SerializeField] private GameObject combatTransition;

    #endregion // Serialized fields

    #region Data members

    private OldCombatState _oldCombatState;
    public bool wait;
    public bool debugOn = true;
    public bool skillSelected;
    private int selectedCharacterNumber;

    #endregion // Data members

    #region Unity callback methods

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        _oldCombatState = OldCombatState.SKILL_PICK;
        wait = false;
        availibleParty = new bool[playerParty.Length];
        ResetSelectorButtons();
        //CombatUI.SetActive(false);
        SetSelectedChar(0);
        if (debugOn) checkSupportObjects();

        combatSkeleton = FindObjectOfType<CombatSkeleton>();
        combatTransition.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            skillSelected = false;
            currentlySelectedSkill = null;
            _oldCombatState = OldCombatState.SKILL_PICK;
            UpdateHelpText();
        }
    }

    #endregion // Unity callback methods

    #region Private methods

    /// <summary>
    ///     Checks if all support objects are initialized from the inspector.
    ///     if not a debug message will be printed in the console.
    /// </summary>
    private void checkSupportObjects()
    {
        if (CombatUI == null) Debug.Log("WARNING: combat UI not initialized");

        if (skillButtons.Length < 4) Debug.Log("WARNING: Only " + skillButtons.Length + " buttons initialized");

        if (playerPositions.Length < 4)
            Debug.Log("WARNING: Only " + playerPositions.Length + " playerPositions initialized");

        if (enemyPositions.Length < 4)
            Debug.Log("WARNING: Only " + enemyPositions.Length + " enemyPositions initialized");

        if (playerQueues.Length < 3) Debug.Log("WARNING: Only " + playerQueues.Length + " player queues initialized");

        if (enemyQueues.Length < 3) Debug.Log("WARNING: Only " + enemyQueues.Length + " enemy queues initialized");

        if (infoBox == null) Debug.Log("WARNING: info box not initialized");

        if (helpText == null) Debug.Log("WARNING: help text not initialized");

        if (arrow == null) Debug.Log("WARNING: pointer arrow not initialized");
    }

    /// <summary>
    ///     Plays a skill from the first/main queue.
    /// </summary>
    /// <param name="isEnemy">
    ///     isEnemy = true, then skill is played from the enemy queue.
    ///     isEnemy = false, then skill is played from the player queue.
    /// </param>
    private void playSkillFromQueue(bool isEnemy)
    {
        if (isEnemy)
        {
            if (!enemyQueues[0].empty)
            {
                var skill = enemyQueues[0].DequeueSkill();
                //skill.PlaySkill();
            }
        }
        else
        {
            if (!playerQueues[0].empty)
            {
                var skill = playerQueues[0].DequeueSkill();
                //skill.PlaySkill();
            }
        }
    }

    #endregion // Private methods

    #region Public methods

    /// <summary>
    ///     Updates the info box. Called after the player mouses over one of the character skills.
    /// </summary>
    /// <param name="number"></param>
    public void ChangeInfoBox(int number)
    {
        var name = selectedCharacter.stats.skillStats[number - 1].skillName;
        var description = selectedCharacter.stats.skillStats[number - 1].description;
        //string damage = (selectedCharacter.stats.damage * selectedCharacter.stats.skillStats[number - 1].damageModifier)
        //  .ToString();
        var speed = selectedCharacter.stats.skillStats[number - 1].speed.ToString();
        //infoBox.SetNewSkillInfo(name, description, damage, speed);
    }

    public void UpdateHelpText()
    {
        switch (_oldCombatState)
        {
            case OldCombatState.SKILL_PICK:
                helpText.UpdateMessege(0);
                break;
            case OldCombatState.TARGETING:
                helpText.UpdateMessege(1);
                break;
            case OldCombatState.ATTACK:
                helpText.UpdateMessege(2);
                break;
            case OldCombatState.WAITING:
                helpText.UpdateMessege(3);
                break;
        }
    }

    /// <summary>
    ///     This function is  called after the player has clicked on a skill button. The skill will not be put in the queue.
    ///     The skill will putted in the queue after the player has selected a target.
    /// </summary>
    /// <param name="skillNumber"> The skill number of the character that will be selected and played later</param>
    public void prepareSkillInQueue(int skillNumber)
    {
        if (selectedCharacter != null)
            //int skillSpeed = selectedCharacter.stats.skillStats[skillNumber - 1].speed - 1;
            //if (!playerQueues[skillSpeed].full)
        {
            skillSelected = true;


            //SelectedSkill = new QueuedSkill();
            //currentlySelectedSkill.origin = selectedCharacter;
            //currentlySelectedSkill.speed = skillSpeed;
            //SelectedSkill.skillNumber = skillNumber;
            //currentlySelectedSkill.type = selectedCharacter.stats.skillStats[skillNumber - 1].type;
            _oldCombatState = OldCombatState.TARGETING;
            UpdateHelpText();
        }
    }

    /// <summary>
    ///     Called when the player has selected a target for the selected skill. Puts a QueuedSkill in the skill queue;
    /// </summary>
    /// <param name="target"> Selected target of the skill</param>
    public void PutSkillInQueue(int targetNumber, bool targetEnemy)
    {
        CombatCharacter target;
        if (targetEnemy)
            target = enemyParty[targetNumber].GetComponent<CombatCharacter>();
        else
            target = playerParty[targetNumber].GetComponent<CombatCharacter>();

        //currentlySelectedSkill.target = target;
        //playerQueues[currentlySelectedSkill.speed].EnqueueSkill(currentlySelectedSkill);
        if (debugOn) Debug.Log("Putting skill in queue");

        skillSelected = false;
        CheckSkillButtons();
        _oldCombatState = OldCombatState.SKILL_PICK;
        UpdateHelpText();
        availibleParty[selectedCharacterNumber] = false;
        selectorButtons[selectedCharacterNumber].DisableButton();
        var nextCharacterIndex = -1;
        for (var i = 0; i < availibleParty.Length; i++)
            if (availibleParty[i])
            {
                SetSelectedChar(i);
                nextCharacterIndex = i;
                break;
            }

        if (nextCharacterIndex < 0)
            for (var i = 0; i < skillButtons.Length; i++)
                skillButtons[i].gameObject.SetActive(false);
    }


    public void SetPlayerParty(GameObject[] playerParty)
    {
        this.playerParty = playerParty;
    }

    public void SetEnemyParty(GameObject[] enemyParty)
    {
        this.enemyParty = enemyParty;
    }

    /// <summary>
    ///     Called at the begining of the combat. Prepares the game for a combat phase. Initializes combat UI, Places the
    ///     characters in the right spots(Combat skeleton needed for that - holds the positions fo the characters).
    /// </summary>
    public void InitCombat()
    {
        combatSkeleton.Init(playerParty, enemyParty);
        skillSelected = false;
        CombatUI.SetActive(true);
        //for (int i = 0; i < 4; i++)
        //{
        //    playerParty[i].gameObject.SetActive(true);
        //    enemyParty[i].gameObject.SetActive(true);
        //    playerParty[i].transform.position = playerPositions[i].position;
        //    enemyParty[i].GetComponent<SpriteRenderer>().flipX = true;
        //    enemyParty[i].transform.position = enemyPositions[i].position;
        //}
    }

    /// <summary>
    ///     Changes the selected character. Called after the player clicks on a character in hi party;
    /// </summary>
    /// <param name="number"> The selector button number - for finding the character in the partry array</param>
    public void SetSelectedChar(int number)
    {
        selectedCharacterNumber = number;
        selectedCharacter = playerParty[number].GetComponent<CombatCharacter>();
        for (var i = 0; i < skillButtons.Length; i++)
            skillButtons[i].ChangeButton(selectedCharacter.stats.skillStats[i].sprite);

        CheckSkillButtons();
        Debug.Log("Works");
        arrow.SetNewPosition(number);
        infoBox.SetCharacterInfo(selectedCharacter.stats.portrait, selectedCharacter.stats.characterName);
        if (debugOn) Debug.Log("Changing selected character to " + number);
    }

    public void CheckSkillButtons()
    {
        for (var i = 0; i < skillButtons.Length; i++)
        {
            //if (playerQueues[selectedCharacter.stats.skillStats[i].speed - 1].full)
            {
                skillButtons[i].DisableButton();
            }
            //else
            {
                skillButtons[i].EnableButton();
            }
        }
    }

    public void ResetSelectorButtons()
    {
        for (var i = 0; i < selectorButtons.Length; i++)
        {
            selectorButtons[i].EnableButton();
            availibleParty[i] = true;
        }
    }

    public void EndTurn()
    {
        StartCoroutine(EndTurnCoroutine());
    }

    #endregion // Public methods

    #region Courutine Methods

    /// <summary>
    ///     ONLY FOR DEBUG AND TESTING PURPOSES!
    ///     Coroutine that automatically destroys a queued skill gameobject
    /// </summary>
    /// <param name="skill"> QueuedSkill connected to an object that will be destroyed</param>
    /// <param name="speed"> For finding the queue the queue. Need to decrease the counter for allowed skills in queues </param>
    /// <returns></returns>
    private IEnumerator AutoDestroySkill(QueuedSkill skill, int speed)
    {
        yield return new WaitForSeconds(2);
        //Destroy(skill.gameObject);
        //skillsInQueues[speed]--;
    }

    private IEnumerator EndTurnCoroutine()
    {
        var skillCount = playerQueues[0].skillsInQueue;
        Debug.Log("Ending turn. Skill count: " + skillCount);
        wait = true;
        _oldCombatState = OldCombatState.WAITING;
        UpdateHelpText();
        playerQueues[0].PlayAllSKillsInQueueu();
        yield return new WaitWhile(() => wait);
        ResetSelectorButtons();
        CheckSkillButtons();
        for (var i = 0; i < skillButtons.Length; i++) skillButtons[i].gameObject.SetActive(true);

        _oldCombatState = OldCombatState.SKILL_PICK;
        UpdateHelpText();
    }

    #endregion // Courutine Methods
}