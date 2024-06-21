extends Node2D

var max_generation_time = 7


# 1.0 = one second. time gets reset every time_step, then all agents get updated
var time = 0
# total_time gets reset every time a new generation is started
var total_time = 0
# every time_step the cars network takes sensory information and decides how to act
var time_step = 0.2
# every generation_step a new generation is made. this gets increased over time.
var generation_step = 15


var savePoints = [0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180,
 190, 200, 210, 220, 230, 240, 250, 260, 270, 280, 290, 300, 310, 320, 330, 340, 350, 360, 370, 380,
 390, 400, 410, 420, 430, 440, 450, 460, 470, 480, 490, 500, 510, 520, 530, 540, 550, 560, 570, 580,
 590, 600, 610, 620, 630, 640, 650, 660, 670, 680, 690, 700, 710, 720, 730, 740, 750, 760, 770, 780,
 790, 800, 810, 820, 830, 840, 850, 860, 870, 880, 890, 900, 910, 920, 930, 940, 950, 960, 970, 980,
 990, 1000, 1010, 1020, 1030, 1040, 1050, 1060, 1070, 1080, 1090, 1100, 1110, 1120, 1130, 1140, 1150,
 1160, 1170, 1180, 1190, 1200, 1210, 1220, 1230, 1240, 1250, 1260, 1270, 1280, 1290, 1300, 1310, 1320,
 1330, 1340, 1350, 1360, 1370, 1380, 1390, 1400, 1410, 1420, 1430, 1440, 1450, 1460, 1470, 1480, 1490,
 1500, 1510, 1520, 1530, 1540, 1550, 1560, 1570, 1580, 1590, 1600, 1610, 1620, 1630, 1640, 1650, 1660,
 1670, 1680, 1690, 1700, 1710, 1720, 1730, 1740, 1750, 1760, 1770, 1780, 1790, 1800, 1810, 1820, 1830,
 1840, 1850, 1860, 1870, 1880, 1890, 1900, 1910, 1920, 1930, 1940, 1950, 1960, 1970, 1980, 1990, 2000,
 2010, 2020, 2030, 2040, 2050, 2060, 2070, 2080, 2090, 2100, 2110, 2120, 2130, 2140, 2150, 2160, 2170,
 2180, 2190, 2200, 2210, 2220, 2230, 2240, 2250, 2260, 2270, 2280, 2290, 2300, 2310, 2320, 2330, 2340,
 2350, 2360, 2370, 2380, 2390, 2400, 2410, 2420, 2430, 2440, 2450, 2460, 2470, 2480, 2490, 2500, 2510,
 2520, 2530, 2540, 2550, 2560, 2570, 2580, 2590, 2600, 2610, 2620, 2630, 2640, 2650, 2660, 2670, 2680,
 2690, 2700, 2710, 2720, 2730, 2740, 2750, 2760, 2770, 2780, 2790, 2800, 2810, 2820, 2830, 2840, 2850,
 2860, 2870, 2880, 2890, 2900, 2910, 2920, 2930, 2940, 2950, 2960, 2970, 2980, 2990, 3000,3010, 3020,
 3030, 3040, 3050, 3060, 3070, 3080, 3090, 3100, 3110, 3120, 3130, 3140, 3150, 3160, 3170, 3180, 3190,
 3200, 3210, 3220, 3230, 3240, 3250, 3260, 3270, 3280, 3290, 3300, 3310, 3320, 3330, 3340, 3350, 3360,
 3370, 3380, 3390, 3400, 3410, 3420, 3430, 3440, 3450, 3460, 3470, 3480, 3490,3500, 3510, 3520, 3530, 3540,
 3550, 3560, 3570, 3580, 3590, 3600, 3610, 3620, 3630, 3640, 3650, 3660, 3670, 3680, 3690, 3700, 3710, 3720,
 3730, 3740, 3750, 3760, 3770, 3780, 3790, 3800, 3810, 3820, 3830, 3840, 3850, 3860, 3870, 3880, 3890, 3900,
 3910, 3920, 3930, 3940, 3950, 3960, 3970, 3980, 3990, 4000, 5000]

export var saveToLoad = ""


var agent_body_path = "res://Characters/AI.tscn"
var ga = GeneticAlgorithm.new(17, 4, agent_body_path, true)

var container


var fitnessCSV : File


func _ready() -> void:
	
	container = get_node("TrainScene/PlayerContainer")
	definePOI()
	
	add_child(ga)
	place_bodies(ga.get_curr_bodies())
	
	var date = OS.get_datetime()
	var dir = Directory.new()
	dir.make_dir_recursive("user://network_configs/"+str(date['year']) + '-' + str(date['month']) + '-' + str(date['day']) + "/")
	
	fitnessCSV = File.new()
	fitnessCSV.open("user://network_configs/"+str(date['year']) + '-' + str(date['month']) + '-' + str(date['day']) + "/fitness.csv", File.WRITE)
	
func _exit_tree():
	if fitnessCSV:
		fitnessCSV.close()

func _physics_process(delta):
	# update time since last update
	time += delta; total_time += delta
	
	# if enough time has passed for the next time_step, update all agents
	if time > time_step:
		time = 0
		ga.next_timestep()
	
	# if all dead or too much time has passed, start a new gen
	if ga.all_agents_dead or total_time > max_generation_time:
		restart()
		
