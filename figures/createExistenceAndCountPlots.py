import os
import os.path
import seaborn as sns
import numpy as np
import pandas as pd
import matplotlib.pyplot as plt

path = os.getcwd() + '\\Results_3\\'
data_structures = [f for f in os.listdir(path) if os.path.isfile(os.path.join(path,f))]


query = ['dptop','dpmid','dpbot', 'construction']
dfAll = pd.DataFrame()
for q in query:
    for ds in data_structures:
        if ds.__contains__('Count') or ds.__contains__('Exist'):
            df = pd.read_csv(path + ds)
            df['name'] = ds
            sns.set()
            plt.figure(figsize=(17,9))
            plt.yscale('log', base=2)
            sns.barplot(data = df, hue='data', x='length', y=q).set(title=ds + ' ' + q)
            plt.tight_layout()
            plt.savefig('CountAndExistenceFixedAndVariable\\' + ds + '_' + q + '.png')
            plt.close()
            dfAll = pd.concat([df,dfAll])

data_set = ['english','realDNA','DNA','proteins']
for d in data_set:
    for q in query:
        sns.set()
        plt.figure(figsize=(17,9))
        plt.yscale('log', base=2)
        dfDs = dfAll.loc[dfAll['data'] == d]
        sns.barplot(data = dfAll, hue='name', x='length', y=q).set(title=d + ' ' + q)
        plt.tight_layout()
        plt.savefig('CountAndExistenceFixedAndVariable\\' + d + '_' + q + '.png')
        plt.close()

