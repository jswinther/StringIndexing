import os
import os.path
import seaborn as sns
import numpy as np
import pandas as pd
import matplotlib.pyplot as plt

path = os.getcwd() + '\\Results_48\\'
data_structures = [f for f in os.listdir(path) if os.path.isfile(os.path.join(path,f))]
query = ['top', 'mid', 'bot', 'construction']
dfAll = pd.DataFrame()
for q in query:
    for ds in data_structures:
        df = pd.read_csv(path + ds)
        df['name'] = ds
        sns.set()
        plt.figure()
        plt.yscale('log', base=2)
        sns.barplot(data = df, hue='data', x='length', y=q).set(title=ds + ' ' + q)
        plt.savefig('SinglePattern\\' + ds + '_' + q + '.png')
        dfAll = pd.concat([df,dfAll])

sns.set()
plt.figure()
plt.yscale('log', base=2)
sns.barplot(data = dfAll, hue='name', x='length', y='construction')
plt.savefig('SinglePattern\\' + 'all' + '_' + 'construction' + '.png')