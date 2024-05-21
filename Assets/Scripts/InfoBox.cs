using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoBox : MonoBehaviour
{
    [Header("Stats panel")] [SerializeField]
    private TextMeshProUGUI skillName;

    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI speedText;

    [Header("Character Menu")] [SerializeField]
    private Image portraitObject;

    [SerializeField] private TextMeshProUGUI characterName;


    public void SetNewSkillInfo(string name, string description, string damage, string speed)
    {
        skillName.text = name;
        descriptionText.text = description;
        damageText.text = damage;
        speedText.text = speed;
    }

    public void SetCharacterInfo(Sprite sprite, string name)
    {
        portraitObject.sprite = sprite;
        characterName.text = name;
    }
}