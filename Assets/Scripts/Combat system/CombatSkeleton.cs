using UnityEngine;

public class CombatSkeleton : MonoBehaviour
{
    #region Unity callback methods

    private void OnTriggerEnter(Collider other)
    {
        GameManager.Instance.StartCombat();
        GetComponent<Collider2D>().enabled = false;
    }

    #endregion // Unity callback methods

    #region Serialized fields

    [SerializeField] private Transform[] playerPartyPositions;
    [SerializeField] private Transform[] enemyPartyPositions;
    [SerializeField] private Targeter[] enemyTargeters;
    [SerializeField] private Targeter[] playerTargeters;

    #endregion // Serialized fields

    #region Public methods

    public void Init(GameObject[] playerParty, GameObject[] enemyParty)
    {
        for (var i = 0; i < playerParty.Length; i++)
        {
            playerParty[i].SetActive(true);
            playerParty[i].transform.position = playerPartyPositions[i].position;
        }

        for (var i = 0; i < enemyParty.Length; i++)
        {
            enemyParty[i].SetActive(true);
            enemyParty[i].transform.position = enemyPartyPositions[i].position;
            enemyParty[i].GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    public void showTargeter(int number, bool isEnemy, bool show)
    {
        //if (isEnemy)
        //{
        //    enemyTargeters[number].gameObject.SetActive(show);
        //    if (show)
        //    {
        //        enemyTargeters[number].ShowTargeter(OldCombatManager.Instance.currentlySelectedSkill.type);
        //    }
        //}
        //else
        //{
        //    playerTargeters[number].gameObject.SetActive(show);
        //    if (show)
        //    {
        //        playerTargeters[number].ShowTargeter(OldCombatManager.Instance.currentlySelectedSkill.type);
        //    }
        //}
    }

    #endregion // Public methods
}