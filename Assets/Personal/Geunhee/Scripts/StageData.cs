using UnityEngine;

[CreateAssetMenu(fileName = "Stage Data", menuName = "Scriptable Object/Stage Data", order = int.MaxValue)]
public class StatgeData : ScriptableObject
{
    [SerializeField]
    public int stageNumber;

    
}