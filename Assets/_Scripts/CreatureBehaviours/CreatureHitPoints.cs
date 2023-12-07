using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureHitPoints : MonoBehaviour {

    public float MaxHP = 1;
    public float currentHP = 1;

    public GameObject _HitParticles;

    public CreatureStats _MyStats;
    public bool _PlayerFlag;

    public delegate void HealthChanged(float health, float maxHealth);
    public event HealthChanged HealthChangedEvent;
    public delegate void MaxHealthChanged(float health);
    public event MaxHealthChanged MaxHealthChangedEvent;

    public delegate void HitReceived(SkillHit hit);
    public event HitReceived HitReceivedEvent;

    [SerializeField]
    private bool alive = true;
    public bool _Alive { get { return alive; } }

    //When a creature is knocked out, it will regain a % of its consciousness per second and though heals. When this is equal to its maxHP, it wakes up
    public float consciousness;
    public float _ConsciousnessRecoverInterval = 0.05f;
    public float consciousnessRecoveryRate = 0.1f;

    public delegate void ConsciousnessChanged(float consciousness, float maxConsciousness);
    public event ConsciousnessChanged ConsciousnessChangedEvent;

    public delegate void KnockOut();
    public event KnockOut KnockOutEvent;
    public delegate void Arise();
    public event Arise AriseEvent;


    void Awake ()
    {
        if (_MyStats == null)
            _MyStats = GetComponent<CreatureStats>();

        _MyStats._StatEvents[(int)PrimaryStat.HP] += OnBaseHPChange;

        // Add a flash renderer that flashes when health changes
        gameObject.AddComponent<FlashRenderer>();

        // DAMAGE OVER TIME TIMER
        InvokeRepeating("UpdateDOTEffects", 0f, _DamageOverTimeInterval);
        KnockOutEvent += ClearAllDOTEffects;
	}

    private void Start()
    {
        if ( gameObject.tag == "Player")
            _PlayerFlag = true;
    }

    public void OnBaseHPChange( float baseHP )
    {
        UpdateMaxHP( RolePlayingFormulas.HPfromHPStat(baseHP, GetComponent<Level>().level) );
    }

    public void UpdateMaxHP( float newMaxHP )
    {
        float hpDifference = newMaxHP - MaxHP;

        MaxHP = newMaxHP;

        //Increase current HP when maxhp increases, also ensure that the current hp is never highter than the total hp
        if (hpDifference > 0)
            currentHP += hpDifference;
        else if (currentHP > MaxHP)
            currentHP = MaxHP;

        HealthChangedEvent?.Invoke(currentHP, MaxHP);
        MaxHealthChangedEvent?.Invoke(MaxHP);
    }

    public void Hit(SkillHit hit)
    {
        if (alive)
        {
            // APPLY TARGET DEPENDANT MODIFIERS
            hit.ApplyTargetModifiers( gameObject );

            // GET DAMAGE OVER TIME EFFECTS
            AddDamageOverTimeEffects(hit.damageOverTimeEffects);

            float totalDamage = 0;
            foreach (DamageInstance dI in hit.damageInstances)
                totalDamage += dI.damage;

            if (totalDamage > 0)
            {
                //Basic damage calculation
                totalDamage = 0;
                foreach (DamageInstance dI in hit.damageInstances)
                {
                    float defensiveStat = 0;
                    float offensiveStat = 0;
                    if (dI.damageClass == DamageClass.PHYSICAL)
                    {
                        defensiveStat = _MyStats.GetStat(PrimaryStat.DEF);
                        offensiveStat = hit.strength;
                    }
                    else
                    {
                        defensiveStat = _MyStats.GetStat(PrimaryStat.WILL);
                        offensiveStat = hit.intelligence;
                    }
                    totalDamage += RolePlayingFormulas.DamageCalculation(dI.damage, offensiveStat, defensiveStat);
                }
                //Critical calculation
                float realCritChance = RolePlayingFormulas.CriticalChanceCalculation( hit.baseCritChance, hit.dextrity, _MyStats.GetStat(PrimaryStat.AGI) );
                bool critical = Random.value < realCritChance;
                if (critical)
                    totalDamage *= RolePlayingFormulas.CriticalBonusCalculation(hit.dextrity, hit.bonusCritDamage);

                //Actual hitpoints removal
                currentHP = currentHP - totalDamage;
                HealthChangedEvent?.Invoke(currentHP, MaxHP);

                //Life steal
                if (hit.lifeSteal > 0 && hit.attacker != null)
                {
                    HealHit healInstance = new HealHit();
                    healInstance.baseHeal = totalDamage * hit.lifeSteal;
                    healInstance.terapeucity = RolePlayingFormulas.TerapeucityFromWill( _MyStats.GetStat(PrimaryStat.WILL) );
                    hit.attacker.GetComponent<CreatureHitPoints>().Heal(healInstance);
                }

                //Hit visual effects
                Vector3 particlePoint = (hit.attackPosition - transform.position).normalized;
                particlePoint.y = 1.5f;
                Instantiate(_HitParticles, particlePoint + transform.position, Quaternion.Euler(particlePoint));

                // Display damage number on screen
                if (DamageAndPickupsDisplayManager.Ref != null) DamageAndPickupsDisplayManager.Ref.DisplayDamage(totalDamage, transform.position, _PlayerFlag, critical);

                ///
                ///DEATH
                ///
                if (currentHP <= 0)
                {
                    Die();
                }
            }
        }
    }

    public void Heal(HealHit heal)
    {
        if (alive)
        {
            float healAmount = RolePlayingFormulas.HealCalculation(heal.baseHeal, heal.terapeucity, 0);
            if (currentHP + healAmount >= MaxHP)
                currentHP = MaxHP;
            else
                currentHP += healAmount;

            HealthChangedEvent?.Invoke(currentHP, MaxHP);
            if (DamageAndPickupsDisplayManager.Ref != null) DamageAndPickupsDisplayManager.Ref.DisplayDamage(healAmount, transform.position, _PlayerFlag, false, true);
        }
        else
        {
            RecoverConsciousness(heal.baseHeal);
        }
    }

    #region DEATH_AND_UNCONSCIOUSNESS

    public void Die()
    {
        GetComponentInChildren<Animator>().SetTrigger("KnockedOut");

        if (tag != "Player")
        {
            List<Transform> tempTransformList = new List<Transform>();
            foreach ( Transform childTransform in transform)
                tempTransformList.Add(childTransform);

            foreach (Transform childTransform in tempTransformList)
            {
                float duration = 2f;

                childTransform.SetParent(null);
                Destroy(childTransform.gameObject, duration);
                
                TransformChange deathAnimationTransformChange = childTransform.gameObject.AddComponent<TransformChange>();
                deathAnimationTransformChange.enabled = false;
                NoPointerTransform noPointerTransform = new NoPointerTransform();
                noPointerTransform.scale = Vector3.zero;
                noPointerTransform.position = transform.position;
                noPointerTransform.rotation = transform.rotation;
                TransformChange.StartTransformChange(childTransform.gameObject, noPointerTransform, duration/2, duration/2, false, true, null, false);
            }

            Destroy(gameObject);
        }
        else
        {
            consciousness = 0;
            ConsciousnessChangedEvent?.Invoke(consciousness, MaxHP);
            currentHP = 0;
            alive = false;
            StartCoroutine(ConsciousnessRegeneration(_ConsciousnessRecoverInterval));
        }

        KnockOutEvent?.Invoke();
    }

    IEnumerator ConsciousnessRegeneration(float consciousnessRegainInterval)
    {
        while (!alive)
        {
            yield return new WaitForSeconds(consciousnessRegainInterval);

            RecoverConsciousness(consciousnessRecoveryRate * MaxHP * consciousnessRegainInterval);
        }
    }

    public void RecoverConsciousness( float amount )
    {
        consciousness += amount;

        ///
        /// KNOCKOUT RECOVERY
        ///
        if ( consciousness >= MaxHP )
        {
            AriseEvent?.Invoke();

            consciousness = 0;
            currentHP = MaxHP;
            HealthChangedEvent?.Invoke(currentHP, MaxHP);
            alive = true;
            GetComponentInChildren<Animator>().SetTrigger("Awake");
        }

        ConsciousnessChangedEvent?.Invoke(consciousness,MaxHP);
    }

    #endregion

    #region DAMAGE_OVER_TIME
    List<DamageOverTime> _DOTEffects = new List<DamageOverTime>();

    public const float _DamageOverTimeInterval = 0.5f;

    public delegate void AddedDamageOverTime(DamageOverTime damageOverTime);
    public event AddedDamageOverTime AddedDamageOverTimeEvent;
    public delegate void RemovedDamageOverTime(DamageOverTime damageOverTime);
    public event RemovedDamageOverTime RemovedDamageOverTimeEvent;

    private void AddDamageOverTimeEffects( List<DamageOverTime> dotEffects )
    {
        if (dotEffects != null && dotEffects.Count > 0)
        {
            foreach (DamageOverTime dotEffect in dotEffects)
            {
                DamageOverTime newDOT = CheckDamageOverTime(dotEffect);
                if (newDOT != null)
                {
                    if (newDOT.stacks < newDOT.maxStacks)
                        newDOT.stacks++;
                    newDOT.timeRemaining = dotEffect.duration;
                    AddedDamageOverTimeEvent?.Invoke(newDOT);
                }
                else
                {
                    newDOT = new DamageOverTime(dotEffect);
                    newDOT.timeRemaining = dotEffect.duration;
                    newDOT.stacks = 1;
                    AddDOT(newDOT);
                }
            }
        }
    }

    //Check an instance of the effect already exists
    public DamageOverTime CheckDamageOverTime(DamageOverTime damageOverTimeEffect)
    {
        foreach (DamageOverTime dot in _DOTEffects)
        {
            if (damageOverTimeEffect.skillRefference == dot.skillRefference)
                return dot;
        }
        return null;
    }

    public void AddDOT(DamageOverTime newDOT)
    {
        AddedDamageOverTimeEvent?.Invoke(newDOT);
        _DOTEffects.Add(newDOT);
    }

    public void RemoveDOT(DamageOverTime deletedDOT)
    {
        deletedDOT.stacks = 1;
        RemovedDamageOverTimeEvent?.Invoke(deletedDOT);
        _DOTEffects.Remove(deletedDOT);
    }

    public void UpdateDOTEffects()
    {
        //Update dot counters, apply damage and remove if expired
        for (int i = 0; i < _DOTEffects.Count; i++)
        {
            if (_DOTEffects[i].timed)
            {
                if (_DOTEffects[i].timeRemaining > 0)
                    _DOTEffects[i].timeRemaining -= _DamageOverTimeInterval;
                else
                    RemoveDOT(_DOTEffects[i]);
            }
        }

        //Apply damage
        for (int i = 0; i < _DOTEffects.Count; i++)
        {
            if (_DOTEffects[i].damageInstance.damage > 0)
            {
                DamageInstance newDamageInstance;
                newDamageInstance.damageClass = _DOTEffects[i].damageInstance.damageClass;
                newDamageInstance.type = _DOTEffects[i].damageInstance.type;
                newDamageInstance.damage = _DOTEffects[i].damageInstance.damage * _DamageOverTimeInterval * _DOTEffects[i].stacks;
                DamageOverTimeTick(newDamageInstance);
            }
            // Life regeneration
            else if (_DOTEffects[i].damageInstance.damage < 0)
            {
                HealHit newHealingInstance = new HealHit();
                newHealingInstance.baseHeal = -_DOTEffects[i].damageInstance.damage * _DamageOverTimeInterval * _DOTEffects[i].stacks;
                LifeRegenerationTick(newHealingInstance);
            }
        }
    }

    public void ClearAllDOTEffects()
    {
        for (int i = 0; i < _DOTEffects.Count; i++)
            RemoveDOT(_DOTEffects[i]);
    }

    public void DamageOverTimeTick(DamageInstance damageInstance)
    {
        float damage = 0;
        float defensiveStat = 0;
        if (damageInstance.type == DamageType.BLUNT || damageInstance.type == DamageType.PIERCE || damageInstance.type == DamageType.SLASH)
        {
            defensiveStat = _MyStats.GetStat(PrimaryStat.DEF);
        }
        else
        {
            defensiveStat = _MyStats.GetStat(PrimaryStat.WILL);
        }
        damage += damageInstance.damage * Mathf.Pow(0.99f, defensiveStat);
        if (DamageAndPickupsDisplayManager.Ref != null) DamageAndPickupsDisplayManager.Ref.DisplayDamage(damage, transform.position, _PlayerFlag);

        currentHP = currentHP - damage;
        HealthChangedEvent?.Invoke(currentHP, MaxHP);
        ///
        ///DEATH
        ///
        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void LifeRegenerationTick(HealHit healInstance)
    {
        float healing = 0;

        if (DamageAndPickupsDisplayManager.Ref != null) DamageAndPickupsDisplayManager.Ref.DisplayDamage(healing, transform.position, _PlayerFlag);

        currentHP = currentHP - healing;
        HealthChangedEvent?.Invoke(currentHP, MaxHP);
    }
    #endregion
}
