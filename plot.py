import pandas as pd
import math
import numpy as np
import os
from pathlib import Path
import matplotlib.pyplot as plt
from scipy.stats import norm

# get data as pandas dataframe
DIR_PATH = Path(__file__).parent
PATH_TO_DATA = os.path.join(DIR_PATH, "Data\\density.csv")

data = pd.read_csv(PATH_TO_DATA, sep=';', decimal=",")

density = np.transpose(data.values[:,1:])

header = np.delete(np.array(data.columns.values), 0)

x = np.array([float(_x.replace(',', '.')) for _x in header])
t = data.iloc[:, 0].to_numpy()

# Create meshgrid
tt, xx = np.meshgrid(t, x)

z_lim = 1.0
density_capped = np.minimum(density, np.full((density.shape), z_lim))

fig = plt.figure(figsize=(13, 7))
ax = plt.axes(projection='3d')
surf = ax.plot_surface(xx, tt, density_capped, rstride=5, cstride=5, cmap='coolwarm', edgecolor='none')
ax.set_xlabel('x')
ax.set_ylabel('t')
ax.set_zlabel('PDF')
ax.set_title('Surface plot of density function')
fig.colorbar(surf, shrink=0.5, aspect=5) # add color bar indicating the PDF
ax.axes.set_zlim3d(bottom=0, top=z_lim) 
ax.view_init(50, 40)

plt.show()


t_steps = 10
t_indices = np.arange(0, len(t)-1, t_steps)

for _it in t_indices :
    plt.plot(density[:,_it], color='red', label='density from PDE')
    std_dev = math.sqrt(t[_it]) * .35
    density_theo = np.array([norm.pdf(_x, loc=.2 + .2 * t[_it], scale=std_dev) for _x in x])
    plt.plot(density_theo, color='blue', label='density theoretical')
    plt.legend()
    plt.show()