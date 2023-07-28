import os
import os.path
import seaborn as sns
import numpy as np
import pandas as pd
import matplotlib.pyplot as plt

path = os.getcwd() + '\\Results\\'
data_structures = [f for f in os.listdir(path) if os.path.isfile(os.path.join(path,f))]


base_data_structures = {
    "ESA + 2DRR": 'Variable_Report_ESA_KdTrees.csv',
    "ESA + Partial Sorting and Top Nodes": 'Variable_Report_PartialSort_TopNodes.csv',
    "ESA + Partial Sorting": 'Variable_Report_PartialSort.csv',
    "ESA + Sorted": 'Variable_Report_ESA_Sorted.csv',
    "ESA + Runtime 1DRR": 'Variable_Report_ESA_Runtime.csv',
    "Suffix Array + 1DRR": 'Variable_Report_SA_Runtime.csv',
    "Suffix Tree + 1DRR": 'Variable_Report_SuffixTree_1DRP.csv',
    "Precomputed substrings + 1DRR": 'Variable_Report_PreComp_1DRP.csv'
}

"""
query = ['sptop','spmid','spbot','dptop','dpmid','dpbot', 'construction']
"""
query = ['dptop','dpmid','dpbot']
dfBase = pd.DataFrame()
sns.set_style('darkgrid')
for e in base_data_structures:
    ds = base_data_structures[e]
    df = pd.read_csv(path + ds)
    df['name'] = e
    dfBase = pd.concat([df,dfBase])

fig, axes = plt.subplots(3,1,figsize=(11,16),sharey=True)
plt.yscale('log', base=10)
dfDs = dfBase.loc[dfBase['data'] == 'english']
sns.barplot(ax=axes[0], data = dfDs, hue='name', x='length', y='construction').set(title='english ' + 'construction')
dfDs = dfBase.loc[dfBase['data'] == 'realDNA']
sns.barplot(ax=axes[1], data = dfDs, hue='name', x='length', y='construction').set(title='DNA ' + 'construction')
#dfDs = dfBase.loc[dfBase['data'] == 'DNA']
#sns.barplot(ax=axes[1,0], data = dfDs, hue='name', x='length', y=q).set(title='DNA ' + q)
dfDs = dfBase.loc[dfBase['data'] == 'proteins']
sns.barplot(ax=axes[2], data = dfDs, hue='name', x='length', y='construction').set(title='proteins ' + 'construction')
plt.tight_layout()  
plt.savefig('Figures\\variable_report_' + 'construction' + '.png')
plt.close()

base_data_structures = {
    "ESA + 2DRR": 'Variable_Report_ESA_KdTrees.csv',
    "ESA + Partial Sorting and Top Nodes": 'Variable_Report_PartialSort_TopNodes.csv',
    "ESA + Partial Sorting": 'Variable_Report_PartialSort.csv',
    "ESA + Sorted": 'Variable_Report_ESA_Sorted.csv',
    "ESA + Runtime 1DRR": 'Variable_Report_ESA_Runtime.csv',
    "Suffix Array + 1DRR": 'Variable_Report_SA_Runtime.csv',
    "Suffix Tree + 1DRR": 'Variable_Report_SuffixTree_1DRP.csv',
}

dfBase = pd.DataFrame()

for e in base_data_structures:
    ds = base_data_structures[e]
    df = pd.read_csv(path + ds)
    df['name'] = e
    dfBase = pd.concat([df,dfBase])

for q in query:
    fig, axes = plt.subplots(3,1,figsize=(11,16),sharey=True)
    plt.yscale('log', base=10)
    dfDs = dfBase.loc[dfBase['data'] == 'english']
    sns.barplot(ax=axes[0], data = dfDs, hue='name', x='length', y=q).set(title='english ' + q)
    dfDs = dfBase.loc[dfBase['data'] == 'realDNA']
    sns.barplot(ax=axes[1], data = dfDs, hue='name', x='length', y=q).set(title='DNA ' + q)
    #dfDs = dfBase.loc[dfBase['data'] == 'DNA']
    #sns.barplot(ax=axes[1,0], data = dfDs, hue='name', x='length', y=q).set(title='DNA ' + q)
    dfDs = dfBase.loc[dfBase['data'] == 'proteins']
    sns.barplot(ax=axes[2], data = dfDs, hue='name', x='length', y=q).set(title='proteins ' + q)
    plt.tight_layout()  
    plt.savefig('Figures\\variable_report_' + q + '.png')
    plt.close()
