using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTooltipBox : MonoBehaviour
{
    public Text _Name, _Description, _Damage, _Critical, _Cooldown, _ManaCost, _Rank;
    public Image _Icon, _DamageType, _DamageClass;

    public void SetSkill(BaseSkill baseSkill)
    {
        // COMMON
        _Name.text = baseSkill.name;
        _Description.text = baseSkill.description;

        // ACTIVE SKILL
        if (baseSkill is BaseActiveSkill)
        {
            BaseActiveSkill baseActiveSkill = (BaseActiveSkill)baseSkill;

            _Icon.sprite = baseActiveSkill.icon;

            if (baseActiveSkill.damage != 0)
            {
                _Damage.text = baseActiveSkill.damage.ToString();
                _Critical.text = ((int)(baseActiveSkill.criticalChance * 100)).ToString()+"%";
            }
            else
            {
                _Damage.gameObject.SetActive(false);
                _Critical.gameObject.SetActive(false);
            }

            if (baseActiveSkill.manaCost != 0)
                _ManaCost.text = baseActiveSkill.manaCost.ToString();
            else
                _ManaCost.gameObject.SetActive(false);

            if (baseActiveSkill.cooldown != 0)
                _Cooldown.text = baseActiveSkill.cooldown.ToString();
            else
                _Cooldown.text = baseActiveSkill.executionDuration.ToString();


            _DamageType.sprite = IconsAndEffects._Ref.damageTypes[(int)baseActiveSkill.damageType];
            _DamageType.color = IconsAndEffects._Ref.damageTypeColors[(int)baseActiveSkill.damageType];

            switch (baseActiveSkill.damageClass)
            {
                case DamageClass.ETHEREAL:
                    _DamageClass.sprite = IconsAndEffects._Ref.baseStats[ (int)PrimaryStat.INT ];
                    break;
                case DamageClass.PHYSICAL:
                    _DamageClass.sprite = IconsAndEffects._Ref.baseStats[(int)PrimaryStat.STR];
                    break;
                default:
                    _DamageClass.sprite = IconsAndEffects._Ref.damageTypes[(int)DamageType.NONE];
                    break;
            }
        }

        // PASSIVE SKILL
        else if ( baseSkill is BasePassiveSkill )
        {
            BasePassiveSkill basePassiveSkill = (BasePassiveSkill)baseSkill;

            _Icon.sprite = baseSkill.icon;

            _Damage.    gameObject.SetActive(false);
            _Critical.  gameObject.SetActive(false);
            _Cooldown.  gameObject.SetActive(false);
            _ManaCost.  gameObject.SetActive(false);
            _DamageType.gameObject.SetActive(false);
            _DamageClass.gameObject.SetActive(false);
        }
    }
}