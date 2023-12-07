using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DropsItems : MonoBehaviour
{
    [SerializeField]
    protected float rarityBonus = 0;

    protected abstract List<Drop> GetDrops();

    protected virtual void DropItems()
    {
        List<Drop> drops = GetDrops();

        for (int i = 0; i < drops.Count; i++)
        {
            Vector3 spawnPosition = PositionTools.GetSpiralPosition(transform.position, i+1, 360 / drops.Count, 1.5f, 0f);
            PickUpItem pickUpItem = Instantiate(IconsAndEffects._Ref.ItemDropping, transform.position, transform.rotation).GetComponent<PickUpItem>();
            pickUpItem.Set(drops[i], spawnPosition);
        }
    }

    protected List<Drop> GetItemDrops()
    {
        List<ItemDropRate> possibleDrops = DataManager._CurrentArea.GetDropRates();

        List<Droppable> drops = GetChangeWeigtedDrops(possibleDrops);

        List<Drop> dropsWithAmount = new List<Drop>();

        foreach (Droppable drop in drops)
        {
            dropsWithAmount.Add(new Drop()
            {
                amount = 1,
                dropable = drop
            });
        }

        return dropsWithAmount;
    }

    protected List<Droppable> GetChangeWeigtedDrops(List<ItemDropRate> possibleDrops)
    {
        List<Droppable> actualDrops = new List<Droppable>();

        List<BaseRecipe> unlockedRecipes = InventoryManager.Ref.GetUnlockedRecipeList();
        foreach (ItemDropRate dropRate in possibleDrops)
        {
            if ( (dropRate.drop is BaseRecipe) &&
                 ( unlockedRecipes.Contains( (BaseRecipe)dropRate.drop) ) )
                continue;

            if (RolePlayingFormulas.DropRateCalculation(dropRate.rate, rarityBonus) > Random.value)
                actualDrops.Add(dropRate.drop);
        }

        return actualDrops;
    }
}
