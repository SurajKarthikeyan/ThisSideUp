using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Package : MonoBehaviour
{
    enum DamageReductionType { exponential, time, distance };

    [Tooltip("The package's current health")]
    public int health = 100;

    [Header("Elemental Status Settings")]
    [Tooltip("Extra damage multiplier while waterlogged")]
    [SerializeField] private float waterloggedDamageMultiplier = 1.5f;
    [Tooltip("Time in seconds the package stays waterlogged for")]
    [SerializeField] private float waterloggedTime = 10.0f;
    [Tooltip("Damage per second while on fire")]
    [SerializeField] private int fireDamagePerSecond = 25;
    [Tooltip("Time in seconds the package burns for")]
    [SerializeField] private float burnTime = 10.0f;
    [Tooltip("Amount of time a zapped package will stun an enemy")]
    [SerializeField] private float zapStunTime = 6.0f;
    [Tooltip("Time in seconds the package is zapped for")]
    [SerializeField] public float zapTime = 10.0f;
    [Tooltip("One-time chunk of damage dealt whenever the zapped effect is applied")]
    [SerializeField] private int zapDamage = 800;

    [Header("Physical Damage Settings")]
    [Tooltip("The type of physical damage reduction to prevent spam")]
    [SerializeField] DamageReductionType damageType;
    [Tooltip("Damage modifier on impact")]
    [SerializeField] private float impactDamageModifier = 10f;
    [Tooltip("Exponential factor at which the physical damage amount will decay if exponential reduction is selected (a modifier of 1 result in constant damage)")]
    [Range(.1f, 1f)]
    [SerializeField] private float decayModifier = .9f;
    [Tooltip("Minimum distance traveled from the previous hit for damage to be dealt again if distance damage reduction is selected")]
    [SerializeField] float minDamageDistance;
    [Tooltip("Minimum time elapsed since the previous hit for damage to be dealt again if time damage reduction is selected")]
    [SerializeField] float minDamageTime;

    [Header("VFX Settings")]
    [Tooltip("The particle system for the packing peanuts")]
    [SerializeField] ParticleSystem packingPart;
    [Tooltip("The multiplier to apply to the damage to determine the number of packing particles to emit")]
    [SerializeField] float packingParticleEmissionFactor;
    [Tooltip("The minimum delay between bursts")]
    [SerializeField] float minBurstDelay;
    [Tooltip("The damage amount at which the packing peanuts will be fully red")]
    [SerializeField] float redPackingPeanutDamage;
    [Tooltip("The sprites for the package at increasing levels of damage")]
    [SerializeField] List<Sprite> damageSprites;
    [Tooltip("The damage thresholds at which to switch sprite")]
    [SerializeField] List<int> damageThresholds;
    [Tooltip("The material to flash when the package takes damage")]
    [SerializeField] Material flashMat;
    [Tooltip("How long to flash the material when the package takes damage")]
    [SerializeField] float flashDuration;
    [Tooltip("The minimum amount of damage required for the package to flash")]
    [SerializeField] float minFlashDamage;
    [Tooltip("How long hitpause should last")]
    [SerializeField] float hitPauseDuration;
    [Tooltip("The minimum amount of damage that will trigger hitpause")]
    [SerializeField] float minHitPauseDamage;
    [Tooltip("The Cinemachine InpulseSource to use for screenshake")]
    [SerializeField] Cinemachine.CinemachineImpulseSource impulseSource;
    [Tooltip("The multiplier to apply to the screenshake velocity")]
    [SerializeField] float impulseFactor;
    [Tooltip("The minimum damage required for screenshake to occur")]
    [SerializeField] float minImpulseDamage;
    [Tooltip("The material to use when this package is on fire")]
    public Material fireMaterial;
    [Tooltip("The gameObject containing the fire effects")]
    public GameObject fireEffects;
    [Tooltip("The material to use when this package is wet")]
    public Material wetMaterial;
    [Tooltip("The gameObject containing the water effects")]
    public GameObject waterEffects;
    [Tooltip("The material to use when this package is electrocuted")]
    public Material electrocutedMaterial;
    [Tooltip("The gameObject containing the electric effects")]
    public  GameObject electricEffects;

    [Header("SFX Settings")]
    [Tooltip("The prefab to instantiate to play a breaking sound effect")]
    [SerializeField] GameObject breakingSFX;
    [Tooltip("The minimum damage and volume pair to scale the volume based on damage")]
    [SerializeField] Vector2 minDamageVolume;
    [Tooltip("The maximum damage and volume pair to scale the volume based on damage")]
    [SerializeField] Vector2 maxDamageVolume;

    [HideInInspector] public PackageInteraction currentInteractor;
    public PackageStatus packageStatus;
    [HideInInspector]
    public SpriteRenderer render;
    [HideInInspector]
    public Material defaultMaterial;
    [HideInInspector]
    public Material currentMaterial;
    [HideInInspector]
    public Movable movable;

    Rigidbody2D rigid;
    ParticleSystem.EmissionModule packingEmis;
    ParticleSystem.MainModule packingMain;
    float timeLastHit;
    float distanceFromLastHit;
    Vector2 storedVelocity;
    float storedAngularVelocity;

    private void Awake()
    {
        render = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        packingEmis = packingPart.emission;
        packingMain = packingPart.main;
        packageStatus = new PackageStatus(this, health, 0, waterloggedTime, burnTime, zapTime);
        defaultMaterial = render.material;
        currentMaterial = defaultMaterial;
        movable = GetComponent<Movable>();
    }

    private void FixedUpdate()
    {
        distanceFromLastHit += rigid.velocity.magnitude * Time.fixedDeltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Calculate damage
        int newDamage = (int)(rigid.velocity.magnitude * impactDamageModifier);
        switch ((int)damageType)
        {
            case (int)DamageReductionType.exponential:
                // Maybe use estimated-based logarithm later. Mathf.Pow is O(log(n)) where n is the closest power of 2 to your number
                newDamage *= (int)Mathf.Pow(decayModifier, packageStatus.damage);
                break;
            case (int)DamageReductionType.distance:
                if (distanceFromLastHit < minDamageDistance)
                    newDamage = 0;
                distanceFromLastHit = 0;
                break;
            case (int)DamageReductionType.time:
                if (Time.time - timeLastHit < minDamageTime)
                    newDamage = 0;
                timeLastHit = Time.time;
                break;
        }
        
        // Add bonus damage from waterlogged effect
        if (packageStatus.IsWaterlogged())
        {
            packageStatus.damage += (int)(newDamage * waterloggedDamageMultiplier) - newDamage;
        }

        //Update damage and health
        packageStatus.TakeDamage(newDamage);

        //Update sprite
        int threshold = damageThresholds.Count;
        for (int i=0; i<damageThresholds.Count; i++)
        {
            if (packageStatus.damage <= damageThresholds[i])
            {
                threshold = i;
                break;
            }
        }
        render.sprite = damageSprites[threshold];

        //packing particle burst
        packingEmis.SetBurst(0, new ParticleSystem.Burst(0, newDamage * packingParticleEmissionFactor));
        packingMain.startColor = Color.Lerp(Color.white, Color.red, packageStatus.damage / redPackingPeanutDamage);
        packingPart.Play();

        //Flash white
        if (newDamage > minFlashDamage)
            StartCoroutine(nameof(FlashWhite));

        //Screenshake
        if (newDamage > minImpulseDamage)
            impulseSource.GenerateImpulseWithVelocity(rigid.velocity * impulseFactor);

        //Hitpause
        if (newDamage > minHitPauseDamage)
            StartCoroutine(nameof(HitPause));

        //SFX
        GameObject bSFX = Instantiate(breakingSFX);
        bSFX.GetComponent<AudioSource>().volume = Mathf.Lerp(minDamageVolume.y, maxDamageVolume.y, Mathf.Clamp((newDamage - minDamageVolume.x)/maxDamageVolume.x, 0, 1));
        print("Package damage: " + packageStatus.damage);
    }

    public Coroutine InflictBurnDamage()
    {
        return StartCoroutine(DealDamagePerSecond(fireDamagePerSecond));
    }

    public void InflictZapDamage()
    {
        Debug.Log("ZAPPED!");
        packageStatus.TakeDamage(zapDamage);
    }

    private IEnumerator DealDamagePerSecond(int damageAmount)
    {
        var startTime = Time.time;
        while (Time.time - startTime < burnTime)
        {
            packageStatus.TakeDamage(damageAmount);
            print(packageStatus.damage);
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator FlashWhite()
    {
        render.material = flashMat;
        yield return new WaitForSeconds(flashDuration);
        render.material = currentMaterial;
    }

    private IEnumerator HitPause()
    {
        storedVelocity = rigid.velocity;
        storedAngularVelocity = rigid.angularVelocity;
        rigid.freezeRotation = true;
        rigid.constraints = RigidbodyConstraints2D.FreezeAll;
        yield return new WaitForSeconds(hitPauseDuration);
        rigid.freezeRotation = false;
        rigid.constraints = RigidbodyConstraints2D.None;
        rigid.velocity = storedVelocity;
        rigid.angularVelocity = storedAngularVelocity;
    }
}
