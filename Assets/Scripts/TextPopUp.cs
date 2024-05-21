using TMPro;
using UnityEngine;

public class TextPopUp : MonoBehaviour
{
    private void Awake()
    {
        if (textComponent == null)
            textComponent = GetComponentInChildren<TextMeshProUGUI>();

        transform.position += new Vector3(Random.Range(-0.7f, 0.7f), 0);
        _origin = transform.position;
    }

    private void Update()
    {
        textComponent.color = new Color(_color.r, _color.g, _color.b, opacityCurve.Evaluate(_time));
        transform.localScale = Vector3.one * scaleCurve.Evaluate(_time);
        transform.position = _origin + new Vector3(0, 1 + positionCurve.Evaluate(_time));

        _time += Time.deltaTime;
    }

    #region Unity serialized fields

    [SerializeField] private TextMeshProUGUI textComponent;
    public AnimationCurve opacityCurve;
    public AnimationCurve scaleCurve;
    public AnimationCurve positionCurve;

    #endregion // Unity serialized fields

    #region Properties

    private string _text;

    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            textComponent.text = value;
        }
    }

    private Color _color;

    public Color Color
    {
        get => _color;
        set
        {
            _color = value;
            textComponent.color = value;
        }
    }

    #endregion // Properties

    #region MyRegion

    private float _time;
    private Vector3 _origin;

    #endregion
}