using System.Collections.Generic;
using UnityEngine;

public class CharacterCoordinator : MonoBehaviour
{
    [SerializeField] List<CharacterContext> m_characters;
    private void Start()
    {
        foreach(CharacterContext character in m_characters)
        {
            character.Initialize();
        }
    }
}
