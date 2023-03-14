﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrigger : MonoBehaviour {
	public AudioSource source;
	public AudioClip clip;

	public void Awake() {
		source = GetComponent<AudioSource> ();
	}

	public void OnTriggerEnter(Collider other) {
		source.Play ();
	}
}
