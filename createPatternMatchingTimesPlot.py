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

path = os.getcwd() + '\\Results_1'
data_structures = [f for f in os.listdir(path) if os.path.isfile(os.path.join(path,f))]

for ds in data_structures:
    df = pd.read_csv(path + '\\' + ds)
    sns.set()
    plt.figure()
    plt.yscale('log')
    ax = sns.barplot(data = df, hue='data', x='length', y='query')
    plt.savefig(ds + '.png')



df = pd.DataFrame()

for f in data_structures:
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

## PLOTTING DNA ##
sns.set()
plt.figure()
plt.yscale('log')
ax = sns.barplot(data = df, hue='dataStruct', x='dataLength', y='topSingle') #palette=['blue', 'red', 'yellow', 'grey'])
plt.show()

## ONLY ENGLISH ##
for f in onlyfiles:
    print(f)
    df2 = pd.read_csv(f)
    df2 = df2[df2['data'].str.startswith('english_')]
    df2['dataStruct'] = f[:-4]
    df2 = df2[df2['dataStruct'].str.startswith('Fixed')]
    df = pd.concat([df,df2])
    
#df = df[df['data'].str.startswith('DNA_')]

dataLength = df['data'].str[4:]
df['dataLength'] = dataLength
#df = df.groupby(['dataLength','dataStruct'])
#df = df.replace(0,1)
#df = df.sort_values('dataLength')

## PLOTTING ENGLISH ##
sns.set()
plt.figure()
plt.yscale('log')
ax = sns.barplot(data = df, hue='dataStruct', x='dataLength', y='topSingle') #palette=['blue', 'red', 'yellow', 'grey'])
plt.show()

## ONLY PROTEINS ##
for f in onlyfiles:
    print(f)
    df2 = pd.read_csv(f)
    df2 = df2[df2['data'].str.startswith('proteins')]
    df2['dataStruct'] = f[:-4]
    df2 = df2[df2['dataStruct'].str.startswith('Fixed')]
    df = pd.concat([df,df2])
    
#df = df[df['data'].str.startswith('DNA_')]

dataLength = df['data'].str[4:]
df['dataLength'] = dataLength
#df = df.groupby(['dataLength','dataStruct'])
#df = df.replace(0,1)
#df = df.sort_values('dataLength')

## PLOTTING PROTEINS ##
sns.set()
plt.figure()
plt.yscale('log')
ax = sns.barplot(data = df, hue='dataStruct', x='dataLength', y='topSingle') #palette=['blue', 'red', 'yellow', 'grey'])
plt.show()

## ONLY REALDNA ##
for f in onlyfiles:
    print(f)
    df2 = pd.read_csv(f)
    df2 = df2[df2['data'].str.startswith('realDNA')]
    df2['dataStruct'] = f[:-4]
    df2 = df2[df2['dataStruct'].str.startswith('Fixed')]
    df = pd.concat([df,df2])
    
#df = df[df['data'].str.startswith('DNA_')]

dataLength = df['data'].str[4:]
df['dataLength'] = dataLength
#df = df.groupby(['dataLength','dataStruct'])
#df = df.replace(0,1)
#df = df.sort_values('dataLength')

## PLOTTING REALDNA ##
sns.set()
plt.figure()
plt.yscale('log')
ax = sns.barplot(data = df, hue='dataStruct', x='dataLength', y='topSingle') #palette=['blue', 'red', 'yellow', 'grey'])
plt.show()


