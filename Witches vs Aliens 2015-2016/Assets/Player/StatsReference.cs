using UnityEngine;
using System.Collections;

public class StatsReference : Stats {
    public virtual Stats referencedStat { get; set; }
    public override Side side { get { return referencedStat.side; } set { referencedStat.side = value; } }
    public override int playerID { get { return referencedStat.playerID; } set { referencedStat.playerID = value; } }
    public override NetworkMode networkMode { get { return referencedStat.networkMode; } set { referencedStat.networkMode = value; } }
}