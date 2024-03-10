extends Control

var timer
var clockAmountProgress
var fill_speed = 1
var MAX_TIMER = 60
var INK_PER_SHOOT = 20
var houseRatingLabel
var journal
var inkProgressBar

func _ready():
	$Panel/ClockProgress.set_value(0.0)
	clockAmountProgress = $Panel/ClockProgress.get_value()
	houseRatingLabel = $Panel/HouseRating
	houseRatingLabel.visible = false
	journal = $Panel/Journal
	journal.visible = false
	inkProgressBar = $Panel/PenTexture/InkProgressBar
	
	$Panel/ClockProgress/Timer.wait_time = 1.0
	$Panel/ClockProgress/Timer.start()

func _on_Timer_timeout():
	clockAmountProgress = clockAmountProgress + fill_speed
	$Panel/ClockProgress.set_value(clockAmountProgress)

	if $Panel/ClockProgress.get_value() >= MAX_TIMER:
		$Panel/ClockProgress/Timer.stop()
		$Panel/ClockProgress/Timer.start()
		clockAmountProgress = 0
		$Panel/ClockProgress.set_value(clockAmountProgress)
		if $Panel/ClockProgress/Label.text == "DAY":
			$Panel/ClockProgress/Label.text = "NIGHT"
		else:
			$Panel/ClockProgress/Label.text = "DAY"

# Click mode for house ratings notes
func _on_StarsButton_pressed():
	houseRatingLabel.visible = !houseRatingLabel.visible

# Keyboard mode for house ratings notes, diary and ink removal
func _input(event):
	if event is InputEventKey and event.scancode == KEY_J and event.pressed:
		houseRatingLabel.visible = !houseRatingLabel.visible
	if event is InputEventKey and event.scancode == KEY_K and event.pressed:
		journal.visible = !journal.visible
	if event is InputEventKey and event.scancode == KEY_L and event.pressed:
		if inkProgressBar.value >=0:
			inkProgressBar.value -= INK_PER_SHOOT
		else:
			inkProgressBar.value = 0
			# PONER UN AVISO DE QUE NO TE QUEDA TINTA
