using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum LifeOrMana
{
    LIFE,
    MANA
}

public class ApplyEffectOnLifeOrManaChange : MonoBehaviour
{
    CreatureLife _MyCreatureLife;
    CreatureMana _MyCreatureMana;

    CreatureStats _MyCreatureStats;

    public Buff _MyBuff;

    public LifeOrMana lifeOrMana = LifeOrMana.LIFE;
    float _ActivationThreshold = 0.5f;

    bool activateWhenGreater = false;

    bool applied = false;

    public void OnEnable()
    {
        if (lifeOrMana == LifeOrMana.LIFE)
        {
            if (_MyCreatureLife == null)
                _MyCreatureLife = GetComponent<CreatureLife>();

            _MyCreatureLife.healthChangedEvent += ResourceChanged;
        }
        else
        {
            if (_MyCreatureMana == null)
                _MyCreatureMana = GetComponent<CreatureMana>();

            _MyCreatureMana.manaChangedEvent += ResourceChanged;
        }


        if (_MyCreatureStats == null)
            _MyCreatureStats = GetComponent<CreatureStats>();
    }

    public void OnDisable()
    {
        if (lifeOrMana == LifeOrMana.LIFE)
            _MyCreatureLife.healthChangedEvent -= ResourceChanged;
        else
            _MyCreatureMana.manaChangedEvent -= ResourceChanged;
    }

    public void ResourceChanged(float value)
    {
        float percentage = value / (lifeOrMana == LifeOrMana.LIFE ? _MyCreatureLife.MaxHP : _MyCreatureMana.MaxMP );

        if ( ( ( percentage > _ActivationThreshold) == activateWhenGreater ) && !applied )
            Activate();
        else if (((percentage > _ActivationThreshold) != activateWhenGreater) && applied)
            Deactivate();
    }

    public void Activate()
    {
        _MyCreatureStats.AddBuff(_MyBuff);
        applied = true;
    }

    public void Deactivate()
    {
        _MyCreatureStats.RemoveBuff(_MyBuff);
        applied = false;
    }
}

