using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureMana : MonoBehaviour {

    public float MaxMP;
    public float currentMP;
    public float regenMP;

    public CreatureStats _MyStats;

    public delegate void ManaChanged(float mana, float maxMana);
    public event ManaChanged manaChangedEvent;
    public delegate void MaxManaChanged(float mana);
    public event MaxManaChanged maxManaChangedEvent;

    void Awake()
    {
        if (_MyStats == null)
            _MyStats = GetComponent<CreatureStats>();

        _MyStats._StatEvents[(int)PrimaryStat.MP] += OnBaseMPChange;
    }

    public void OnBaseMPChange(float baseMP)
    {
        //This provides scaling with level plus a base wich is dependent on base stats from level one
        UpdateMaxMP(baseMP + (baseMP / GetComponent<Level>().level) * 10f);
    }

    public void UpdateMaxMP(float newMaxMP)
    {
        float mpDifference = newMaxMP - MaxMP;

        MaxMP = newMaxMP;

        //Increase current HP when maxhp increases, also ensure that the current hp is never highter than the total hp
        if (mpDifference > 0)
            currentMP += mpDifference;
        else if (currentMP > MaxMP)
            currentMP = MaxMP;

        manaChangedEvent?.Invoke(currentMP, MaxMP);
        maxManaChangedEvent?.Invoke(MaxMP);
    }

    public void AddMana( float manaAmount )
    {
        currentMP = Mathf.Min(currentMP + manaAmount, MaxMP);
        DamageAndPickupsDisplayManager.Ref.DisplayManaGain(manaAmount, transform.position);
        manaChangedEvent?.Invoke(currentMP, MaxMP);
    }

    public void ConsumeMana( float manaAmount )
    {
        currentMP -= manaAmount;
        manaChangedEvent?.Invoke(currentMP, MaxMP);
    }

    public void Update()
    {
        float previousMP = currentMP;

        currentMP += regenMP * Time.deltaTime;

        if (currentMP > MaxMP)
            currentMP = MaxMP;

        if (previousMP != currentMP)
            manaChangedEvent?.Invoke(currentMP, MaxMP);
    }
}
