using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFactory : MonoBehaviour
{
    public GameObject BaseCard;
    public GameObject BaseStackableCard;
    public GameObject BasePackCard;

    public static GameFactory instance;

    private void Awake()
    {
        instance = this;
    }

    Vector3 ClampSpawnPosition(Vector3 position)
    {
        return GameUtility.ClampPointInBounds(GameManager.PlayableBounds, position);
    }

    public Card GetCard(Vector3 position, Transform parent = null, string name = "Card")
    {
        GameObject obj = Instantiate(BaseCard, ClampSpawnPosition(position), Quaternion.identity);
        obj.name = name;
        if (parent != null)
        {
            obj.transform.parent = parent.transform;
        }
        Card card = obj.GetComponent<Card>();
        return card;
    }

    public StackGroup GetEmptyStackGroup(Vector3 position, Transform parent = null, string name = "Stack Group")
    {
        GameObject obj = new GameObject(name);
        obj.transform.position = ClampSpawnPosition(position);
        if (parent != null)
        {
            obj.transform.parent = parent.transform;
        }
        StackGroup newGroup = obj.AddComponent<StackGroup>();
        return newGroup;
    }

    public StackableCard GetStackableCard(Vector3 position, Transform parent = null, string name = "Stackable")
    {
        GameObject obj = Instantiate(BaseStackableCard, ClampSpawnPosition(position), Quaternion.identity);
        obj.name = name;
        if (parent != null)
        {
            obj.transform.parent = parent.transform;
        }
        StackableCard card = obj.GetComponentInChildren<StackableCard>();
        return card;
    }

    public PackCard GetPackCard(Vector3 position, Transform parent = null, string name = "Card")
    {
        GameObject obj = Instantiate(BasePackCard, ClampSpawnPosition(position), Quaternion.identity);
        obj.name = name;
        if (parent != null)
        {
            obj.transform.parent = parent.transform;
        }
        PackCard card = obj.GetComponent<PackCard>();
        return card;
    }
}
