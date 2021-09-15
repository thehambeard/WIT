using DG.Tweening;
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
	public class ScalableWindow : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
	{
		private bool _moveMode;
		private Vector2 _mouseStartPos;
		private Vector2 _lastMausePos;
		private TooltipTrigger _tooltip;
		private RectTransform _ownRectTransform;
		private Vector2 _currentScale;

		void Start()
		{
			_ownRectTransform = (RectTransform) transform.parent.parent;
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
			_ownRectTransform.DOScale(_ownRectTransform.localScale, 0.1f).SetUpdate(true);
			_currentScale = _ownRectTransform.localScale;
			_tooltip.enabled = false;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			_ownRectTransform.DOScale(_ownRectTransform.localScale, 0.1f).SetUpdate(true);
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
			var x = vector.x;
			
			if (x < 100f && x > 0f)
			{
				_ownRectTransform.localScale = MapValue(0f, 100f, _currentScale.x, 1.2f, x);
			}
			if (x > -100f && x < 0f)
			{
				_ownRectTransform.localScale = MapValue(0f, -100f, _currentScale.x, .5f, x);
			}
			_lastMausePos = vector;
        }

		private Vector3 MapValue(float a0, float a1, float b0, float b1, float a)
		{
			float v =  b0 + (b1 - b0) * ((a - a0) / (a1 - a0));
			return new Vector3(v, v, v);
		}
	}
}