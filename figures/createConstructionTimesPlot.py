import seaborn as sns
import numpy as np
import pandas as pd
import matplotlib.pyplot as plt

df = pd.read_csv('saf_constructions.csv')

sns.set()
plt.figure()

ax = sns.lineplot(data = df, hue='set', x='size', y='time', palette=['blue', 'red', 'yellow', 'grey'])
plt.show()


