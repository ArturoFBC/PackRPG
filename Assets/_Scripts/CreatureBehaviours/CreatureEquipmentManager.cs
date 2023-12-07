using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    CHARM,
    ACCESORY
}

[System.Serializable]
public class EquipmentSlot
{
    public EquipmentType type;

    [SerializeField] private bool empty = true;
    [SerializeField] private Item _item;
    public Item item
    {
        set
        {
            Item oldItem = _item;
            empty = (value == null);
            _item = empty ? null : ItemFactory.CreateItem(value.GetSaveData());
            ItemChangedEvent?.Invoke( empty ?  null : _item, oldItem);
        }
        get
        {
            return empty ? null : _item;
        }
    }

    public delegate void ItemChanged(Item newItem, Item oldItem );
    public event ItemChanged ItemChangedEvent;
}

public class CreatureEquipmentManager : MonoBehaviour
{
    private bool initialized = false;

    private List<EquipmentSlot> _Slots = new List<EquipmentSlot>();
    private Dictionary<Item, EquippedItemEffect> _EquipmentBehaviours = new Dictionary<Item, EquippedItemEffect>();

    public delegate void EquipmentChanged();
    public event EquipmentChanged EquipmentChangedEvent;

    void Start()
    {
        CreatureStats myStats = GetComponent<CreatureStats>();
        if (myStats != null)
        {
            InitializeEquipment(myStats.GetSpecimen());
        }
    }

    public void InitializeEquipment( Specimen mySpecimen)
    {
        if (initialized == false)
        {
            foreach ( EquipmentType equipmentType in mySpecimen.species.equipmentSlots )
            {
                EquipmentSlot equipmentSlot = new EquipmentSlot();
                equipmentSlot.type = equipmentType;
                _Slots.Add(equipmentSlot);
            }

            for (int i = 0; i < mySpecimen.equipmentSlots.Count; i++)
            {
                if (mySpecimen.equipmentSlots[i].item != null)
                {
                    for (int j = 0; j < _Slots.Count; j++)
                    {
                        if (_Slots[j].item == null && _Slots[j].type == mySpecimen.equipmentSlots[i].type)
                        {
                            EquipItem(mySpecimen.equipmentSlots[i].item, _Slots[j]);
                        }

                    }
                }
            }

            for (int i = 0; i < mySpecimen.equipmentSlots.Count; i++)
            {
                mySpecimen.equipmentSlots[i].ItemChangedEvent += OnSpecimenEquipmentChange;
            }

            initialized = true;

            EquipmentChangedEvent?.Invoke();
        }
    }

    public void OnSpecimenEquipmentChange(Item item, Item oldItem)
    {
        if ( item != null )
            AddItemEffect(item);

        if (oldItem != null)
            RemoveItemEffect(oldItem);
    }


    public Item EquipItem(Item item, EquipmentSlot equipmentSlot)
    {
        Item unequippedItem = UnequipItem(equipmentSlot);
        equipmentSlot.item = ItemFactory.CreateItem( item.GetSaveData() );

        AddItemEffect(equipmentSlot.item);

        EquipmentChangedEvent?.Invoke();

        return unequippedItem;
    }

    public Item UnequipItem(EquipmentSlot equipmentSlot)
    {
        Item unequippedItem = equipmentSlot.item;
        equipmentSlot.item = null;

        RemoveItemEffect(unequippedItem);

        EquipmentChangedEvent?.Invoke();

        return unequippedItem;
    }


    private void AddItemEffect(Item item)
    {
        EquippedItemEffect newEffectBehaviour = EquippedItemEffect.AssignToCreature(gameObject, item);
        _EquipmentBehaviours.Add(item, newEffectBehaviour);
    }

    private void RemoveItemEffect(Item unequippedItem)
    {
        if (unequippedItem != null && _EquipmentBehaviours.ContainsKey(unequippedItem))
        {
            Destroy(_EquipmentBehaviours[unequippedItem]);
            _EquipmentBehaviours.Remove(unequippedItem);
        }
    }
}
