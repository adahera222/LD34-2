using UnityEngine;
using System.Collections;

public class PlayerScoreBoard : MonoBehaviour
{
	public TextMesh ScoreText;
	public PlayerPiece PlayerPiece;
	public MeshRenderer TopMesh;
	
	// Use this for initialization
	void Start ()
	{
		SetInactive();	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if( PlayerPiece != null )
		{
			string stringScore = string.Format ( "{0}", PlayerPiece.Score );
			if( ScoreText.text != stringScore )
			{
				ScoreText.text = stringScore;
			}
		}
	}
	
	public void SetActive()
	{
		if( PlayerPiece != null )
		{
			TopMesh.materials[0].mainTexture = PlayerPiece.ActiveTexture;
		}
	}

	public void SetInactive()
	{
		if( PlayerPiece != null )
		{
			TopMesh.materials[0].mainTexture = PlayerPiece.InactiveTexture;
		}
	}
}
