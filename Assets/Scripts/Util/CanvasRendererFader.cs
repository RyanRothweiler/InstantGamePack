using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasRendererFader : MonoBehaviour
{

	public bool hideOnAwake;
	[HideInInspector] public bool isFading = false;

	private float currAlpha;
	private CanvasRenderer[] rends;

	private float timeFadeEnd;
	private float timeFadeStart;
	private float timeFadeLength;
	private float currTarget;
	private float startAlpha;
	private float endAlpha;

	void Awake()
	{
		rends = this.GetComponentsInChildren<CanvasRenderer>();
		if (hideOnAwake) {
			UpdateAlpha(0.0f);
		} else {
			UpdateAlpha(1.0f);
		}
	}

	void Update()
	{
		if (isFading) {
			if (Time.unscaledTime >= timeFadeEnd) {
				isFading = false;
				UpdateAlpha(currTarget);
			} else {
				float newAlpha = Mathf.Lerp(startAlpha, endAlpha, (Time.unscaledTime - timeFadeStart) / timeFadeLength);
				UpdateAlpha(newAlpha);
			}
		}
	}

	public void Fade(float target, float length)
	{
		if (target > 0) {
			this.gameObject.SetActive(true);
		}

		if (length == 0.0f) {
			UpdateAlpha(target);
		} else {
			isFading = true;
			startAlpha = currAlpha;
			endAlpha = target;
			currTarget = target;
			timeFadeEnd = Time.unscaledTime + length;
			timeFadeStart = Time.unscaledTime;
			timeFadeLength = length;
		}
	}

	// Only for convenience
	public void FadeIn(float length) { Fade(1.0f, length); }
	public void FadeOut(float length) { Fade(0.0f, length); }

	private void UpdateAlpha(float newAlpha)
	{
		if (newAlpha == 0.0f && endAlpha == 0.0f) {
			this.gameObject.SetActive(false);
		}

		currAlpha = newAlpha;
		for (int index = 0; index < rends.Length; index++) {
			rends[index].SetAlpha(currAlpha);
		}
	}
}