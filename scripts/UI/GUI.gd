extends Control

signal start_day
signal start_night

var timer
var clockAmountProgress
var fill_speed = 1
var MAX_TIMER = 60
var INK_PER_SHOOT = 20
var houseRatingLabel
var houseRatingVBoxContainer
var tenantTexture
var BAD_RATING_STRINGS = [
	"Las energías son\nun poco turbias\n",
	"Las cosas no se quedan\ndonde las dejas\n",
	"La electricidad falla\nmucho sin motivo\n",
	"Los muebles hacen ruidos\ny se abren solos\n",
	"Decían que habia\n fantasmas pero era\nmentira"
]
var BAD_COLOR_RATING = "a71212"
var journal
var inkProgressBar
var startsButton

export var SceneOrder : int


var starEmptySprite = preload("res://resources/sprites/UI/Star_Empty2.png")

var poshUIportrait = preload("res://resources/sprites/UI/tenants_UI/cube_posh.png")
var cube_artistUIportrait = preload("res://resources/sprites/UI/tenants_UI/cube_artist.png")
var cube_bluecollarUIportrait = preload("res://resources/sprites/UI/tenants_UI/cube_bluecollar.png")
var investorUIportrait = preload("res://resources/sprites/UI/tenants_UI/cube_investor.png")
var cube_ghosthunterUIportrait = preload("res://resources/sprites/UI/tenants_UI/cube_ghosthunter.png")

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
	$Panel/DayNightFilter.visible = false
	tenantTexture = $Panel/TenantCubeUI/TenantTexture

	# TODO: Refactor tthis troll code :_(
	match SceneOrder:
		1:
			tenantTexture.texture = poshUIportrait
		2:
			tenantTexture.texture = cube_artistUIportrait
			startsButton.get_child(4).texture = starEmptySprite
			for i in range(0, 1):
				var rating = houseRatingVBoxContainer.get_child(i)
				rating.text = BAD_RATING_STRINGS[i]
				rating.add_color_override("font_color", Color(BAD_COLOR_RATING))
		3:
			tenantTexture.texture = cube_bluecollarUIportrait
			startsButton.get_child(4).texture = starEmptySprite
			startsButton.get_child(3).texture = starEmptySprite
			for i in range(0, 2):
				var rating = houseRatingVBoxContainer.get_child(i)
				rating.text = BAD_RATING_STRINGS[i]
				rating.add_color_override("font_color", Color(BAD_COLOR_RATING))
		4:
			tenantTexture.texture = investorUIportrait
			startsButton.get_child(4).texture = starEmptySprite
			startsButton.get_child(3).texture = starEmptySprite
			startsButton.get_child(2).texture = starEmptySprite
			for i in range(0, 3):
				var rating = houseRatingVBoxContainer.get_child(i)
				rating.text = BAD_RATING_STRINGS[i]
				rating.add_color_override("font_color", Color(BAD_COLOR_RATING))
		5:
			tenantTexture.texture = cube_ghosthunterUIportrait
			startsButton.get_child(4).texture = starEmptySprite
			startsButton.get_child(3).texture = starEmptySprite
			startsButton.get_child(2).texture = starEmptySprite
			startsButton.get_child(1).texture = starEmptySprite
			for i in range(0, 4):
				var rating = houseRatingVBoxContainer.get_child(i)
				rating.text = BAD_RATING_STRINGS[i]
				rating.add_color_override("font_color", Color(BAD_COLOR_RATING))
	
	GameManager.connect("InkSpent", self, "_on_ink_spent");
	GameManager.connect("TenantDamaged", self, "_on_tenant_damaged");
	GameManager.connect("NewJournalAction", self, "_on_new_journal_action")
	GameManager.connect("UpdateRatings", self, "_on_new_ratings")
	
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
			$Panel/DayNightFilter.visible = true
			emit_signal("start_night")
		else:
			$Panel/ClockProgress/Label.text = "DAY"
			$Panel/DayNightFilter.visible = false
			emit_signal("start_day")

# Click mode for house ratings notes
func _on_StarsButton_pressed():
	houseRatingLabel.visible = !houseRatingLabel.visible

# Keyboard mode for house ratings notes, diary and ink removal
func _input(event):
	if event is InputEventKey and event.scancode == KEY_R and event.pressed:
		houseRatingLabel.visible = !houseRatingLabel.visible
	if event is InputEventKey and event.scancode == KEY_F and event.pressed:
		journal.visible = !journal.visible

func _on_ink_spent(amountPct):
	inkProgressBar.value -= amountPct
	print("spent some ink")
	
func _on_tenant_damaged(amountPct):
	print("REDUCE LIFE BAR BY" + str(amountPct))
	find_node("TenantLifeBar").value -= amountPct
	$Panel/TenantCubeUI/CubeAnimation.play("goDmg")

func _on_new_journal_action(sentence):
	journal.get_node("ActionsText").text += "\n - " + sentence;
	
	# Small rotation wiggle -- this helps the player notice that a new action has been added
	# to the log even when they don't have it open.
	yield(get_tree().create_timer(0.2), "timeout")
	$Panel/JournalText/JournalAnimation.play("Wiggle")

func _on_new_ratings(num_bad_ratings):
	for i in range(0, num_bad_ratings):
		var rating = houseRatingVBoxContainer.get_child(i)
		rating.text = BAD_RATING_STRINGS[i]
		rating.add_color_override("font_color", Color(BAD_COLOR_RATING))
