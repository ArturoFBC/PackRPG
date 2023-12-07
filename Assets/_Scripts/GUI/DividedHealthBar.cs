using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GaugeData
{
    LIFE,
    MANA,
    EXPERIENCE
}

public class DividedHealthBar : MonoBehaviour
{
    private GameObject _Target;
    private GaugeData _Data;

    public Color divisionColor = new Color(0f, 0f, 0f, 0.8f);

    public Color delayedGaugeColorIncreasing = Color.green;
    public Color delayedGaugeColorDecreasing = Color.white;

    public float _DelayedGaugeCatchUpSpeed = 0.1f;

    public RectTransform _LifeGauge;
    public RectTransform _DelayedGauge;
    private Image _DelayedGaugeImage;
    List<RectTransform> barDivisions = new List<RectTransform>();

    float maxValue;

    private void Awake()
    {
        if ( _DelayedGauge != null )
            _DelayedGaugeImage = _DelayedGauge.GetComponent<Image>();
    }

    public void SetTarget( GameObject target, GaugeData dataToDisplay )
    {
        ClearTarget();
        _Target = target;
        _Data = dataToDisplay;

        switch (_Data)
        {
            case GaugeData.EXPERIENCE:
                Level targetLevel = _Target.GetComponent<Level>();
                SetMaxValue(Level.ExpLevels[targetLevel.level + 1] - Level.ExpLevels[targetLevel.level]);
                SetValue(targetLevel.exp - Level.ExpLevels[targetLevel.level], Level.ExpLevels[targetLevel.level + 1] - Level.ExpLevels[targetLevel.level]);
                targetLevel.maxExpChangedEvent += SetMaxValue;
                targetLevel.expChangedEvent += SetValue;
                break;

            case GaugeData.LIFE:
                CreatureHitPoints targetLife = _Target.GetComponent<CreatureHitPoints>();
                SetMaxValue(targetLife.MaxHP);
                SetValue(targetLife.currentHP, targetLife.MaxHP);
                targetLife.MaxHealthChangedEvent += SetMaxValue;
                targetLife.HealthChangedEvent += SetValue;
                break;

            case GaugeData.MANA:
                CreatureMana targetMana = _Target.GetComponent<CreatureMana>();
                SetMaxValue(targetMana.MaxMP);
                SetValue(targetMana.currentMP, targetMana.MaxMP);
                targetMana.maxManaChangedEvent += SetMaxValue;
                targetMana.manaChangedEvent += SetValue;
                break;
        }
    }

    public void ClearTarget()
    {
        if (_Target != null)
        {
            switch (_Data)
            {
                case GaugeData.EXPERIENCE:
                    Level targetLevel = _Target.GetComponent<Level>();
                    targetLevel.maxExpChangedEvent -= SetMaxValue;
                    targetLevel.expChangedEvent += SetValue;
                    break;

                case GaugeData.LIFE:
                    CreatureHitPoints targetLife = _Target.GetComponent<CreatureHitPoints>();
                    targetLife.MaxHealthChangedEvent -= SetMaxValue;
                    targetLife.HealthChangedEvent -= SetValue;
                    break;

                case GaugeData.MANA:
                    CreatureMana targetMana = _Target.GetComponent<CreatureMana>();
                    targetMana.maxManaChangedEvent -= SetMaxValue;
                    targetMana.manaChangedEvent -= SetValue;
                    break;
            }
        }
    }

    public void SetMaxValue(float max_value)
    {
        if (max_value == maxValue)
            return;

        maxValue = max_value;
        float totalWidth = ((RectTransform)_LifeGauge.parent).rect.width;
        int barAmountNeeded = Mathf.FloorToInt(maxValue / 100);


        for (int i = 0; i < barAmountNeeded; i++)
        {
            if (barDivisions.Count < i + 1)
            {
                GameObject healthBarDivision = new GameObject("HealthBarDivision");
                RectTransform rect = healthBarDivision.AddComponent<RectTransform>();
                Image image = healthBarDivision.AddComponent<Image>();
                rect.SetParent(_LifeGauge.parent);
                rect.anchorMin = new Vector2(0f, 0f);
                rect.anchorMax = new Vector2(0f, 1f);
                float thickness = (i % 10 == 0 && i != 0 ? 2f : 1f);
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, thickness);
                image.color = divisionColor;
                barDivisions.Add(rect);
            }

            float xPosition = (i + 1) * 100 * totalWidth / maxValue;
            barDivisions[i].anchoredPosition = new Vector2(xPosition, 0f);
            barDivisions[i].gameObject.SetActive(true);
        }

        if (barDivisions.Count > barAmountNeeded)
        {
            for (int i = barAmountNeeded; i < barDivisions.Count; i++)
                barDivisions[i].gameObject.SetActive(false);
        }
    }

    public void SetValue( float value, float max_value)
    {
        if ( maxValue != 0 )
            _LifeGauge.localScale = new Vector3(value / max_value, 1f, 1f);
    }

	// Update is called once per frame
	void Update ()
    {
       
        if ( _DelayedGauge != null && _LifeGauge.localScale.x != _DelayedGauge.localScale.x)
        {
            //if (_EnemyLifeGauge.localScale.x > _DelayedGauge.localScale.x)
            //    _DelayedGaugeImage.color = delayedGaugeColorIncreasing;
            //else
            //    _DelayedGaugeImage.color = delayedGaugeColorDecreasing;

            float difference = (_DelayedGauge.localScale.x - _LifeGauge.localScale.x);

            if ( difference > 0.01)
                _DelayedGauge.localScale = new Vector3(_DelayedGauge.localScale.x - (difference * _DelayedGaugeCatchUpSpeed),   1f, 1f);
            else
                _DelayedGauge.localScale = new Vector3(_LifeGauge.localScale.x, 1f, 1f);
        }

    }

    private void OnDestroy()
    {
        ClearTarget();
    }
}
