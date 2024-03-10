extends Control

var timer
var clockAmountProgress
var fill_speed = 1
var MAX_TIMER = 60
var INK_PER_SHOOT = 20
var houseRatingLabel
var houseRatingVBoxContainer
var BAD_RATING_STRINGS = [
	"Las energias son\nun poco turbias\n",
	"Las cosas no se quedan\ndonde las dejas\n",
	"La electricidad falla\nmucho sin motivo\n",
	"Los muebles hacen ruidos\ny se abren solos\n",
	"Decian que habia\n fantasmas pero era\nmentira"
]
var BAD_COLOR_RATING = "7a0d0d"
var journal
var inkProgressBar
var startsButton

var starEmptySprite = preload("res://resources/sprites/UI/Star_Empty2.png")

func _ready():
	$Panel/ClockProgress.set_value(0.0)
	clockAmountProgress = $Panel/ClockProgress.get_value()
	houseRatingLabel = $Panel/HouseRating
	houseRatingLabel.visible = false
	houseRatingVBoxContainer = $Panel/HouseRating/VBoxContainer
	journal = $Panel/Journal
	journal.visible = false
	inkProgressBar = $Panel/PenTexture/InkProgressBar
	startsButton = $Panel/StarsButton
	
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
	if event is InputEventKey and event.scancode == KEY_I and event.pressed:
		var index = 0
		for star in startsButton.get_children():
			print(star.texture)
			star.texture = starEmptySprite
		for rating in houseRatingVBoxContainer.get_children():
			rating.text = BAD_RATING_STRINGS[index]
			index += 1
			rating.add_color_override("font_color", Color(BAD_COLOR_RATING))
			
