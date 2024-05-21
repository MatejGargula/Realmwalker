using TMPro;
using UnityEngine;

public class HelpText : MonoBehaviour
{
    [SerializeField] private string[] helpTexts; // array of help texts - ex: Select skill, Select target etc...
    private int currentIndex;
    private TextMeshProUGUI textComponent; // cached

    private void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        currentIndex = 0;
        textComponent.text = helpTexts[currentIndex];
    }

    /// <summary>
    ///     Updates the help text (Panel in the middle)
    ///     0 = SKILL PICK
    ///     1 = TARGETING
    ///     2 = ATTACK
    ///     3 = WAITING
    /// </summary>
    /// <param name="messageIndex"></param>
    public void UpdateMessege(int messageIndex)
    {
        textComponent.text = helpTexts[messageIndex];
    }
}