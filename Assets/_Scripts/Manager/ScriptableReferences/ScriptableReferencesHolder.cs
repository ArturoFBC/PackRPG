using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableReferencesHolder : MonoBehaviour
{
    public static ScriptableReferencesHolder Ref;

    // This class will hold references to scriptable objects so when saving and loading an index can be saved to reference them
    [SerializeField]
    private ScriptableSpeciesList species;
    private List<Species> _Species;
    [SerializeField]
    private ScriptableItemList items;
    private List<BaseItem> _Items;
    [SerializeField]
    private ScriptableSkillList skills;
    private List<BaseSkill> _Skills;
    [SerializeField]
    private ScriptableAreaList areas;
    private List<Area> _Areas;
    [SerializeField]
    private ScriptableRecipeList recipes;
    private List<BaseRecipe> _Recipes;

    void Awake()
    {
        if (Ref == null)
            Ref = this;
        else
        {
            Destroy(this);
            return;
        }

        _Species = species.species;
        _Items = items.items;
        _Skills = skills.skills;
        _Areas = areas.areas;
        _Recipes = recipes.recipes;
    }

    #region SPECIES
    public static Species GetSpeciesReference(int index)
    {
        if (Ref._Species.Count > index && index >= 0)
            return Ref._Species[index];
        else
        {
            Debug.LogError("Index out of range when trying to access Species " + index );
            return null;
        }
    }

    public static int GetSpeciesIndex(Species species)
    {
        if (Ref._Species.Contains(species))
            return Ref._Species.IndexOf(species);
        else
        {
            Debug.LogError("Can not find species in the scriptableReferenceHolder species list" + species.name);
            return -1;
        }
    }

    public static IList<Species> GetSpeciesList()
    {
        return Ref._Species.AsReadOnly();
    }
    #endregion

    #region ITEMS
    public static BaseItem GetItemReference(int index)
    {
        if (Ref._Items.Count > index && index >= 0)
            return Ref._Items[index];
        else
        {
            if ( index != -1 )
                Debug.LogError("Index out of range when trying to access Item " + index);

            return null;
        }
    }

    public static int GetItemIndex(BaseItem item)
    {
        if (item == null)
            return -1;

        if (Ref._Items.Contains(item))
            return Ref._Items.IndexOf(item);
        else
        {
            Debug.LogError("Can not find species in the scriptableReferenceHolder item list" + item.name);
            return -2;
        }
    }
    #endregion

    #region SKILLS
    public static BaseSkill GetSkillReference(int index)
    {
        if (Ref._Skills.Count > index && index >= 0)
            return Ref._Skills[index];
        else
        {
            if (index != -1)
                Debug.LogError("Index out of range when trying to access skill " + index);

            return null;
        }
    }

    public static int GetSkillIndex(BaseSkill skill)
    {
        if (skill == null)
            return -1;

        if (Ref._Skills.Contains(skill))
            return Ref._Skills.IndexOf(skill);
        else
        {
            Debug.LogError("Can not find species in the scriptableReferenceHolder skill list" + skill.name);
            return -2;
        }
    }
    #endregion

    #region AREAS
    public static Area GetAreaReference(int index)
    {
        if (Ref._Areas.Count > index && index >= 0)
            return Ref._Areas[index];
        else
        {
            Debug.LogError("Index out of range when trying to access Area " + index);
            return null;
        }
    }

    public static int GetAreaIndex(Area area)
    {
        if (Ref._Areas.Contains(area))
            return Ref._Areas.IndexOf(area);
        else
        {
            Debug.LogError("Can not find area in the scriptableReferenceHolder area list" + area.name);
            return -1;
        }
    }
    #endregion

    #region RECIPES
    public static BaseRecipe GetRecipeReference(int index)
    {
        if (Ref._Recipes.Count > index && index >= 0)
            return Ref._Recipes[index];
        else
        {
            Debug.LogError("Index out of range when trying to access Recipe " + index);
            return null;
        }
    }

    public static int GetRecipeIndex(BaseRecipe recipe)
    {
        if (Ref._Recipes.Contains(recipe))
            return Ref._Recipes.IndexOf(recipe);
        else
        {
            Debug.LogError("Can not find area in the scriptableReferenceHolder recipe list" + recipe.name);
            return -1;
        }
    }
    #endregion
}