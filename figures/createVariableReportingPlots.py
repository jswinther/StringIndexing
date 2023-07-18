import os
import os.path
import seaborn as sns
import numpy as np
import pandas as pd
import matplotlib.pyplot as plt

path = os.getcwd() + '\\Results_3\\'
data_structures = [f for f in os.listdir(path) if os.path.isfile(os.path.join(path,f))]


query = ['sptop','spmid','spbot','dptop','dpmid','dpbot', 'construction']
dfAll = pd.DataFrame()
for ds in data_structures:
    if ds.startswith('Variable_Report'):
        df = pd.read_csv(path + ds)
        df['name'] = ds
        dfAll = pd.concat([df,dfAll])

for q in query:
    fig, axes = plt.subplots(2,2,figsize=(17,9),sharey=True)
    plt.yscale('log', base=2)
    dfDs = dfAll.loc[dfAll['data'] == 'english']
    sns.barplot(ax=axes[0,0], data = dfDs, hue='name', x='length', y=q).set(title='english ' + q)
    dfDs = dfAll.loc[dfAll['data'] == 'realDNA']
    sns.barplot(ax=axes[0,1], data = dfDs, hue='name', x='length', y=q).set(title='realDNA ' + q)
    dfDs = dfAll.loc[dfAll['data'] == 'DNA']
    sns.barplot(ax=axes[1,0], data = dfDs, hue='name', x='length', y=q).set(title='DNA ' + q)
    dfDs = dfAll.loc[dfAll['data'] == 'proteins']
    sns.barplot(ax=axes[1,1], data = dfDs, hue='name', x='length', y=q).set(title='proteins ' + q)
    plt.tight_layout()  
    plt.savefig('VariableReport\\variable_report_' + q + '.png')
    plt.close()

