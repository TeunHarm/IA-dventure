import json
import matplotlib.pyplot as plt

def load_network_from_json(json_file):
    with open(json_file, 'r') as f:
        network_data = json.load(f)
    return network_data

def load_data(network_data):
    input_layer = []
    hidden_layers = {}
    output_layer = []
    curve_dic = {}

    for neuron in network_data["neurons"]:
        if neuron["type"] == 0:
            input_layer.append(neuron["id"])
            curve_dic[neuron["id"]] = round(neuron["curve"], 2)
        if neuron["type"] == 3:
            output_layer.append(neuron["id"])
            curve_dic[neuron["id"]] = round(neuron["curve"], 2)
        elif neuron["id"] == 1:
            input_layer.append(neuron["id"])
            curve_dic[neuron["id"]] = round(neuron["curve"], 2)
    for neuron in network_data["neurons"]:
        if neuron["type"] == 2 and connectedToOutput(neuron["id"], network_data["links"], input_layer, output_layer, 0):
            layer = count_connections(neuron["id"], network_data["links"], input_layer, output_layer, 0)
            if layer not in hidden_layers:
                hidden_layers[layer] = []
            hidden_layers[layer].append(neuron["id"])
            curve_dic[neuron["id"]] = round(neuron["curve"], 2)

    return input_layer, hidden_layers, output_layer, curve_dic

def count_connections(neuron_id, links, input_layer, output_layer, reclim):
    if reclim > 30:
        return 0
    count = -99999
    for link in links:
        if not(link["from"] == link["to"]) and not(link["from"] in output_layer):
            if link["to"] == neuron_id and not(check_for_loop(neuron_id, link["from"], links)):
                if link["from"] in input_layer:
                    if count<1:
                        count = 1
                else:
                    temp_count = count_connections(link["from"], links, input_layer, output_layer, reclim+1) + 1
                    if temp_count>count:
                        count = temp_count
    return count

def connectedToOutput (neuron_id, links, input_layer, output_layer, reclim):
    if reclim > 30:
        return False
    for link in links:
        if not(link["from"] == link["to"]) and not(link["to"] in input_layer):
            if link["from"] == neuron_id:
                if link["to"] in output_layer:
                    return True
                elif connectedToOutput(link["to"], links, input_layer, output_layer, reclim+1):
                    return True
    return False

def check_for_loop(neuron_id, neuron_id2, links):
    for link in links:
        if link["from"] == neuron_id2 and link["to"] == neuron_id:
            return True
    return False
def neuron_by_layer( hidden_layers):
    neuron = []
    for key in hidden_layers.keys():
        z = 0
        if not(key <= -1):
            for neurons in hidden_layers[key]:
                    z += 1
            neuron.append(z)
    return neuron
def draw_network(input_layer, hidden_layers, output_layer, curve_dict, links):


    hidden_layer_size = neuron_by_layer(hidden_layers)
    neuron_coo = {}

    if len(hidden_layer_size) == 0:
        hidden_layer_size.append(0)
    max_layer = max(len(input_layer)+1, max(hidden_layer_size), len(output_layer))
    plt.figure(figsize=(17, 10))

    for i, neuron_id in enumerate(input_layer):
        plt.scatter(0, i + (max_layer - len(input_layer)) / 2, color='blue', s=600, zorder=2)
        plt.text(0, i + (max_layer-len(input_layer))/2+0.125, f'{neuron_id}', ha='center', va='center', fontsize=10)
        plt.text(0, i + (max_layer - len(input_layer)) /2 -0.225, f'{curve_dict[neuron_id]}', ha='center', va='center', fontsize=8)
        neuron_coo[neuron_id] = [0, i + (max_layer-len(input_layer))/2]

    k = 0
    max_key = 0
    for key in hidden_layers.keys():
        z = 0
        if not (key <= -1):
            for neuron_id in hidden_layers[key]:
                plt.scatter(key, z + (max_layer - hidden_layer_size[k]) / 2, color='blue', s=500, zorder=2)
                plt.text(key, z + (max_layer - hidden_layer_size[k]) /2 +0.125, f'{neuron_id}', ha='center', va='center',
                         fontsize=10)
                plt.text(key,z + (max_layer - hidden_layer_size[k]) /2 - 0.225, f'{curve_dict[neuron_id]}', ha='center',
                         va='center', fontsize=8)
                neuron_coo[neuron_id] = [key, z + (max_layer - hidden_layer_size[k]) / 2]
                z += 1
            k += 1
            if key>max_key:
                max_key = key

    for i, neuron_id in enumerate(output_layer):
        plt.scatter(max_key+1, i + (max_layer-len(output_layer))/2, color='blue', s=500, zorder=2)
        plt.text(max_key+1, i + (max_layer-len(output_layer))/2+0.125, f'{neuron_id}', ha='center', va='center', fontsize=10)
        plt.text(max_key+1, i + (max_layer-len(output_layer))/2 - 0.225, f'{curve_dict[neuron_id]}', ha='center',
                 va='center', fontsize=8)
        neuron_coo[neuron_id] = [max_key+1, i + (max_layer-len(output_layer))/2]

    for link in links:
        if not (link["from"] == link["to"]) and not(link["from"] in output_layer) and not(link["from"] in input_layer and link["to"] in input_layer) and not(link["from"] in output_layer and link["to"] in output_layer):
            from_neuron = link["from"]
            to_neuron = link["to"]
            if (from_neuron in neuron_coo) and (to_neuron in neuron_coo):
                if link["weight"]>0 :
                    plt.plot([neuron_coo[from_neuron][0], neuron_coo[to_neuron][0]], [neuron_coo[from_neuron][1], neuron_coo[to_neuron][1]], color='green', zorder=1)
                else:
                    plt.plot([neuron_coo[from_neuron][0], neuron_coo[to_neuron][0]],
                             [neuron_coo[from_neuron][1], neuron_coo[to_neuron][1]], color='red', zorder=1)

    plt.xlabel('Layer')
    plt.axis('off')

    return plt



