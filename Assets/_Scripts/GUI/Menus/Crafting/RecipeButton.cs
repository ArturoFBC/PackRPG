using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RecipeButton : MonoBehaviour
{
    private BaseRecipe myRecipe;
    [SerializeField] private Text nameLabel;
    [SerializeField] private Toggle myToggle;

    public delegate void ToggleDelegate( BaseRecipe selectedRecipe);
    public ToggleDelegate myToggleDelegate;

    public void Awake()
    {
        if (nameLabel == null)
            nameLabel = GetComponentInChildren<Text>();

        if (myToggle == null)
            myToggle = GetComponentInChildren<Toggle>();

        myToggle.onValueChanged.AddListener(OnToggle);
    }

    public void Set( BaseRecipe assignedRecipe, ToggleGroup myToggleGroup, ToggleDelegate onToggleActivated )
    {
        myRecipe = assignedRecipe;

        nameLabel.text = myRecipe.name;

        myToggleDelegate = onToggleActivated;
        myToggle.group = myToggleGroup;
    }

    public void OnToggle(bool activated)
    {
        if (activated)
            myToggleDelegate(myRecipe);
    }
}
