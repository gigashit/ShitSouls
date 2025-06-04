using System.Collections.Generic;
using UnityEngine;

public class InputKeyIconRandomizer : MonoBehaviour
{
    public List<Sprite> interactIcons;
    public List<Sprite> dodgeIcons;
    public List<Sprite> useItemIcons;
    public List<Sprite> attackIcons;
    public List<Sprite> runIcons;
    public List<Sprite> jumpIcons;

    public Sprite GetRandomIcon(List<Sprite> list)
    {
        return list[Random.Range(0, list.Count)];
    }
}
