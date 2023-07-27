import os
import os.path
import seaborn as sns
import numpy as np
import pandas as pd
import matplotlib.pyplot as plt

path = os.getcwd() + '\\Results\\'
data_structures = [f for f in os.listdir(path) if os.path.isfile(os.path.join(path,f))]



base_data_structures = [
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
query = ['dptop','dpmid','dpbot']
dfBase = pd.DataFrame()

for ds in base_data_structures:
    df = pd.read_csv(path + ds)
    df['name'] = ds
    dfBase = pd.concat([df,dfBase])

fig, axes = plt.subplots(3,1,figsize=(11,16),sharey=True)
plt.yscale('log', base=2)
dfDs = dfBase.loc[dfBase['data'] == 'english']
sns.barplot(ax=axes[0], data = dfDs, hue='name', x='length', y='construction').set(title='english ' + 'construction')
dfDs = dfBase.loc[dfBase['data'] == 'realDNA']
sns.barplot(ax=axes[1], data = dfDs, hue='name', x='length', y='construction').set(title='realDNA ' + 'construction')
#dfDs = dfBase.loc[dfBase['data'] == 'DNA']
#sns.barplot(ax=axes[1,0], data = dfDs, hue='name', x='length', y=q).set(title='DNA ' + q)
dfDs = dfBase.loc[dfBase['data'] == 'proteins']
sns.barplot(ax=axes[2], data = dfDs, hue='name', x='length', y='construction').set(title='proteins ' + 'construction')
plt.tight_layout()  
plt.savefig('VariableReport\\variable_report_' + 'construction' + '.png')
plt.close()

base_data_structures = [
    'Variable_Report_ESA_KdTrees.csv',
    'Variable_Report_PartialSort_TopNodes.csv',
    'Variable_Report_PartialSort.csv',
    'Variable_Report_ESA_Sorted.csv',
    'Variable_Report_ESA_Runtime.csv',
    'Variable_Report_SA_Runtime.csv',
    'Variable_Report_SuffixTree_1DRP.csv',
]

dfBase = pd.DataFrame()

for ds in base_data_structures:
    df = pd.read_csv(path + ds)
    df['name'] = ds
    dfBase = pd.concat([df,dfBase])

for q in query:
    fig, axes = plt.subplots(3,1,figsize=(11,16),sharey=True)
    plt.yscale('log', base=2)
    dfDs = dfBase.loc[dfBase['data'] == 'english']
    sns.barplot(ax=axes[0], data = dfDs, hue='name', x='length', y=q).set(title='english ' + q)
    dfDs = dfBase.loc[dfBase['data'] == 'realDNA']
    sns.barplot(ax=axes[1], data = dfDs, hue='name', x='length', y=q).set(title='realDNA ' + q)
    #dfDs = dfBase.loc[dfBase['data'] == 'DNA']
    #sns.barplot(ax=axes[1,0], data = dfDs, hue='name', x='length', y=q).set(title='DNA ' + q)
    dfDs = dfBase.loc[dfBase['data'] == 'proteins']
    sns.barplot(ax=axes[2], data = dfDs, hue='name', x='length', y=q).set(title='proteins ' + q)
    plt.tight_layout()  
    plt.savefig('VariableReport\\variable_report_' + q + '.png')
    plt.close()
