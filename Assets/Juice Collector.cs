using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuiceCollector : MonoBehaviour
{
    ParticleSystem ps;

    List<ParticleSystem.Particle> particles = new List<ParticleSystem.Particle>();


    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }


    // Update is called once per frame
    private void OnParticleTrigger()
    {
        
        int triggeredParticles = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, particles);

        //Debug.Log(triggeredParticles);

        for (int i = 0; i < triggeredParticles; i++)
        {
            ParticleSystem.Particle particle = particles[i];
            particle.remainingLifetime = 0;
            GetComponentInParent<MainPlayerMovement>().juiceAmount += GetComponentInParent<MainPlayerMovement>().decreasePerJump/6*0.8f;
            particles[i] = particle;
        }

        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, particles);
    }
}
