using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraIdleSway
{
	[Header("Head Sway Settings")]
	[SerializeField] private bool enabled = true;
	[SerializeField] private float swayAmount;
	[SerializeField] private float swayFrequency;

	public Vector3 HeadSwayOffset { get; private set; } = Vector3.zero;
	private float headSwayScroller = 0;

	public void IdleCameraSway(PlayerRef player)
	{
		if (!enabled) return;
		if (!player.PlayerMovement.Grounded || player.PlayerMovement.Magnitude > 5f) return;

		headSwayScroller += Time.deltaTime * swayFrequency;
		HeadSwayOffset = Vector3.Lerp(HeadSwayOffset, LissajousCurve(headSwayScroller) * swayAmount, 5f * Time.deltaTime);
	}

	private Vector3 LissajousCurve(float Time)
	{
		return new Vector3(Mathf.Sin(Time), 1f * Mathf.Sin(2f * Time + Mathf.PI));
	}

	public void Enable(bool enabled) => this.enabled = enabled;
}
