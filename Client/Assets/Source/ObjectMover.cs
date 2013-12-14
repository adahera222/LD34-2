using UnityEngine;
using System.Collections;

public class ObjectMover : MonoBehaviour
{
	public delegate void OnMoveComplete();
	
	// Used to rotate the piece faster.
	public float RotationMultiplier = 1.0f;
	
	private float _moveDelta = 1.0f;
	private Vector3 _originalPosition = Vector3.zero;
	private Quaternion _originalRotation = Quaternion.identity;
	private Vector3 _targetPosition = Vector3.zero;
	private Quaternion _targetRotation = Quaternion.identity;
	private float _peakHeight = 0.0f;
	private OnMoveComplete _onMoveComplete = null;
	
	// Use this for initialization
	void Start ()
	{
		this.enabled = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Handle moving of objects.
		if( _moveDelta < 1.0f )
		{
			float positionDelta = Mathf.SmoothStep( 0.0f, 1.0f, _moveDelta );
			float rotationDelta = Mathf.SmoothStep( 0.0f, 1.0f, Mathf.Clamp01( _moveDelta * RotationMultiplier ) );
			
			transform.position = Vector3.Lerp( _originalPosition, _targetPosition, positionDelta ) + new Vector3( 0.0f, Mathf.Sin ( positionDelta * Mathf.PI ) * _peakHeight, 0.0f );
			transform.rotation = Quaternion.Slerp( _originalRotation, _targetRotation, rotationDelta );
			
			// Increase move delta.
			_moveDelta += Time.deltaTime;
		}
		else
		{
			this.enabled = false;
		}
	}
	
	// Move object to target.
	public void Move( Vector3 targetPosition, Quaternion targetRotation, float peakHeight, OnMoveComplete onMoveComplete )
	{
		this.enabled = true;
		_moveDelta = 0.0f;
		_originalPosition = transform.position;
		_originalRotation = transform.rotation;
		_targetPosition = targetPosition;
		_targetRotation = targetRotation;
		_peakHeight = peakHeight;
		_onMoveComplete = onMoveComplete;				
	}
}
