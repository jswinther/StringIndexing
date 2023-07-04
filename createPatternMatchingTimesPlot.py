# -*- coding: utf-8 -*-
"""
Created on Mon Jun 19 10:49:50 2023

@author: frede
"""

import os
import os.path
import seaborn as sns
import numpy as np
import pandas as pd
import matplotlib.pyplot as plt

path = os.getcwd()
print(path)

os.chdir(path+"/StringIndexingGapResults/Results_2")
path = os.getcwd()
print(path)
#df = pd.read_csv('SA_R_V1_0.csv')

onlyfiles = [f for f in os.listdir(path) if os.path.isfile(os.path.join(path,f))]

df = pd.DataFrame()

for f in onlyfiles:
    print(f)
    df2 = pd.read_csv(f)
    df2 = df2[df2['data'].str.startswith('DNA_')]
    df2['dataStruct'] = f[:-4]
    df2 = df2[df2['dataStruct'].str.startswith('Fixed')]
    df = pd.concat([df,df2])
    
#df = df[df['data'].str.startswith('DNA_')]

dataLength = df['data'].str[4:]
df['dataLength'] = dataLength
#df = df.groupby(['dataLength','dataStruct'])
#df = df.replace(0,1)
#df = df.sort_values('dataLength')




sns.set()
plt.figure()
plt.yscale('log')
ax = sns.barplot(data = df, hue='dataStruct', x='dataLength', y='topSingle') #palette=['blue', 'red', 'yellow', 'grey'])
plt.show()


