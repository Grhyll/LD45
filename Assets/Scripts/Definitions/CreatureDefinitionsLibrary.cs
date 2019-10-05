using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CreatureType
{
    MC = 0,

    Enemy1 = 200,
}

[CreateAssetMenu(fileName = "CreatureDefinitionsLibrary", menuName = "LD45/Create creature definitions library")]
public class CreatureDefinitionsLibrary : ScriptableObject
{
    public CreatureDefinition[] definitions;

    public CreatureDefinition GetDefinition(CreatureType creatureType)
    {
        for (int i = 0; i < definitions.Length; i++)
        {
            if (definitions[i].creatureType == creatureType)
            {
                return definitions[i];
            }
        }
        Debug.LogError("Error: trying to get creature definition for type " + creatureType + " but it isn't available in library.");
        return null;
    }
}

[System.Serializable]
public class CreatureDefinition
{
    public CreatureType creatureType;

    public Sprite sprite;

    public int baseHealth;
    public int baseDamage;
    public int baseRange;
    public int baseMoves;

}
