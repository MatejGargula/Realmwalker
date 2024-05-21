using UnityEngine;

public enum TargeterType
{
    ATTACK, // red cross sprite
    DEFEND, // shield sprite
    BUFF, // arrows pointing up and plus sign
    DEBUFF // arrows pointing down and minus sign
}

public class Targeter : MonoBehaviour
{
    [SerializeField] private Sprite attackSprite;
    [SerializeField] private Sprite defendSprite;
    [SerializeField] private Sprite buffSprite;
    [SerializeField] private Sprite debuffSprite;

    private SpriteRenderer sr;

    public void Awake()
    {
        gameObject.SetActive(false);
        sr = GetComponent<SpriteRenderer>();
    }

    public void ShowTargeter(TargeterType type)
    {
        Sprite newSprite = null;
        switch (type)
        {
            case TargeterType.ATTACK:
                newSprite = attackSprite;
                break;
            case TargeterType.DEFEND:
                newSprite = defendSprite;
                break;
            case TargeterType.BUFF:
                newSprite = buffSprite;
                break;
            case TargeterType.DEBUFF:
                newSprite = debuffSprite;
                break;
            default:
                Debug.Log("Warning: Unkown targeter type!");
                break;
        }

        if (newSprite != null)
            sr.sprite = newSprite;
        else
            Debug.Log("Warning: Cannot display target sprite - uknown type?");
    }
}