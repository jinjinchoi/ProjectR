using System.Collections;
using UnityEngine;

public class VFXComponent : MonoBehaviour
{
    [SerializeField] private Material onHitMaterial;
    [SerializeField] private float onHitVFXDuration = .15f;

    private SpriteRenderer sr;
    private Material originalMat;
    private Coroutine onHitCoroutine;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMat = sr.material;
    }

    public void PlayOnDamageVfx()
    {
        if (onHitMaterial == null) return;

        if (onHitCoroutine != null)
            StopCoroutine(onHitCoroutine);

        onHitCoroutine = StartCoroutine(OnDamageVfxCo());
    }

    private IEnumerator OnDamageVfxCo()
    {
        sr.material = onHitMaterial;
        
        yield return new WaitForSeconds(onHitVFXDuration);

        sr.material = originalMat;
    }

}
