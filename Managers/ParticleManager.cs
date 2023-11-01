using UnityEngine;

public enum Particle
{
    None,
BombExplosion
}
public class ParticleManager : MonoBehaviourSingletonPersistent<ParticleManager>
{
    [SerializeField] ParticleItem[] particles;

    public void PlayParticle(Particle particle, Vector3 pos)
    {
        if (particle == Particle.None) return;

        for (int i = 0; i < particles.Length; i++)
        {
            if (particles[i].type == particle)
            {
                var temp = Instantiate(particles[i].particleSystem);
                if (temp)
                {
                    temp.gameObject.transform.position = pos;
                    temp.Play();
                }
            }
        }
    }
    public void PlayParticle(Particle particle, Vector3 pos, Vector3 rotation)
    {
        if (particle == Particle.None) return;

        for (int i = 0; i < particles.Length; i++)
        {
            if (particles[i].type == particle)
            {
                var temp = Instantiate(particles[i].particleSystem);
                if (temp)
                {
                    temp.gameObject.transform.position = pos;
                    temp.gameObject.transform.rotation = rotation.GetQuaternion();
                    temp.Play();
                }
            }
        }
    }
    public void PlayParticle(Particle particle, Vector3 pos, Quaternion rotation)
    {
        PlayParticle(particle, pos, rotation.eulerAngles);
    }
    public void PlayParticle(Particle particle, Transform parent)
    {
        if (particle == Particle.None) return;

        for (int i = 0; i < particles.Length; i++)
        {
            if (particles[i].type == particle)
            {
                var temp = Instantiate(particles[i].particleSystem);
                if (temp)
                {
                    temp.transform.SetParent(parent);
                    temp.transform.localPosition = Vector3.zero;
                    temp.Play();
                }
            }
        }
    }
    [System.Serializable]
    public struct ParticleItem
    {
        public Particle type;
        public ParticleSystem particleSystem;
    }
}
