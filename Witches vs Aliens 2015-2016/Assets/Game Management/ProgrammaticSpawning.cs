using UnityEngine;
using System.Collections;

public class ProgrammaticSpawning : MonoBehaviour, IObserver<Message> {

    [SerializeField]
    protected GameObject PuckPrefab;
    [SerializeField]
    protected PlayerComponents[] playerComponentPrefabs;

    [SerializeField]
    [AutoLink(parentTag = Tags.stage, parentName = "Left")]
    protected Transform leftRespawnPointsParent;
    [SerializeField]
    [AutoLink(parentTag = Tags.stage, parentName = "Right")]
    protected Transform rightRespawnPointsParent;

    [SerializeField]
    [AutoLink(parentTag = Tags.stage, parentName = "PuckRespawnPoint")]
    protected Transform puckRespawnPoint;

    [SerializeField]
    protected float resetDuration;

    [SerializeField]
    protected float goalToResetTime;

    Vector2[] leftPoints;
    Vector2[] rightPoints;
    Transform[] players;
    PuckFX puck;

    // Use this for initialization
    void Awake () {
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

        puck = ((GameObject)(Instantiate(PuckPrefab, puckRespawnPoint.position, Quaternion.identity))).GetComponent<PuckFX>();

        players = new Transform[playerComponentPrefabs.Length];
        for (int i = 0; i < playerComponentPrefabs.Length; i++)
        {
            GameObject spawnedPlayer = (GameObject)Instantiate(playerComponentPrefabs[i].basePlayer, i % 2 == 0 ? leftPoints[i % 2] : rightPoints[i % 2], Quaternion.identity); //the positions are temporary
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
        Callback.FireForNextFrame(() => resetPositions(), this);
	}

    void resetPositions()
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
        puck.Respawn(puckRespawnPoint.position);
    }

    public void Notify(Message m)
    {
        switch (m.messageType)
        {
            case GoalScoredMessage.classMessageType:
                Callback.FireAndForget(() => resetPositions(), goalToResetTime, this);
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