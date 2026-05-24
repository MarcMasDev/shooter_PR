using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GhostLapData", menuName = "Scriptable Objects/GhostLapData")]
public class GhostLapData : ScriptableObject
{
    public List<Vector3> carPositions = new List<Vector3>();
    public List<Quaternion> carRotations = new List<Quaternion>();
    public List<float> carTimes = new List<float>();
    public List<float> checkpointTimes = new List<float>();
    public float raceTime = Mathf.Infinity;
    public void AddNewData(Transform t, float time)
    {
        carPositions.Add(t.position);
        carRotations.Add(t.rotation);
        carTimes.Add(time);
    }

    public void GetDataAt(int sample, out Vector3 pos, out Quaternion rot)
    {
        pos = carPositions[sample];
        rot = carRotations[sample];
    }

    public void ResetData()
    {
        carPositions.Clear();
        carRotations.Clear();
        carTimes.Clear();
        checkpointTimes.Clear();
    }

    public void SetData(GhostLapData newData, float time)
    {
        ResetData();

        for (int i = 0; i < newData.carPositions.Count; ++i)
        {
            carPositions.Add(newData.carPositions[i]);
        }
        for (int i = 0; i < newData.carRotations.Count; ++i)
        {
            carRotations.Add(newData.carRotations[i]);
        }
        for (int i = 0; i < newData.carTimes.Count; ++i)
        {
            carTimes.Add(newData.carTimes[i]);
        }
        for (int i = 0; i < newData.checkpointTimes.Count; ++i)
        {
            checkpointTimes.Add(newData.checkpointTimes[i]);
        }

        raceTime = time;
    }
}
