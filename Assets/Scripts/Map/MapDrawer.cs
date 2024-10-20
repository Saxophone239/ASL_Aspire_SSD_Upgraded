using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDrawer : MonoBehaviour
{
    [SerializeField] private LineRenderer lr;
	[SerializeField] private GameObject[] icons;

	private void Start()
	{
		if (lr == null) lr = GetComponent<LineRenderer>();
		lr.positionCount = icons.Length;
	}

	public void DrawIconLockedStatus()
	{
		if (icons.Length != GlobalManager.Instance.MapIconIsLockedStatus.Count)
		{
			Debug.LogError($"Number of loaded icons ({GlobalManager.Instance.MapIconIsLockedStatus.Count}) and scene icons ({icons.Length}) do not equal each other!");
		}

		for (int i = 0; i < icons.Length; i++)
		{
			if (icons[i].TryGetComponent<MapButton>(out MapButton button))
			{
				button.IsLocked = GlobalManager.Instance.MapIconIsLockedStatus[i];
			}
		}
	}

	private void Update()
	{
		for (int i = 0; i < icons.Length; i++)
		{
			lr.SetPosition(i, icons[i].transform.position);
		}
	}

	
}
