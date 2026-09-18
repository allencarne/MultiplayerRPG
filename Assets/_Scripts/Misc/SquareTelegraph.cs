using Unity.Netcode;
using UnityEngine;

public class SquareTelegraph : NetworkBehaviour, ITelegraph
{
    [SerializeField] SpriteRenderer frontSprite;
    CharacterStats stats;
    float fillSpeed;
    CrowdControl crowdControl;
    bool listenInterrupt;
    bool listenStagger;

    public void Init(CharacterStats _stats, float _fillDuration, CrowdControl _crowdControl, bool cancelOnInterrupt, bool cancelOnStagger)
    {
        stats = _stats;
        fillSpeed = _fillDuration;
        crowdControl = _crowdControl;
        listenInterrupt = cancelOnInterrupt;
        listenStagger = cancelOnStagger;

        if (listenInterrupt) crowdControl.OnInterrupted.AddListener(Destroy);
        if (listenStagger) crowdControl.OnStagger.AddListener(Destroy);
        stats.OnDeath.AddListener(Destroy);
    }

    private void OnDisable()
    {
        if (stats == null) return;
        if (listenInterrupt) crowdControl.OnInterrupted.RemoveListener(Destroy);
        if (listenStagger) crowdControl.OnStagger.RemoveListener(Destroy);
        stats.OnDeath.RemoveListener(Destroy);
    }

    private void Start()
    {
        if (frontSprite != null)
        {
            // Start with scale.x = 0
            frontSprite.transform.localScale = new Vector3(0f, frontSprite.transform.localScale.y, frontSprite.transform.localScale.z);
        }
    }

    private void Update()
    {
        float scaleIncrement = Time.deltaTime / fillSpeed;

        // Grow only along the X axis
        Vector3 currentScale = frontSprite.transform.localScale;
        float newScaleX = Mathf.Min(currentScale.x + scaleIncrement, 1f);

        frontSprite.transform.localScale = new Vector3(newScaleX, currentScale.y, currentScale.z);

        // Once fully filled, destroy
        if (newScaleX >= 1f && IsServer)
        {
            Destroy(gameObject);
        }
    }

    void Destroy()
    {
        Destroy(gameObject);
    }
}
