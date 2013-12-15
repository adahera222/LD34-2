using UnityEngine;
using System.Collections;

public class Glower : MonoBehaviour
{
	
	public float GlowTarget = 0.0f;
	
	public MeshRenderer GlowMesh = null;
	private float _glowLerp = 0.0f;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		_glowLerp = ( _glowLerp * 0.9f ) + ( GlowTarget * 0.1f );
		
		if( _glowLerp < 0.01f )
		{
			GlowMesh.enabled = false;
		}
		else
		{
			GlowMesh.enabled = true;
			
			float glow = ( Mathf.Abs ( Mathf.Sin( Time.time * 1.5f ) ) ) * 0.35f * _glowLerp;
			
			GlowMesh.materials[0].SetColor ( "_TintColor", new Color( 0.0f, glow, glow, glow ) );
		}
	}
}
