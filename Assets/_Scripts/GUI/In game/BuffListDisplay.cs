using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffListDisplay : MonoBehaviour
{
    public GameObject _BuffDisplay;
    public CreatureStats _MyStats;
    Dictionary<StatModifier, GameObject> _Icons = new Dictionary<StatModifier, GameObject>();
    /*
    void OnEnable()
    {
        _MyStats.AddedBuffEvent += AddBuffIcon;
        _MyStats.RemovedBuffEvent += RemoveBuffIcon;
    }

    void OnDisable()
    {
        _MyStats.AddedBuffEvent -= AddBuffIcon;
        _MyStats.RemovedBuffEvent -= RemoveBuffIcon;
    }*/

    public void AddBuffIcon( StatModifier buff )
    {
        if ( _Icons.ContainsKey(buff) )
        {
            BuffIcon buffIcon = _Icons[buff].GetComponent<BuffIcon>();
            if (buff.maxStacks > 1)
            {
                buffIcon._StackNumberDisplay.SetActive( true );
                buffIcon._StackNumberText.text = buff.stacks.ToString();
            }
        }
        else
        {
            GameObject newBuffIcon = Instantiate(_BuffDisplay, transform);
            BuffIcon buffIcon = newBuffIcon.GetComponent<BuffIcon>();
            buffIcon._Background.color = buff.value > 0 ? Color.green : Color.red;
            buffIcon._Icon.sprite = buff.icon;
            if (buff.maxStacks > 1)
                buffIcon._StackNumberText.text = buff.stacks.ToString();
            buffIcon._StackNumberDisplay.SetActive(buff.maxStacks > 1);

            _Icons.Add(buff, newBuffIcon);
        }
    }

    public void RemoveBuffIcon(StatModifier buff)
    {
        if (_Icons.ContainsKey(buff))
        {
            Destroy(_Icons[buff]);
            _Icons.Remove(buff);
        }
    }
}
