using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WanderPoint : AbstractWanderNode {

    [SerializeField]
    protected float speed;

    [SerializeField]
    protected float accel;

    List<Wanderer> wanderers = new List<Wanderer>();
    List<float> wandererDistances = new List<float>();

    public override Vector2 targetPosition()
    {
        return this.transform.position;
    }

    public override void direct(Wanderer newWanderer)
    {
        do
        {
            newWanderer.destination = randomNeighbor();
        } while (newWanderer.source == newWanderer.destination);

        newWanderer.targetPosition = newWanderer.destination.targetPosition();
        newWanderer.source = this;

        wanderers.Add(newWanderer);
        Vector2 direction = newWanderer.targetPosition - newWanderer.rigid.position;
        wandererDistances.Add(direction.magnitude);
    }

	// Update is called once per frame
	void FixedUpdate () {
        for (int i = 0; i < wanderers.Count; i++)
        {
            Vector2 direction = wanderers[i].targetPosition - wanderers[i].rigid.position;
            float distance = direction.magnitude;
            if (distance > wandererDistances[i] && distance < 1)
            {
                wanderers[i].destination.direct(wanderers[i]);
                wanderers.RemoveAt(i);
                wandererDistances.RemoveAt(i);
                i--;
            }
            else
            {
                direction.Normalize();
                wanderers[i].rigid.velocity = Vector2.ClampMagnitude(Vector2.MoveTowards(wanderers[i].rigid.velocity, speed * direction, speed * accel * Time.fixedDeltaTime), speed);
                wandererDistances[i] = distance;
            }
        }
	}
}
