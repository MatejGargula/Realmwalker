using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectorButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Config")] [SerializeField] private int number;

    [SerializeField] private bool isEnemy;
    [SerializeField] private GameObject targeter;

    [Header("Support Objects")] [SerializeField]
    private Button button;

    //cahed
    private OldCombatManager cm;

    private void Awake()
    {
        cm = FindObjectOfType<OldCombatManager>();
        targeter.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (cm.skillSelected)
        {
            cm.combatSkeleton.showTargeter(number, isEnemy, true);
            ;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cm.combatSkeleton.showTargeter(number, isEnemy, false);
        //targeter.SetActive(false);
    }

    public void selectCharacter()
    {
        if (cm.skillSelected)
            cm.PutSkillInQueue(number, isEnemy);
        else if (!isEnemy) cm.SetSelectedChar(number);
    }

    public void DisableButton()
    {
        button.interactable = false;
        Debug.Log("Disabling a selector button");
    }

    public void EnableButton()
    {
        button.interactable = true;
        Debug.Log("Enabling a selector button");
    }
}