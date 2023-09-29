using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraLook
{
	[Header("Sensitivity")]
	[SerializeField] private Vector2 sensitivity;
	[SerializeField] private Vector2 rotateSmoothTime;

	[Header("Clamp Rotation")]
	[SerializeField] private float upClampAngle;
	[SerializeField] private float downClampAngle;

	private Vector2 rotation;
	public Vector2 SmoothRotation { get; private set; }
	public Vector2 RotationDelta { get; private set; }

    public void LookUpdate(Vector2 input)
    {
		//Calculate Rotation
		{
			RotationDelta = 0.02f * sensitivity * input;
			rotation.y += RotationDelta.y;
			rotation.x -= RotationDelta.x;

			rotation.x = Mathf.Clamp(rotation.x, -upClampAngle, downClampAngle);
		}

		//Smooth Rotation
        {
			SmoothRotation = new Vector2(
				Mathf.Lerp(SmoothRotation.x, rotation.x, rotateSmoothTime.x * Time.deltaTime),
				Mathf.Lerp(SmoothRotation.y, rotation.y, rotateSmoothTime.y * Time.deltaTime));
		}
	}
}
