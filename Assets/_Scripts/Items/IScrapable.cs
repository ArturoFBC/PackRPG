using UnityEngine;

public interface IScrapable
{
    int GetCurrencyValue();

    void Scrap(int amount);

    Sprite GetIcon();
}