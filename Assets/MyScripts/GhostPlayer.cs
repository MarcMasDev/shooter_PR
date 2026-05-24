using UnityEngine;

public class GhostPlayer : MonoBehaviour
{
    public GhostLapData ghostData;
    [SerializeField] private Transform ghostCar;
    [SerializeField] private TimeController timer;

    private float ghostTime = 0;

    private int currentSample = 0;

    private Vector3 lastPos;
    private Vector3 nextPos;

    private Quaternion lastRot;
    private Quaternion nextRot;

    private void Update()
    {
        if (ghostData == null || ghostData.carTimes.Count == 0) return;
        if (ghostTime >= ghostData.carTimes[ghostData.carTimes.Count - 1]) return;

        ghostTime += Time.deltaTime;

        while (currentSample < ghostData.carTimes.Count - 2 && ghostData.carTimes[currentSample + 1] < ghostTime) 
            currentSample++;


        Vector3 p1 = ghostData.carPositions[currentSample];
        Vector3 p2 = ghostData.carPositions[currentSample + 1];

        Quaternion r1 = ghostData.carRotations[currentSample];
        Quaternion r2 = ghostData.carRotations[currentSample + 1];

        float t1 = ghostData.carTimes[currentSample];
        float t2 = ghostData.carTimes[currentSample + 1];

        float lerp = Mathf.InverseLerp(t1, t2, ghostTime);

        ghostCar.position = Vector3.Lerp(p1, p2, lerp);
        ghostCar.rotation = Quaternion.Slerp(r1, r2, lerp);
    }


    public void StartGhost()
    {
        if (ghostData.carTimes.Count < 2) return;

        ghostTime = 0;
        currentSample = 0;

        ghostCar.position = ghostData.carPositions[0];
        ghostCar.rotation = ghostData.carRotations[0];
    }
}
