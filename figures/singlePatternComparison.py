import os
import os.path
import seaborn as sns
import numpy as np
import pandas as pd
import matplotlib.pyplot as plt

path = os.getcwd() + '\\Results\\'
data_structures = [f for f in os.listdir(path) if os.path.isfile(os.path.join(path,f))]
query = ['topMatching', 'midMatching', 'botMatching', 'construction', 'topReporting', 'midReporting', 'botReporting']

dfAll = pd.DataFrame()
for ds in data_structures:
    if ds.startswith('Suffix') or ds.startswith('Precomp') or ds.startswith('Enhanced'):
        df = pd.read_csv(path + ds)
        df['name'] = ds
        dfAll = pd.concat([df,dfAll])
    
sns.set_style('darkgrid')
for q in query:
    fig, axes = plt.subplots(3,1,figsize=(11,16),sharey=True)
    
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
    plt.savefig('Figures\\singlepattern_' + q + '.png')
    plt.close()

