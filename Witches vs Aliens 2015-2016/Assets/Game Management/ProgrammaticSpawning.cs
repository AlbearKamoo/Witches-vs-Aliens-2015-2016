using UnityEngine;
using System.Collections;

public class ProgrammaticSpawning : MonoBehaviour, IObserver<Message> {

    //demonstration that we can actually do this

    public PlayerComponents[] playerComponentPrefabs;

    [SerializeField]
    [AutoLink(parentTag = Tags.stage, parentName = "Left")]
    protected Transform leftRespawnPointsParent;
    [SerializeField]
    [AutoLink(parentTag = Tags.stage, parentName = "Right")]
    protected Transform rightRespawnPointsParent;

    [SerializeField]
    protected float resetDuration;

    [SerializeField]
    protected float goalToResetTime;

    Vector2[] leftPoints;
    Vector2[] rightPoints;
    Transform[] players;

    // Use this for initialization
    void Start () {
        Observers.Subscribe(this, new string[]{GoalScoredMessage.classMessageType});

        leftPoints = new Vector2[leftRespawnPointsParent.childCount];
        int index = 0;
        foreach(Transform child in leftRespawnPointsParent)
        {
            leftPoints[index] = child.position;
            index++;
        }

        index = 0;
        rightPoints = new Vector2[rightRespawnPointsParent.childCount];
        foreach (Transform child in rightRespawnPointsParent)
        {
            rightPoints[index] = child.position;
            index++;
        }

        players = new Transform[playerComponentPrefabs.Length];
        for (int i = 0; i < playerComponentPrefabs.Length; i++)
        {
            GameObject spawnedPlayer = (GameObject)Instantiate(playerComponentPrefabs[i].basePlayer, Vector3.zero, Quaternion.identity);
            spawnedPlayer.AddComponent<Stats>().side = playerComponentPrefabs[i].side;
            switch (playerComponentPrefabs[i].inputMode)
            {
                case PlayerComponents.PlayerInputType.MOUSE:
                    spawnedPlayer.AddComponent<MousePlayerInput>().bindings = playerComponentPrefabs[i].bindings;
                    break;
                case PlayerComponents.PlayerInputType.JOYSTICK:
                    spawnedPlayer.AddComponent<JoystickPlayerInput>().bindings = playerComponentPrefabs[i].bindings;
                    break;
                case PlayerComponents.PlayerInputType.CRAPAI:
                    spawnedPlayer.AddComponent<CrappyAIInput>().bindings = playerComponentPrefabs[i].bindings;
                    break;
            }
            GameObject.Instantiate(playerComponentPrefabs[i].movementAbility).transform.SetParent(spawnedPlayer.transform, false);
            GameObject.Instantiate(playerComponentPrefabs[i].genericAbility).transform.SetParent(spawnedPlayer.transform, false);
            //GameObject.Instantiate(playerComponentPrefabs[i].superAbility).transform.SetParent(spawnedPlayer.transform);
            players[i] = spawnedPlayer.transform;
        }
        Callback.FireForNextFrame(() => resetPlayerPositions(), this);
	}

    void resetPlayerPositions()
    {
        leftPoints.Shuffle<Vector2>();
        rightPoints.Shuffle<Vector2>();
        int leftPointsIndex = 0;
        int rightPointsIndex = 0;
        foreach (Transform t in players)
        {
            switch (t.GetComponent<Stats>().side)
            {
                case Side.LEFT:
                    t.GetComponent<ResetScripting>().Reset(leftPoints[leftPointsIndex], resetDuration);
                    leftPointsIndex++;
                    break;
                case Side.RIGHT:
                    t.GetComponent<ResetScripting>().Reset(rightPoints[rightPointsIndex], resetDuration);
                    rightPointsIndex++;
                    break;
            }
        }

    }

    public void Notify(Message m)
    {
        switch (m.messageType)
        {
            case GoalScoredMessage.classMessageType:
                Callback.FireAndForget(() => resetPlayerPositions(), goalToResetTime, this);
                break;
        }
    }
}
[System.Serializable]
public class PlayerComponents
{
    public GameObject basePlayer;
    public GameObject movementAbility;
    public GameObject genericAbility;
    //public GameObject superAbility;

    public PlayerInputType inputMode;
    public Side side;
    public InputConfiguration bindings;
    public PlayerComponents() { }

    public enum PlayerInputType
    {
        MOUSE,
        JOYSTICK,
        CRAPAI
    }
}