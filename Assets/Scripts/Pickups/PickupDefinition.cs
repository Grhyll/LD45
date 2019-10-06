using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PickupType
{
    Category = 0, 
    Effect = 1,
}
public enum PickupEffect
{
    None = 0,
    Damage = 1,

}
public abstract class PickupDefinition 
{
    public abstract PickupType type { get; }

    public abstract Sprite GetSprite();
    public abstract Color GetBackgroundColor();
    public abstract string GetDescription();
}
public class PickupCategoryDefinition : PickupDefinition
{
    public override PickupType type { get { return PickupType.Category; } }
    public CardCategory category { get; private set; }

    public PickupCategoryDefinition(CardCategory _category)
    {
        category = _category;
    }

    public override Sprite GetSprite()
    {
        return GlobalGameManager.Instance.cardBaseDefinitionsLibrary.GetCategoryInfo(category).sprite;
    }
    public override Color GetBackgroundColor()
    {
        return Color.white;
    }
    public override string GetDescription()
    {
        return GlobalGameManager.Instance.cardBaseDefinitionsLibrary.GetCategoryInfo(category).pickupDescription;
    }
}
public class PickupEffectDefinition : PickupDefinition
{
    public override PickupType type { get { return PickupType.Effect; } }
    public PickupEffect effect { get; private set; }

    public PickupEffectDefinition(PickupEffect _effect)
    {
        effect = _effect;
    }

    public override Sprite GetSprite()
    {
        return GlobalGameManager.Instance.cardBaseDefinitionsLibrary.GetPickupEffectInfo(effect).sprite;
    }
    public override Color GetBackgroundColor()
    {
        return GlobalGameManager.Instance.cardBaseDefinitionsLibrary.GetPickupEffectInfo(effect).color;
    }
    public override string GetDescription()
    {
        return GlobalGameManager.Instance.cardBaseDefinitionsLibrary.GetPickupEffectInfo(effect).pickupDescription;
    }
}
