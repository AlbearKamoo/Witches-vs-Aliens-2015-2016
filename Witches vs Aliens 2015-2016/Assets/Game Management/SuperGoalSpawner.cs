using UnityEngine;
using System.Collections;

//should be placed on the parent of all the supergoal spawn pairs

public class SuperGoalSpawner : MonoBehaviour {
    [SerializeField]
    protected GameObject SuperGoalPrefab;

    [SerializeField]
    protected float spawnTime;
    [SerializeField]
    protected float spawnTimeVariance;
    [SerializeField]
    protected float superGoalDuration;
    SuperGoal SuperGoal1;
    SuperGoal SuperGoal2;
    Transform[] children;
	// Use this for initialization
	void Awake () {
        SuperGoal1 = Instantiate(SuperGoalPrefab).GetComponent<SuperGoal>();
        SuperGoal2 = Instantiate(SuperGoalPrefab).GetComponent<SuperGoal>();
        SuperGoal1.mirror = SuperGoal2;
        SuperGoal2.mirror = SuperGoal1;

        children = new Transform[transform.childCount];
        int i = 0;
        foreach (Transform child in transform)
        {
            children[i++] = child;
        }

        StartCoroutine(GoalSpawning());
	}

    void spawnSuperGoal()
    {
        //randomly chose a spawn point
        int childNum = Random.Range(0, transform.childCount);

        SuperGoal1.transform.SetParent(children[childNum], false);
        SuperGoal2.transform.SetParent(children[childNum].Find("Mirror"), false);

        SuperGoal1.active = true;
        SuperGoal2.active = true;
    }

    IEnumerator GoalSpawning()
    {
        while (true)
        {
            yield return new WaitForSeconds(RandomLib.RandFloatRange(spawnTime, spawnTimeVariance));
            spawnSuperGoal();
            Callback.FireAndForget(() => { SuperGoal1.active = false; SuperGoal2.active = false; }, superGoalDuration, this);
        }
    }
}