func definePOI():
	
	var GrpObj = get_tree().get_nodes_in_group("FirstObjective")
	var Obj
	#remet à plat les objectifs
	for i in range(GrpObj.size()):
		Obj = GrpObj[i]
		Obj.Id = 80
		Obj.NextPOI = 81
		
	# assigne le premier objectif
	var currobj = randi() % GrpObj.size()
	Obj = GrpObj[currobj]
	Obj.Id = 10
	Obj.NextPOI = 11
	
	if currobj+1 < GrpObj.size():
		Obj = GrpObj[currobj+1]
		Obj.Id = 11
		Obj.NextPOI = 12
	else :
		Obj = GrpObj[currobj-1]
		Obj.Id = 11
		Obj.NextPOI = 12
	
	
	GrpObj = get_tree().get_nodes_in_group("SecObjective")
	#remet à plat les objectifs
	for i in range(GrpObj.size()):
		Obj = GrpObj[i]
		Obj.Id = 80
		Obj.NextPOI = 81
	# assigne le deuxième objectif
	currobj = randi() % GrpObj.size()
	Obj = GrpObj[currobj]
	Obj.Id = 12
	Obj.NextPOI = 13
	

		
	GrpObj = get_tree().get_nodes_in_group("ThirdObj")
	#remet à plat les objectifs
	for i in range(GrpObj.size()):
		Obj = GrpObj[i]
		Obj.Id = 80
		Obj.NextPOI = 81
	# assigne le deuxième objectif
	currobj = randi() % GrpObj.size()
	Obj = GrpObj[currobj]
	Obj.Id = 13
	Obj.NextPOI = 14
	
	if currobj+1 < GrpObj.size():
		Obj = GrpObj[currobj+1]
		Obj.Id = 14
		Obj.NextPOI = 15
	else :
		Obj = GrpObj[currobj-1]
		Obj.Id = 14
		Obj.NextPOI = 15
		
	GrpObj = get_tree().get_nodes_in_group("FourthObj")
	#remet à plat les objectifs
	for i in range(GrpObj.size()):
		Obj = GrpObj[i]
		Obj.Id = 80
		Obj.NextPOI = 81
	# assigne le deuxième objectif
	currobj = randi() % GrpObj.size()
	Obj = GrpObj[currobj]
	Obj.Id = 15
	Obj.NextPOI = 16
	


func restart():
	total_time = 0
	if (ga.curr_generation in savePoints):
		save()

	
	if (ga.curr_generation >= 4000):
		get_tree().quit()
		return
	
	ga.evaluate_generation()
	if ga.best_species:
		if (ga.curr_best.fitness >= 80):
			max_generation_time = 40
		elif (ga.curr_best.fitness >= 70):
			max_generation_time = 30
		elif (ga.curr_best.fitness >= 50):
			max_generation_time = 25
		elif (ga.curr_best.fitness >= 40):
			max_generation_time = 17
		else:
			max_generation_time = 9
			
	if fitnessCSV and ga.best_species:
		fitnessCSV.store_string(str(ga.curr_best.fitness) + ";")
		
	definePOI()
	
	ga.next_generation()
	#place_bodies(ga.get_curr_bodies())


func place_bodies(new_bodies: Array) -> void:
	# remove all old bodies
	for body in container.get_children():
		body.queue_free()
	
	# add the new bodies 
	for body in new_bodies:
		body.IsTraining = true
		container.add_child(body)

func save():
	print("Save des agents, ", len(ga.curr_agents))
	var date = OS.get_datetime()
	var path = str(date['year']) + '-' + str(date['month']) + '-' + str(date['day']) + '/' + str(ga.curr_generation) + '/'
	
	ga.all_time_best.agent.network.save_to_json(path + "best_ever")
	ga.curr_best.agent.network.save_to_json(path + "best")
	

func _input(event):
	return
	
	if (Input.is_key_pressed(KEY_K)):
		# Save the network
		save()
	
	if (Input.is_key_pressed(KEY_G)):
		# Load the network
		if (saveToLoad != ""):
			var cDir = Directory.new()
			if cDir.open("user://network_configs/" + saveToLoad) == ERR_INVALID_PARAMETER:
				return
			cDir.list_dir_begin()
			var dirs = []
			var dir = cDir.get_next()
			while dir != "":
				if (!cDir.current_is_dir()):
					dirs.append(dir)
				dir = cDir.get_next()
			
			if (dirs[-1] != ""):
				if cDir.open(cDir.get_current_dir() + "/" + dirs[-1]) == ERR_INVALID_PARAMETER:
					print("Erreur de chargement, chemin " + cDir.get_current_dir() + "/" + dirs[-1])
					return
				
				cDir.list_dir_begin()
				var file = cDir.get_next()
				var numDone = 0
				while file != "":
					if file.ends_with(".json"):
						"""var net = loadNet(saveToLoad + "/" + dirs[-1] + "/" + file);
						if (file == "best_ever.json"):
							ga.all_time_best.agent.network = net
						elif (file == "best.json"):
							ga.curr_best.agent.network = net
						else:
							ga.curr_genomes[numDone].agent.network = net
							numDone = numDone + 1"""
					
					dir = cDir.get_next()
