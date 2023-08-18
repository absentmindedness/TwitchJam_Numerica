using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatAbout : MonoBehaviour
{
    [SerializeField] Vector3 rotationDegrees = new Vector3();
	[SerializeField] Vector3 rotationSpeeds = new Vector3();

	[SerializeField] Vector3 transformDegrees = new Vector3();
	[SerializeField] Vector3 transformSpeeds = new Vector3();

	[SerializeField] float swellSpeed = 0f;

	private Vector3 identityRotationEulers, tempRotation;
	private Vector3 identityTransform, tempTransform;

	private float waveSeed = 0f;
	private float swellValue = 1f;

    private Vector3 seedVarRot = new Vector3();
	private Vector3 seedVarTrans = new Vector3();

	void Start()
    {
        identityRotationEulers = transform.localEulerAngles;
		identityTransform = transform.localPosition;

        seedVarRot.x = Random.Range(.9f, 1.1f);
		seedVarRot.y = Random.Range(.9f, 1.1f);
		seedVarRot.z = Random.Range(.9f, 1.1f);

		seedVarTrans.x = Random.Range(.9f, 1.1f);
		seedVarTrans.y = Random.Range(.9f, 1.1f);
		seedVarTrans.z = Random.Range(.9f, 1.1f);
	}

    void Update()
    {
        waveSeed += Time.deltaTime;

		if (swellSpeed != 0)
		{
			swellValue = 0.5f + Mathf.Sin(waveSeed * swellSpeed) * 0.5f;
		}

		if (rotationDegrees != Vector3.zero)
		{
			tempRotation.x = identityRotationEulers.x + Mathf.Sin((waveSeed * seedVarRot.x) * rotationSpeeds.x) * (rotationDegrees.x) * swellValue;
			tempRotation.y = identityRotationEulers.y + Mathf.Sin((waveSeed * seedVarRot.y) * rotationSpeeds.y) * (rotationDegrees.y) * swellValue;
			tempRotation.z = identityRotationEulers.z + Mathf.Sin((waveSeed * seedVarRot.z) * rotationSpeeds.z) * (rotationDegrees.z) * swellValue;
		}

		tempTransform.x = identityTransform.x + Mathf.Sin((waveSeed * seedVarTrans.x) * transformSpeeds.x) * (transformDegrees.x) * swellValue;
		tempTransform.y = identityTransform.y + Mathf.Sin((waveSeed * seedVarTrans.y) * transformSpeeds.y) * (transformDegrees.y) * swellValue;
		tempTransform.z = identityTransform.z + Mathf.Sin((waveSeed * seedVarTrans.z) * transformSpeeds.z) * (transformDegrees.z) * swellValue;

		if (rotationDegrees != Vector3.zero) transform.localEulerAngles = tempRotation;
		transform.localPosition = tempTransform;
	}
}
