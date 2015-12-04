using UnityEngine;
using System.Collections;

public class LastBumped : MonoBehaviour, IObservable<BumpedSideChangedMessage>
{
    Observable<BumpedSideChangedMessage> _bumpedSideChangedObservable = new Observable<BumpedSideChangedMessage>();
    public Observable<BumpedSideChangedMessage> Observable(IObservable<BumpedSideChangedMessage> self) { return _bumpedSideChangedObservable; }
    //keeps track of who bumped the puck last
    Side _side;
    public Side side { get { return _side; } }
    Transform _player;
    public Transform player { get { return _player; } }
	// Use this for initialization
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag(Tags.player))
        {
            setLastBumped(other.transform);
        }
    }

    public void setLastBumped(Transform player)
    {
        if (player != _player)
        {
            _player = player;
            _side = player.GetComponent<Stats>().side;
            _bumpedSideChangedObservable.Post(new BumpedSideChangedMessage(_side, _player));
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