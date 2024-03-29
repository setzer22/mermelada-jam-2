tool
extends TextureButton

signal onClick

export(String) var text = "Text button"
export(int) var arrow_margin_from_center = 100

func _ready():
	setup_text()
	hide_arrows()
	set_focus_mode(true)
	connect("pressed", self, "on_pressed");
	connect("button_down", self, "_on_button_down");
	connect("button_up", self, "_on_button_up");

func _process(delta):
	if Engine.editor_hint:
		setup_text()
		show_arrows()

func setup_text():
	$RichTextLabel.bbcode_text = "[center] %s [/center]" % [text]
	
func show_arrows():
	for arrow in [$LeftArrow, $RightArrow]:
		arrow.visible = true
		arrow.global_position.y = rect_global_position.y + (rect_size.y / 3.0)

	var center_x = rect_global_position.x + (rect_size.x / 2)
	$LeftArrow.global_position.x = center_x - arrow_margin_from_center
	$RightArrow.global_position.x = center_x + arrow_margin_from_center

func hide_arrows():
	for arrow in [$LeftArrow, $RightArrow]:
		arrow.visible = false

func _on_button_down():
	$RichTextLabel.add_color_override("default_color",Color.gray)
	
func _on_button_up():
	$RichTextLabel.add_color_override("default_color",Color.white)
	
func on_pressed():
	get_node("/root/SoundManager").PlayOneShot("Select")
	emit_signal("onClick")

func _on_TextureButton_focus_entered():
	show_arrows()
	get_node("/root/SoundManager").PlayOneShot("Hover")

func _on_TextureButton_focus_exited():
	hide_arrows()

func _on_TextureButton_mouse_entered():
	grab_focus()
