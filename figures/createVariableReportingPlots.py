import os
import os.path
import seaborn as sns
import numpy as np
import pandas as pd
import matplotlib.pyplot as plt

path = os.getcwd() + '\\Results\\'
data_structures = [f for f in os.listdir(path) if os.path.isfile(os.path.join(path,f))]


base_data_structures = [
    'Variable_Report_ESA_BottomUp.csv',
    'Variable_Report_ESA_KdTrees.csv',
    'Variable_Report_PartialSort_TopNodes.csv',
    'Variable_Report_PartialSort.csv',
    'Variable_Report_ESA_Sorted.csv',
    'Variable_Report_ESA_Runtime.csv',
    'Variable_Report_SA_Runtime.csv',
    'Variable_Report_SuffixTree_1DRP.csv',
    'Variable_Report_PreComp_1DRP.csv'
]

"""
query = ['sptop','spmid','spbot','dptop','dpmid','dpbot', 'construction']
"""
query = ['dptop','dpmid','dpbot', 'construction']
dfBase = pd.DataFrame()

for ds in base_data_structures:
    df = pd.read_csv(path + ds)
    df['name'] = ds
    dfBase = pd.concat([df,dfBase])

for q in query:
    fig, axes = plt.subplots(2,2,figsize=(17,9),sharey=True)
    plt.yscale('log', base=2)
    dfDs = dfBase.loc[dfBase['data'] == 'english']
    sns.barplot(ax=axes[0,0], data = dfDs, hue='name', x='length', y=q).set(title='english ' + q)
    dfDs = dfBase.loc[dfBase['data'] == 'realDNA']
    sns.barplot(ax=axes[0,1], data = dfDs, hue='name', x='length', y=q).set(title='realDNA ' + q)
    dfDs = dfBase.loc[dfBase['data'] == 'DNA']
    sns.barplot(ax=axes[1,0], data = dfDs, hue='name', x='length', y=q).set(title='DNA ' + q)
    dfDs = dfBase.loc[dfBase['data'] == 'proteins']
    sns.barplot(ax=axes[1,1], data = dfDs, hue='name', x='length', y=q).set(title='proteins ' + q)
    plt.tight_layout()  
    plt.savefig('VariableReport\\variable_report_' + q + '.png')
    plt.close()
