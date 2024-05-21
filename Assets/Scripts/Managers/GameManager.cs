using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    #region Data members

    public DebugLogger logger;

    #endregion // Data members

    #region Properties

    public static GameManager Instance { get; private set; }

    #endregion // Properties

    #region Serialized fields

    [SerializeField] private CinemachineVirtualCamera mainCamera;

    [FormerlySerializedAs("combatManager")] [SerializeField]
    private OldCombatManager oldCombatManager;

    [SerializeField] private GameObject combatTransition;

    [SerializeField] private Transform CameraPlayer;
    [SerializeField] private Transform CameraCombat;

    #endregion // Serialized fields

    #region Public methods

    public void ExitGame()
    {
        Application.Quit();
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene("Combat_Prototype", LoadSceneMode.Single);
    }

    public void StopCombat()
    {
    }

    public void StartCombat()
    {
        Debug.Log("Starting COMBAT!");
        //TODO: combat manager init magic
        mainCamera.Follow = CameraCombat;
        // mainCamera.LookAt = CameraCombat;
        //combatManager.InitCombat();
        combatTransition.SetActive(true);
    }

    #endregion // Public methods

    #region Unity callback methods

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) StartCombat();
    }

    #endregion // Unity callback methods
}