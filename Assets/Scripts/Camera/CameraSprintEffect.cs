using UnityEngine;

[System.Serializable]
public class CameraSprintEffect 
{
	[SerializeField] private float speedMultiplier;
	[SerializeField] private ParticleSystem sprintEffect;
	[SerializeField] private AudioClip sprintClip;
	private AudioSource windSource = null;

	public void SpeedLines(PlayerRef player)
	{
		if (player.PlayerMovement.Magnitude >= 1f)
		{
			if (!sprintEffect.isPlaying)
            {
				sprintEffect.Play();
				windSource = AudioManager.Instance.PlayOnce(sprintClip, player.transform.position);
			}

			float velocityRatio = player.PlayerMovement.Magnitude / 15f;

			float rateOverLifeTime = Mathf.Max(Vector3.Angle(player.PlayerMovement.Velocity, player.PlayerCam.transform.forward) * 0.15f, 1f);
			rateOverLifeTime = player.PlayerMovement.Magnitude * velocityRatio / rateOverLifeTime;

			ParticleSystem.EmissionModule em = sprintEffect.emission;
			em.rateOverTime = rateOverLifeTime * velocityRatio * 2f;

			ParticleSystem.VelocityOverLifetimeModule velOverLife = sprintEffect.velocityOverLifetime;
			velOverLife.speedModifier = velocityRatio * speedMultiplier;

			if (windSource != null) windSource.volume = AudioManager.Instance.SoundDictionary[sprintClip].Volume * velocityRatio;
		}
		else if (sprintEffect.isPlaying)
        {
			sprintEffect.Stop();
			AudioManager.Instance.StopSound(sprintClip, windSource);
			windSource = null;
		}
	}
}
