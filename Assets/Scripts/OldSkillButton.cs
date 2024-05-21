using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class OldSkillButton : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private int number;
    [SerializeField] public bool usable = true;
    [SerializeField] public bool onCooldown;

    [SerializeField] public int
        speedRateOfSkill = 1; // Determines where should the skill should be put in the queue. (not used for now) 

    [SerializeField] private Button button;

    [FormerlySerializedAs("combatManager")] [SerializeField]
    private OldCombatManager oldCombatManager;

    // Serialized field is only for debug purposes
    // Skill sprite should be loaded from Charater stats scriptable object
    [SerializeField] public Sprite sprite;

    private Image image;

    private void Awake()
    {
        oldCombatManager = FindObjectOfType<OldCombatManager>().GetComponent<OldCombatManager>();
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        image.sprite = sprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //OldCombatManager.Instance.ChangeInfoBox(number);
    }


    public void ButtonClicked()
    {
        //Debug.Log("You have pressed a button number: " + number);
        //Debug.Log("Putting skill in queue. Skill number: " + number);
        oldCombatManager.prepareSkillInQueue(number);

        if (usable && !onCooldown)
        {
            //Debug.Log("Putting skill in queue. Skill number: " + number);
            //combatManager.putSkillInQueue(number);
        }
        //Debug.Log("THIS SKILL IS NOT USABLE - number: " + number);
    }

    public void ChangeButton(Sprite sprite)
    {
        this.sprite = sprite;
        image.sprite = sprite;
        button.interactable = true;
    }

    public void DisableButton()
    {
        button.interactable = false;
        //Debug.Log("Disabling a skill button");
    }

    public void EnableButton()
    {
        button.interactable = true;
        //Debug.Log("Enabling a skill button");
    }
}