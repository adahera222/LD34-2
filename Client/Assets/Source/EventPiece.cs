using UnityEngine;
using System.Collections;

public class EventPiece : MonoBehaviour
{
	public int EventId = 0;
	
	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
	
	public void SetTexture( Texture2D texture, int eventId )
	{
		EventId = eventId;
		var meshRenderer = GetComponentInChildren< MeshRenderer >();
		var newMaterial = meshRenderer.materials[0]; // thing bleh.
		meshRenderer.materials[0] = newMaterial;
		newMaterial.mainTexture = texture;
	}
}
