using UnityEngine;

public class PopUpGenerator : MonoBehaviour
{
    #region Properties

    public static PopUpGenerator Instance { get; private set; }

    #endregion //Properties

    #region Unity callback methods

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    #endregion // Unity callback methods

    #region Public methods

    public void CreateDamagePopUp(Vector3 position, string damageText, Color color)
    {
        var popUp = Instantiate(damagePopUpPrefab, position, Quaternion.identity);
        if (popUp.TryGetComponent(out TextPopUp textMeshPro))
        {
            textMeshPro.Text = damageText;
            textMeshPro.Color = color;
        }

        //Pop up will be destroyed after 1 second
        Destroy(popUp, popUpLifeTime);
    }

    #endregion // Public methods

    #region Unity serialized fields

    public GameObject damagePopUpPrefab; // prefab with text mesh pro component

    public float popUpLifeTime = 1f;

    #endregion // Unity serialized fields

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        CreateDamagePopUp(Vector3.zero, "999");
    //    }
    //}
}