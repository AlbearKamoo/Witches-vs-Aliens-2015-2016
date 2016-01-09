using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class NetworkedTransform : MonoBehaviour, IObserver<OutgoingNetworkStreamReaderMessage> {
    NetworkNode node;
    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
	void Start () {
        node = GameObjectExtension.GetComponentWithTag<NetworkNode>(Tags.gameController);
        if (node is Server)
        {
            node.Subscribe<OutgoingNetworkStreamReaderMessage>(this);
        }
        else if (node is Client)
        {

        }
	}

    public void Notify(OutgoingNetworkStreamReaderMessage m)
    {
        //m.writer //oh....how will we communicate GUIDs?
            //probably integrate this class into players, so we can use PlayerID.
            //and there's only one puck, so that's easy
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
