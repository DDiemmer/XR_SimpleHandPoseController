using UnityEngine;

[CreateAssetMenu(fileName = "HandSimulatorPrefabs")]
public class HandSimulatorPrefabs : ScriptableObject
{
    public HandSimulatorPrefabs()
    {
        Instance = this;
    }
    public static HandSimulatorPrefabs Instance { get; private set; }

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        Instance = Resources.Load<HandSimulatorPrefabs>("HandSimulatorPrefabs");
    }
    public GameObject HandSimulator;
}