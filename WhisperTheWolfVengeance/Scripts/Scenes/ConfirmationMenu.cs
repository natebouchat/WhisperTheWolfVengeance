using Godot;
using System;

public partial class ConfirmationMenu : ColorRect
{
	private Label title;
	private Button confirm;
	private Button deny;
	private Action onConfirm;
	private Action onDeny;
	
	public override void _Ready()
	{
		title = GetNode<Label>("ColorRect/VBoxContainer/Title");
		confirm = GetNode<Button>("ColorRect/VBoxContainer/HBoxContainer/Confirm");
		deny = GetNode<Button>("ColorRect/VBoxContainer/HBoxContainer/Deny");
	}

	public void SetMenuElements(string aTitle, string aConfirm, string aDeny, Action confirmFunction, Action denyFunction) {
		title.Text = aTitle;
		confirm.Text = aConfirm;
		deny.Text = aDeny;
		onConfirm = confirmFunction;
		onDeny = denyFunction;
		confirm.GrabFocus();
	}

	private void OnConfirmPressed() {
		onConfirm();
		this.QueueFree();
	}

	private void OnDenyPressed() {
		onDeny();
		this.QueueFree();
	}

}
