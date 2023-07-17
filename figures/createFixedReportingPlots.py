import os
import os.path
import seaborn as sns
import numpy as np
import pandas as pd
import matplotlib.pyplot as plt

path = os.getcwd() + '\\Results_3\\'
data_structures = [f for f in os.listdir(path) if os.path.isfile(os.path.join(path,f))]
data_set = ["english,realDNA,DNA,proteins"]


query = ['sptop','spmid','spbot','dptop','dpmid','dpbot', 'construction']
dfAll = pd.DataFrame()
for q in query:
    for ds in data_structures:
        if ds.startswith('Fixed_Report'):
            df = pd.read_csv(path + ds)
            df['name'] = ds
            sns.set()
            plt.figure()
            plt.yscale('log', base=2)
            sns.barplot(data = df, hue='data', x='length', y=q).set(title=ds + ' ' + q)
            plt.savefig('FixedReport\\' + ds + '_' + q + '.png')
            dfAll = pd.concat([df,dfAll])

for ds in data_set:
    sns.set()
    plt.figure()
    plt.yscale('log', base=2)
    dfDs = dfAll[ds]
    sns.barplot(data = dfAll, hue='name', x='length', y='construction')
    plt.savefig('FixedReport\\' + 'all' + '_' + 'construction' + '.png')

