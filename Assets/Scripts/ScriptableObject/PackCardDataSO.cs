using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Pack Card Data", menuName="Pack Card Data")]
public class PackCardDataSO : CardDataSO
{
    [SerializeField] CardDataSO[] _packContents;
    public CardDataSO[] PackContent => _packContents;

    [SerializeField][Min(1)] int _contentOutput = 1;
    public int ContentOutput => _contentOutput;
}
