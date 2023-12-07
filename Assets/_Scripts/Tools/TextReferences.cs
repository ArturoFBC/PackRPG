using System.Collections.Generic;

public static class TextReferences
{
    private static Dictionary<string, string> texts;

    public static string GetText(string reference)
    {
        if (texts == null)
            InitializeTexts();

        return texts[reference];
    }


    private static void InitializeTexts()
    {
        texts = new Dictionary<string, string>();

    #region BUTTON TOOLTIPS
        texts.Add("@bait_button", "Press the button then a packling to bait it\n50 essence required to bait\nDefeat the baited packling to recruit it");
        texts.Add("@control_mode_description", "Switch control modes:\nCommand all: Sends move and attack command to all your packlings\nFollow: Sends move and attack commands only to selected packling, the rest follow");
        #endregion

    #region STAT DESCRIPTIONS
        texts.Add("@STR_description", "Strength: Increases strength based attack damage");
        texts.Add("@DEF_description", "Defense: Reduces strength based attack damage");
        texts.Add("@INT_description", "Intelligence: Increases intelligence based attack damage");
        texts.Add("@WIL_description", "Will: Reduces intelligence based attack damage\nIncreases healing done");
        texts.Add("@DEX_description", "Dextrity: Increases chances of dealing a critical hit\nIncreases critical hit damage");
        texts.Add("@AGI_description", "Agility: Reduces chances of receiving a critical hit\nReduces cooldowns and increases basic attack speed");
        texts.Add("@HP_description", "Health: Increases max hit points");
        texts.Add("@MP_description", "Stamina: Increases max energy points\nBase energy recovery is based on max energy");
        #endregion

        #region ITEM NAMES
        texts.Add("1_wood", "Conglomerate");
        texts.Add("2_wood", "Pine");
        texts.Add("3_wood", "Ebony");
        texts.Add("4_wood", "Oak");
        texts.Add("5_wood", "Boj");

        texts.Add("1_metal", "Copper");
        texts.Add("2_metal", "Iron");
        texts.Add("3_metal", "Bronze");
        texts.Add("4_metal", "Silver");
        texts.Add("5_metal", "Gold");

        texts.Add("1_crystal", "Plexiglass");
        texts.Add("2_crystal", "Window shard");
        texts.Add("3_crystal", "Onyx");
        texts.Add("4_crystal", "Quartz");
        texts.Add("5_crystal", "Varowski");

        texts.Add("1_gas", "Nitrogen");
        texts.Add("2_gas", "Oxygen");
        texts.Add("3_gas", "hydrogen");
        texts.Add("4_gas", "argon");
        texts.Add("5_gas", "helium");

        texts.Add("1_glue", "Sticky stuff");
        texts.Add("2_glue", "Goo");
        texts.Add("3_glue", "Wax");
        texts.Add("4_glue", "Glue");
        texts.Add("5_glue", "Resin");

        texts.Add("bait_name", "Bait");
        texts.Add("pill_name", "Pill");
        #endregion

        #region ITEM DESCRIPTIONS
        texts.Add("@bait_description", "Use on a creature of the appropiate species to capture");
        texts.Add("@pill_description", "Use on a creature to rise its phenotype on a certain stat");
        #endregion
    }
}
