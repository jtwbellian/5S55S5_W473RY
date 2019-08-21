using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour
{
    public enum FX
    {
        Splash, Spray, Ripple, Confetti2, Confetti1, LogSplit, Mist, TreeSplit, Dust
    }

    static FXManager instance = null;
    public ParticleSystem[] part_systems;

    public int index = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (FXManager.GetInstance() == null)
        {
            instance = this;
        }
        else if (instance != this)
            Destroy(this);

        if (part_systems == null)
        {
            part_systems = GetComponentsInChildren<ParticleSystem>();
        }

        foreach (ParticleSystem ps in part_systems)
        {
            var main = ps.main;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
        }
    }

    public static FXManager GetInstance()
    {
        return instance;
    }

    // burst from position (directionless)
    public void Burst(FX type, Vector3 pos, int amt)
    {
        var emitter = new ParticleSystem.EmitParams(); 
        emitter.position = pos;
        emitter.applyShapeToPosition = true;
        part_systems[(int)type].Emit(emitter, amt);
    }

    // burst from position (directionless)
    public void SynchronizedBurst(int type, Vector3 pos, int amt)
    {
        var emitter = new ParticleSystem.EmitParams(); 
        emitter.position = pos;
        emitter.applyShapeToPosition = true;
        part_systems[type].Emit(emitter, amt);
    }

    // burst from position (directionless)
    public void Burst(FX type, Vector3 pos, Vector3 rotation, int amt)
    {
        var emitter = new ParticleSystem.EmitParams();

        emitter.position = pos;
    
        var shape = part_systems[(int)type].shape;
        shape.rotation = rotation;

        emitter.applyShapeToPosition = true;

        part_systems[(int)type].Emit(emitter, amt);
    }
}
