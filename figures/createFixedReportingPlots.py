import os
import os.path
import seaborn as sns
import numpy as np
import pandas as pd
import matplotlib.pyplot as plt

path = os.getcwd() + '\\Results\\'
data_structures = {
    "ESA + Partial Hashing": 'Fixed_Report_ESA_PartialHash.csv',
    "ESA + Hashed": 'Fixed_Report_ESA_Hashed.csv',
    "ESA + Runtime Hashing": 'Fixed_Report_ESA_Runtime.csv',
    "SA + Runtime Hashing": 'Fixed_Report_SA_Runtime.csv',
    "Suffix tree": 'Fixed_Report_SuffixTree_Hash.csv',
    "Precomputed substring": 'Fixed_Report_PreComp_Hash.csv'
}

sns.set_style('darkgrid')
dfAll = pd.DataFrame()
for name in data_structures:
    file = data_structures[name]
    df = pd.read_csv(path + file)
    df['name'] = name
    dfAll = pd.concat([df,dfAll])

fig, axes = plt.subplots(3,1,figsize=(11,16),sharey=True)

plt.yscale('log', base=10)
dfDs = dfAll.loc[dfAll['data'] == 'english']
sns.barplot(ax=axes[0], data = dfDs, hue='name', x='length', y='construction').set(title='english ' + 'construction')
dfDs = dfAll.loc[dfAll['data'] == 'realDNA']
sns.barplot(ax=axes[1], data = dfDs, hue='name', x='length', y='construction').set(title='DNA ' + 'construction')
#dfDs = dfAll.loc[dfAll['data'] == 'DNA']
#sns.barplot(ax=axes[1,0], data = dfDs, hue='name', x='length', y=q).set(title='DNA ' + q)
dfDs = dfAll.loc[dfAll['data'] == 'proteins']
sns.barplot(ax=axes[2], data = dfDs, hue='name', x='length', y='construction').set(title='proteins ' + 'construction')
plt.tight_layout()  
plt.savefig('Figures\\fixed_report_' + 'construction' + '.png')


query = ['dptop','dpmid','dpbot']

data_structures = {
    "ESA + Partial Hashing": 'Fixed_Report_ESA_PartialHash.csv',
    "ESA + Hashed": 'Fixed_Report_ESA_Hashed.csv',
    "ESA + Runtime Hashing": 'Fixed_Report_ESA_Runtime.csv',
    "SA + Runtime Hashing": 'Fixed_Report_SA_Runtime.csv',
    "Suffix tree": 'Fixed_Report_SuffixTree_Hash.csv'
}

dfAll = pd.DataFrame()
for name in data_structures:
    file = data_structures[name]
    df = pd.read_csv(path + file)
    df['name'] = name
    dfAll = pd.concat([df,dfAll])

for q in query:
    fig, axes = plt.subplots(3,1,figsize=(11,16),sharey=True)
    
    plt.yscale('log', base=10)
    dfDs = dfAll.loc[dfAll['data'] == 'english']
    sns.barplot(ax=axes[0], data = dfDs, hue='name', x='length', y=q).set(title='english ' + q)
    dfDs = dfAll.loc[dfAll['data'] == 'realDNA']
    sns.barplot(ax=axes[1], data = dfDs, hue='name', x='length', y=q).set(title='DNA ' + q)
    #dfDs = dfAll.loc[dfAll['data'] == 'DNA']
    #sns.barplot(ax=axes[1,0], data = dfDs, hue='name', x='length', y=q).set(title='DNA ' + q)
    dfDs = dfAll.loc[dfAll['data'] == 'proteins']
    sns.barplot(ax=axes[2], data = dfDs, hue='name', x='length', y=q).set(title='proteins ' + q)
    plt.tight_layout()  
    plt.savefig('Figures\\fixed_report_' + q + '.png')

plt.close()