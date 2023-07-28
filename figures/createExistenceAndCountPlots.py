import os
import os.path
import seaborn as sns
import numpy as np
import pandas as pd
import matplotlib.pyplot as plt

path = os.getcwd() + '\\Results\\'


data_structures = {
    "Count ESA + 1DRP": "Variable_Count_ESA_Runtime.csv",
    "Count ESA + Hash": "Fixed_Count_ESA_Runtime.csv",
    "Exist ESA + 1DRP": "Variable_Exist_ESA_Runtime.csv",
    "Exist ESA + Hash": "Fixed_Exist_ESA_Runtime.csv"
}

query = ['dptop','dpmid','dpbot', 'construction']
dfAll = pd.DataFrame()
for ds in data_structures:
    file = data_structures[ds]
    df = pd.read_csv(path + file)
    df['name'] = ds
    dfAll = pd.concat([df,dfAll])
    

sns.set_style('darkgrid')
for q in query:
    fig, axes = plt.subplots(3,1,figsize=(17,9),sharey=True)
    plt.yscale('log', base=2)
    dfDs = dfAll.loc[dfAll['data'] == 'english']
    sns.barplot(ax=axes[0], data = dfDs, hue='name', x='length', y=q).set(title='english ' + q)
    dfDs = dfAll.loc[dfAll['data'] == 'realDNA']
    sns.barplot(ax=axes[1], data = dfDs, hue='name', x='length', y=q).set(title='DNA ' + q)
    #dfDs = dfAll.loc[dfAll['data'] == 'DNA']
    #sns.barplot(ax=axes[1,0], data = dfDs, hue='name', x='length', y=q).set(title='DNA ' + q)
    dfDs = dfAll.loc[dfAll['data'] == 'proteins']
    sns.barplot(ax=axes[2], data = dfDs, hue='name', x='length', y=q).set(title='proteins ' + q)
    plt.tight_layout()  
    plt.savefig('Figures\\exist_count_' + q + '.png')
    plt.close()

