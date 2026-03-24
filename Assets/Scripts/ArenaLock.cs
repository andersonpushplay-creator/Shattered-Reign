using UnityEngine;

public class ArenaLock : MonoBehaviour
{
    public GameObject leftWall;
    public GameObject rightWall;

    public void LockArena()
    {
        if (leftWall != null) leftWall.SetActive(true);
        if (rightWall != null) rightWall.SetActive(true);

        Debug.Log("Arena travada!");
    }

    public void UnlockArena()
    {
        if (leftWall != null) leftWall.SetActive(false);
        if (rightWall != null) rightWall.SetActive(false);

        Debug.Log("Arena liberada!");
    }
}