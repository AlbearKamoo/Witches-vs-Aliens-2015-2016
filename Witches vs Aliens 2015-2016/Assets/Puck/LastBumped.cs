using UnityEngine;
using System.Collections;

public class LastBumped : MonoBehaviour, IObservable<BumpedSideChangedMessage>
{
    Observable<BumpedSideChangedMessage> _bumpedSideChangedObservable = new Observable<BumpedSideChangedMessage>();
    public Observable<BumpedSideChangedMessage> Observable(IObservable<BumpedSideChangedMessage> self) { return _bumpedSideChangedObservable; }
    //keeps track of who bumped the puck last
    Side _side;
    public Side side { get { return _side; } }
    Transform _lastBumpedPlayer;
    public Transform lastBumpedPlayer { get { return _lastBumpedPlayer; } }
    Transform _lastBumpedPlayerOpposingSide;
    /// <summary>
    /// The player who last touched the ball who is not on the same side as the lastBumpedPlayer
    /// </summary>
    public Transform lastBumpedPlayerOpposingSide { get { return _lastBumpedPlayerOpposingSide; } }
	// Use this for initialization
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.root.CompareTag(Tags.player))
        {
            setLastBumped(other.transform);
        }
    }

    public void setLastBumped(Transform player)
    {
        if (player != _lastBumpedPlayer)
        {
            Side newSide = player.GetComponent<Stats>().side;
            if (_side != newSide)
            {
                _side = newSide;
                //push current lastBumpedPlayer into _lastBumpedPlayerOpposingSide;
                _lastBumpedPlayerOpposingSide = _lastBumpedPlayer;
            }

            _lastBumpedPlayer = player.root;
            _bumpedSideChangedObservable.Post(new BumpedSideChangedMessage(_side, _lastBumpedPlayer));
        }
    }
}

public class BumpedSideChangedMessage
{
    public readonly Side side;
    public readonly Transform player;
    public BumpedSideChangedMessage(Side side, Transform player)
    {
        this.side = side;
        this.player = player;
    }
}