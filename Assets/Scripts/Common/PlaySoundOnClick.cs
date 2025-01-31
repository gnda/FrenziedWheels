﻿using UnityEngine;
using UnityEngine.UI;

public class PlaySoundOnClick : MonoBehaviour {


	[SerializeField] string m_SoundName;

	// Use this for initialization
	void Start() {
		Button button = GetComponent<Button>();
		if (button) button.onClick.AddListener(PlaySound);
	}
	
	void PlaySound()
	{
		if (SfxManager.Instance) SfxManager.Instance.PlaySfx(m_SoundName);
	}
}
