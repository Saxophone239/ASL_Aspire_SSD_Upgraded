using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class MapButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField] private GameObject tooltip;
	[SerializeField] private TextMeshProUGUI tooltipText;
	[SerializeField] private Image imageIcon;
	[SerializeField] private Image lockIcon;
	[SerializeField] private Button button;
	[SerializeField] private GameObject spinner;
	[SerializeField] private Canvas canvas;
	[SerializeField] protected int packetIDDisplayed;
	[SerializeField] protected int reviewNumber;
	[SerializeField] protected bool isReviewDay;

	[SerializeField] protected bool _isLocked;
	public bool IsLocked
	{
		get {return _isLocked;}
		set
		{
			// if (value == _isLocked) return;

			_isLocked = value;
			ToggleLock(_isLocked);
		}
	}

	public abstract void OnButtonClick();

	public void SetTooltipText(string text, bool setInactiveAfter)
	{
		tooltipText.text = text;
		if (setInactiveAfter) tooltip.SetActive(false);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (IsLocked) return;
		tooltip.SetActive(true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		tooltip.SetActive(false);
	}

	public void ToggleLock(bool isLocked)
	{
		if (isLocked)
		{
			// Lock icon
			lockIcon.gameObject.SetActive(true);
			button.interactable = false;
		}
		else
		{
			// Unlock icon
			lockIcon.gameObject.SetActive(false);
			button.interactable = true;
		}
	}

	public void TurnOnSpinner(bool shouldTurnOn)
	{
		if (shouldTurnOn) spinner.SetActive(true);
		else spinner.SetActive(false);
	}

	public void SetCamera()
	{
		canvas.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
	}
}
