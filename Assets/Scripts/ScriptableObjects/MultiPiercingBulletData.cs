using UnityEngine;

[CreateAssetMenu(menuName = nameof(BulletData) 
    + "/" + nameof(MultiPiercingBulletData)
    )]
public class MultiPiercingBulletData : BulletData
{
    [SerializeField] int piercingCount = 3;

    public int PiercingCount => piercingCount;
}
