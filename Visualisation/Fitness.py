# -*- coding: utf-8 -*-
"""
Created on Sun Apr 21 08:09:10 2024

@author: dimit
"""

import pandas as pd
import matplotlib.pyplot as plt
from matplotlib.ticker import ScalarFormatter

# Charger les données
nom_fichier = "C:/Users/dimit/Downloads/fitness(8).csv"
donnees = pd.read_csv(nom_fichier, delimiter=';', header=None)

# Prendre la seule ligne de données
ligne_de_donnees = donnees.iloc[0]

# Création du graphique
plt.figure(figsize=(20, 6))
plt.scatter(range(0, len(ligne_de_donnees)), ligne_de_donnees, color='b', label='Données')

plt.xlabel("Index")
plt.ylabel("Données")
plt.yscale('log')  # Utilisation d'une échelle logarithmique pour l'axe y
plt.grid(True)
plt.legend()


plt.gca().yaxis.set_major_formatter(ScalarFormatter(useMathText=True))

# Affichage du graphique
plt.show()