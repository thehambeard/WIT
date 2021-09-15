﻿using DG.Tweening;
using Kingmaker.UI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Kingmaker.UI.ServiceWindow;
using static WIT.Main;
using Kingmaker.Assets.UI.Common;
using HarmonyLib;
using System.Reflection;
using Kingmaker.UI.Tooltip;

namespace WIT.UI.QuickInventory
{
	public class DraggableWindow : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
	{
		private bool _moveMode;
		private Vector2 _mouseStartPos;
		private Vector2 _containerStartPos;
		private Vector2 _lastMausePos;
		private Vector2 _takeDrag;
		private TooltipTrigger _tooltip;
		private RectTransform _ownRectTransform;
		private RectTransform _parentRectTransform;

		void Start()
		{
			_takeDrag = new Vector2(0f, 0f);
			_ownRectTransform = (RectTransform) transform.parent.parent;
			_parentRectTransform = (RectTransform)_ownRectTransform.parent;
			_tooltip = gameObject.GetComponent<TooltipTrigger>();
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}
			_moveMode = true;
			_mouseStartPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			_ownRectTransform.anchoredPosition = _ownRectTransform.anchoredPosition + _takeDrag;
			_ownRectTransform.DOAnchorPos(_ownRectTransform.anchoredPosition + _takeDrag, 0.1f, false).SetUpdate(true);
			_containerStartPos = _ownRectTransform.anchoredPosition;
			_tooltip.enabled = false;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			_ownRectTransform.DOAnchorPos(_ownRectTransform.anchoredPosition - _takeDrag, 0.1f, false).SetUpdate(true);
			_moveMode = false;
			_mouseStartPos = default(Vector2);
			_tooltip.enabled = true;
		}

		public void LateUpdate()
		{
			if (!_moveMode)
			{
				return;
			}
			Vector2 vector = new Vector2(Input.mousePosition.x - _mouseStartPos.x, Input.mousePosition.y - _mouseStartPos.y);
			if (_lastMausePos == vector)
			{
				return;
			}
			Vector2 vector2 = _containerStartPos + vector - _takeDrag;
			vector2 = UIUtility.LimitPositionRectInRect(vector2, _parentRectTransform, _ownRectTransform);
			_ownRectTransform.anchoredPosition = vector2 + _takeDrag;
			_lastMausePos = vector;
		}
	}
}