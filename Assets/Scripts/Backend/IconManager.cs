using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IconManager
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Sprite missingIcon;

    public Sprite GetIcon(int id)
    {
        if (id < 1 && id > sprites.Length + 1)
            return missingIcon;
        return sprites[id - 1];
    }
}
