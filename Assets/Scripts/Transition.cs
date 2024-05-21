using System.Collections;
using UnityEngine;

public class Transition : MonoBehaviour
{
    public void EndTransition()
    {
        StartCoroutine(TransitionCoroutine());
    }

    private IEnumerator TransitionCoroutine()
    {
        FindObjectOfType<OldCombatManager>().InitCombat();
        yield return new WaitForSeconds(1.0f);
        gameObject.SetActive(false);
    }
}