if __name__ == "__main__":
    for i in range(0, 350):
        savePoints = [10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240,
                      250, 260, 270, 280, 290, 300, 310, 320, 330, 340, 350, 360, 370, 380, 390, 400, 410, 420, 430, 440, 450, 460, 470,
                      480, 490, 500, 510, 520, 530, 540, 550, 560, 570, 580, 590, 600, 610, 620, 630, 640, 650, 660, 670, 680, 690, 700,
                      710, 720, 730, 740, 750, 760, 770, 780, 790, 800, 810, 820, 830, 840, 850, 860, 870, 880, 890, 900, 910, 920, 930, 940,
                      950, 960, 970, 980, 990, 1000, 1010, 1020, 1030, 1040, 1050, 1060, 1070, 1080, 1090, 1100, 1110, 1120, 1130, 1140, 1150,
                      1160, 1170, 1180, 1190, 1200, 1210, 1220, 1230, 1240, 1250, 1260, 1270, 1280, 1290, 1300, 1310, 1320, 1330, 1340, 1350, 1360,
                      1370, 1380, 1390, 1400, 1410, 1420, 1430, 1440, 1450, 1460, 1470, 1480, 1490, 1500, 1510, 1520, 1530, 1540, 1550, 1560, 1570,
                      1580, 1590, 1600, 1610, 1620, 1630, 1640, 1650, 1660, 1670, 1680, 1690, 1700, 1710, 1720, 1730, 1740, 1750, 1760, 1770, 1780,
                      1790, 1800, 1810, 1820, 1830, 1840, 1850, 1860, 1870, 1880, 1890, 1900, 1910, 1920, 1930, 1940, 1950, 1960, 1970, 1980, 1990,
                      2000, 2010, 2020, 2030, 2040, 2050, 2060, 2070, 2080, 2090, 2100, 2110, 2120, 2130, 2140, 2150, 2160, 2170, 2180, 2190, 2200,
                      2210, 2220, 2230, 2240, 2250, 2260, 2270, 2280, 2290, 2300, 2310, 2320, 2330, 2340, 2350, 2360, 2370, 2380, 2390, 2400, 2410,
                      2420, 2430, 2440, 2450, 2460, 2470, 2480, 2490, 2500, 2510, 2520, 2530, 2540, 2550, 2560, 2570, 2580, 2590, 2600, 2610, 2620,
                      2630, 2640, 2650, 2660, 2670, 2680, 2690, 2700, 2710, 2720, 2730, 2740, 2750, 2760, 2770, 2780, 2790, 2800, 2810, 2820, 2830,
                      2840, 2850, 2860, 2870, 2880, 2890, 2900, 2910, 2920, 2930, 2940, 2950, 2960, 2970, 2980, 2990, 3000,3010, 3020,
                      3030, 3040, 3050, 3060, 3070, 3080, 3090, 3100, 3110, 3120, 3130, 3140, 3150, 3160, 3170, 3180, 3190,
 3200, 3210, 3220, 3230, 3240, 3250, 3260, 3270, 3280, 3290, 3300, 3310, 3320, 3330, 3340, 3350, 3360,
 3370, 3380, 3390, 3400, 3410, 3420, 3430, 3440, 3450, 3460, 3470, 3480, 3490, 4000, 5000]
        json_file = ("C:/Users/dimit/AppData/Roaming/Godot/app_userdata/Ia-dventure/network_configs/2024-5-10/" + str(savePoints[i])
                     + "/best.json")  # Replace with the path to your JSON file
        network_data = load_network_from_json(json_file)
        input_layer, hidden_layers, output_layer, curve_dict = load_data(network_data)
        plt = draw_network(input_layer, hidden_layers, output_layer, curve_dict, network_data["links"])
        plt.title('Best ever generation:' + str(savePoints[i]))
        ##plt.show()
        plt.savefig("C:/Users/dimit/Documents/Iadventureplot/" + str(savePoints[i]) + ".png")
        plt.close()


