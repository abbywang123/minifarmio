using UnityEngine;
using Unity.Netcode;
using TMPro;

public class TileNetworkSync : NetworkBehaviour
{
    [SerializeField] private TMP_Text label;

    private NetworkVariable<int> x = new NetworkVariable<int>();
    private NetworkVariable<int> y = new NetworkVariable<int>();
    private NetworkVariable<string> cropId = new NetworkVariable<string>();
    private NetworkVariable<int> growDays = new NetworkVariable<int>();

    public void SetTile(int _x, int _y, string _cropId, int _growDays)
    {
        x.Value = _x;
        y.Value = _y;
        cropId.Value = _cropId;
        growDays.Value = _growDays;
    }

    void Update()
    {
        if (label != null)
        {
            label.text = $"{cropId.Value}\n({x.Value},{y.Value})";
        }
    }
}

