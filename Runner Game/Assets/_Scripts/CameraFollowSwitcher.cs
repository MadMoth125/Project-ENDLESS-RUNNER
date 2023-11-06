using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraFollowSwitcher : MonoBehaviour
{
	private CinemachineVirtualCamera _virtualCamera;
	private CinemachineTransposer _virtualCameraTransposer;

	private void Awake()
	{
		_virtualCamera = GetComponent<CinemachineVirtualCamera>();
		_virtualCameraTransposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
	}

	private void OnEnable()
	{
		GameManager.Instance.OnPlayerDie += OnPlayerDie;
	}

	private void OnDisable()
	{
		GameManager.Instance.OnPlayerDie -= OnPlayerDie;
	}

	private void OnPlayerDie()
	{
		// _virtualCameraTransposer.m_BindingMode = CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp;
		_virtualCamera.m_LookAt = null;
	}
}