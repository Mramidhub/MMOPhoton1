using Assets.Scripts.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanelZero : MonoBehaviour
{
    Vector3 defaultPosition;

	void Start ()
    {
        defaultPosition = UIManager.Instance.UIMainMenuPanel.transform.position;
        transform.position = defaultPosition;
        gameObject.SetActive(false);
	}
}
