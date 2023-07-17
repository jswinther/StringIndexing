import os
import os.path
import seaborn as sns
import numpy as np
import pandas as pd
import matplotlib.pyplot as plt

path = os.getcwd() + '\\Results_3\\'
data_structures = [f for f in os.listdir(path) if os.path.isfile(os.path.join(path,f))]
data_set = ['english','realDNA','DNA','proteins']


query = ['sptop','spmid','spbot','dptop','dpmid','dpbot', 'construction']
dfAll = pd.DataFrame()
for q in query:
    for ds in data_structures:
        if ds.startswith('Variable_Report'):
            df = pd.read_csv(path + ds)
            df['name'] = ds
            sns.set()
            plt.figure(figsize=(17,9))
            plt.yscale('log', base=2)
            sns.barplot(data = df, hue='data', x='length', y=q).set(title=ds + ' ' + q)
            plt.tight_layout()
            plt.savefig('VariableReport\\' + ds + '_' + q + '.png')
            plt.close()
            dfAll = pd.concat([df,dfAll])

for d in data_set:
    for q in query:
        sns.set()
        plt.figure(figsize=(17,9))
        plt.yscale('log', base=2)
        dfDs = dfAll.loc[dfAll['data'] == d]
        sns.barplot(data = dfAll, hue='name', x='length', y=q).set(title=d + ' ' + q)
        plt.tight_layout()
        plt.savefig('VariableReport\\' + d + '_variable_reporting_' + q + '.png')
        plt.close()


