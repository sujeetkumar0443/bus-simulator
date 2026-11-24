using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class TireParticleEmitter : MonoBehaviour
{
	public enum Mode
	{
		PressureAndSkid = 0,
		PressureAndVelocity = 1
	}

	public Mode mode;

	public float emissionRate = 10f;

	[Range(0f, 1f)]
	public float emissionShuffle;

	public float maxLifetime = 7f;

	public float minVelocity = 1f;

	public float maxVelocity = 15f;

	[Range(0f, 1f)]
	public float tireVelocityRatio = 0.5f;

	public Color Color1 = Color.white;

	public Color Color2 = Color.gray;

	public bool randomColor;

	private ParticleSystem m_particles;

	private ParticleSystem.Particle m_particle;

	private void OnEnable()
	{
		m_particles = GetComponent<ParticleSystem>();
		m_particles.Stop();
	}
	[System.Obsolete]
	public float EmitParticle(Vector3 position, Vector3 wheelVelocity, Vector3 tireVelocity, float pressureRatio, float intensityRatio, float lastParticleTime)
	{
		if (!base.isActiveAndEnabled)
		{
			return -1f;
		}
		if (lastParticleTime < 0f)
		{
			lastParticleTime = Time.time - 1f / emissionRate;
		}
		int num = (int)((Time.time - lastParticleTime) * emissionRate);
		if (num <= 0)
		{
			return lastParticleTime;
		}
		float num2 = 0f;
		switch (mode)
		{
		case Mode.PressureAndSkid:
			num2 = pressureRatio * intensityRatio * maxLifetime;
			break;
		case Mode.PressureAndVelocity:
		{
			float value = tireVelocity.magnitude + wheelVelocity.magnitude;
			num2 = pressureRatio * maxLifetime * Mathf.InverseLerp(minVelocity, maxVelocity, value);
			break;
		}
		}
		if (num2 <= 0f)
		{
			return -1f;
		}
		for (int i = 0; i < num; i++)
		{
			m_particle.position = position;
			m_particle.velocity = wheelVelocity * 0.9f + tireVelocity * tireVelocityRatio;
			float num3 = num2 * Random.Range(0.6f, 1.4f);
			m_particle.remainingLifetime = num3;
			m_particle.startLifetime = num3;
			m_particle.size = num3 / maxLifetime * Random.Range(0.8f, 1.4f);
			m_particle.rotation = Random.Range(0f, 360f);
			m_particle.angularVelocity = 0f;
			if (randomColor)
			{
				m_particle.color = Color.Lerp(Color1, Color2, Random.value);
			}
			else
			{
				m_particle.color = Color1;
			}
			m_particle.randomSeed = (uint)Random.Range(0, 20000);
			m_particles.Emit(m_particle);
		}
		return Time.time + Random.Range(0f - emissionShuffle, emissionShuffle) / emissionRate;
	}
}
