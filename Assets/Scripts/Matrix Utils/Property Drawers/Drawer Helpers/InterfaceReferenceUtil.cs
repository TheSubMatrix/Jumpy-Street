using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public static class InterfaceReferenceUtil {
	public static Label CreateInterfaceLabelOverlay(string interfaceName, ObjectField objectField) {
		Label label = new()
		{
			pickingMode = PickingMode.Ignore,
			style = {
				position = Position.Absolute,
				right = 20,
				top = 1,
				bottom = 1,
				unityTextAlign = TextAnchor.MiddleRight,
				paddingRight = 2,
				fontSize = 11,
				color = new Color(0.7f, 0.7f, 0.7f, 1f)
			}
		};
		UpdateLabelText(false);
		objectField.RegisterCallback<MouseEnterEvent>(_ => UpdateLabelText(true));
		objectField.RegisterCallback<MouseLeaveEvent>(_ => UpdateLabelText(false));
		objectField.RegisterValueChangedCallback(_ => UpdateLabelText(false));

		return label;

		void UpdateLabelText(bool isHovering) {
			label.text = (objectField.value == null || isHovering) ? $"({interfaceName})" : "*";
		}
	}
}