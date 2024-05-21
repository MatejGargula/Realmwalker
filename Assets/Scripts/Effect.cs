using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Effect : MonoBehaviour
{
    public float _lifeSpan;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void InitEffect(float lifeSpan)
    {
        _lifeSpan = lifeSpan;
        StartCoroutine(EffectCourutine());
    }

    public IEnumerator EffectCourutine()
    {
        yield return new WaitForSeconds(_lifeSpan);
        Destroy(gameObject);
    }
}