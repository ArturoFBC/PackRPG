using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class AreaUnlock
{
    public Area area;
    public bool unlocked;

    public AreaUnlock(Area define_area, bool define_unlocked = false)
    {
        area = define_area ?? throw new System.ArgumentNullException("The area is used as an index and as such can not be null");
        unlocked = define_unlocked;
    }
}

//
// The whole class is a list wrapped as a dictionary so it appears in the inspector
//
[System.Serializable]
public class AreaList : IEnumerable, IEnumerator
{
    [SerializeField]
    private List<AreaUnlock> areas = new List<AreaUnlock>();

    public int Count
    {
        get { return areas.Count; }
        private set { }
    }

    object IEnumerator.Current => ((IEnumerator)areas).Current;

    public bool this[Area areaIndex]
    {
        get { return areas.Find(x => x.area == areaIndex).unlocked; }
        set
        {
            for (int i = 0; i < areas.Count; i++)
            {
                if (areas[i].area == areaIndex)
                {
                    areas[i].unlocked = value;
                    return;
                }
            }
            throw new System.IndexOutOfRangeException("The area used as key was not found on the list of areas");
        }
    }

    public void Clear()
    {
        areas.Clear();
    }

    public void Add(Area area, bool unlocked)
    {
        if (ContainsKey(area))
        {
            throw new System.ArgumentException("Area already in the area list");
        }
        else
        {
            AreaUnlock newEssenceValue = new AreaUnlock(area, unlocked);
            areas.Add(newEssenceValue);
        }
    }

    public bool Remove(Area areaToRemove)
    {
        return areas.Remove(areas.Find(x => x.area == areaToRemove));
    }

    public bool ContainsKey(Area area)
    {
        AreaUnlock foundEssence = areas.Find(x => x.area == area);

        return (foundEssence != null);
    }

    public IEnumerator GetEnumerator()
    {
        return ((IEnumerable)areas).GetEnumerator();
    }

    bool IEnumerator.MoveNext()
    {
        return ((IEnumerator)areas).MoveNext();
    }

    void IEnumerator.Reset()
    {
        ((IEnumerator)areas).Reset();
    }
}
